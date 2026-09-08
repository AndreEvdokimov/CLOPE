using CLOPE.Import;
using CLOPE.Transactions;
using NUnit.Framework;

namespace CLOPE.Tests
{
    [TestFixture]
    internal class TransactionTest
    {
        [Test]
        public void LoadTransactions()
        {
            TransactionSetParams moohSetParams = new TransactionSetParams()
            {
                SkippedCols = new int[] { 0, 1 },
                Delimiter = ',',
                NullValues = new string[] { "?" }
            };

            string mooh50 = CLOPE.Helpers.Helpers.DataFilePath("mooh_50.txt");
            TextFile moohTxt = new TextFile(mooh50);
            TransactionSet transactions = new TransactionSet(moohTxt, moohSetParams);

            Assert.That(transactions.Count, Is.EqualTo(50), "Количество загруженных транзакций должно быть 50");

            var rawRows = File.ReadAllLines(mooh50);

            foreach (var tr in transactions) 
            {
                Assert.That(tr.Count, Is.Positive, $"У транзакции {tr.Id} должен быть хотя бы один элемент");
            }

            // Спот-чек длины: число полей после пропуска колонок и «?»
            foreach (string id in new[] { "0", "1", "49" })
            {
                int rowIndex = int.Parse(id);
                int expectedCount = ExpectedItemCount(rawRows[rowIndex], moohSetParams);

                Assert.That(transactions[id].Count, Is.EqualTo(expectedCount),
                    $"Число элементов транзакции {id} должно быть {expectedCount}");
            }
        }

        /// <summary>
        /// Ожидаемое число элементов транзакции: поля вне SkippedCols и не из NullValues.
        /// </summary>
        private static int ExpectedItemCount(string row, TransactionSetParams setParams)
        {
            string[] items = row.Split(setParams.Delimiter);
            int count = 0;

            for (int i = 0; i < items.Length; i++)
            {
                if (!setParams.SkippedCols.Contains(i) && !setParams.NullValues.Contains(items[i]))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
