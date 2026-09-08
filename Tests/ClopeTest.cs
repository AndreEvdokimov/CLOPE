using CLOPE.Clusters;
using CLOPE.Core;
using CLOPE.Import;
using CLOPE.Transactions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace CLOPE.Tests
{
    [TestFixture]
    [SingleThreaded]
    internal class ClopeTest
    {
        private TextFile moohTxt;
        // Параметры набора транзакций с грибами
        private TransactionSetParams moohSetParams = new TransactionSetParams() { SkippedCols = new int[] { 0, 1 }, Delimiter = ',', NullValues = new string[] { "?" } };
        private ClusterSet clusters = new ClusterSet();
        private TransactionSet moohTrs;
        private ClopeEngine clope = new ClopeEngine();
    

        [OneTimeSetUp]
        public void LoadMoohTestDataToTansactions() 
        {
            // Путь до файла с параметрами грибов
            string mooh = CLOPE.Helpers.Helpers.DataFilePath("mooh_with_ids.txt");
            
            Assert.That(mooh, Is.Not.Empty);

            // Читаем строки текстового файла
            this.moohTxt = new TextFile(mooh);

            Assert.That(this.moohTxt.GetRow(), Is.Not.Empty);

            this.moohTrs = new TransactionSet(this.moohTxt, this.moohSetParams);

            Assert.That(this.moohTrs.Count, Is.Positive, "Количество транзакций должно быть больше 0");
        }

        [Test]
        public void Clope() 
        {
            double repulsion = 2.6;

            ClusterSet clusters = new();

            // Результат работы алгоритма
            var clopeRes = this.clope.Run(this.moohTrs, clusters, repulsion);

            Assert.That(clopeRes.RowsCount(), Is.Positive, "Количество записей в таблице должно быть больше 0");
        }

        [Test]
        public void RepulsionNotGreaterThanOneException()
        {
            double repulsion = 0.6;

            var exception = Assert.Throws<ArgumentException>(
                new Action(() => this.clope.Run(this.moohTrs, this.clusters, repulsion)));

            Assert.That(exception.Message, Does.Contain("больше 1"));
        }

        [Test]
        public void EmptyTransactionSetException()
        {
            double repulsion = 2.6;

            // Читаем пустой текстовый файл
            var emptyTxt = new TextFile(CLOPE.Helpers.Helpers.DataFilePath("empty.txt"));

            // Подготавливаем ПУСТОЙ набор транзакций
            var transactions = new TransactionSet(emptyTxt, moohSetParams);

            Assert.That(transactions.Count, Is.EqualTo(0));

            var exception = Assert.Throws<ArgumentException>(
                new Action(() => this.clope.Run(transactions, this.clusters, repulsion)));

            Assert.That(exception.Message, Does.Contain("Набор транзакций пуст"));
        }

    }
}
