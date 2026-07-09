using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLOPE.Tests
{
    [TestClass]
    public class ClusterTests
    {
        private List<Transactions.Transaction> trS = new() { new Transactions.Transaction("0"), new Transactions.Transaction("1"), new Transactions.Transaction("2") };
        private List<int> tr = new List<int>()  { 45, 34, 3, 36, 5, 6, 7, 44, 38, 51, 11, 12, 59, 14, 15, 16, 17, 18, 19, 43, 52 };
        private List<int> tr1 = new List<int>() { 45, 46, 3, 36, 5, 6, 7, 25, 38, 51, 11, 12, 13, 14, 15, 16, 17, 18, 31, 49, 52 };
        private List<int> tr2 = new List<int>() { 27, 46, 3, 36, 5, 6, 7, 42, 38, 51, 11, 12, 59, 62, 15, 16, 17, 18, 31, 49, 52 };

        public ClusterTests()
        {
            foreach (var item in tr)
            {
                this.trS[0].Add(item);
            }

            foreach (var item in tr1)
            {
                this.trS[1].Add(item);
            }

            foreach (var item in tr2)
            {
                this.trS[2].Add(item);
            }
        }

        [TestMethod]
        public void AddTransaction()
        {
            Clusters.Cluster cluster = new Clusters.Cluster(0);

            cluster.AddTransaction(trS[0]);

            Assert.AreEqual(1, cluster.N, "Неправильное количество трапнзакций в кластере");
            Assert.AreEqual(21, cluster.S, "Неправильное количество элементов транзакций в кластере");
            Assert.AreEqual(21, cluster.W, "Неправильное количество **уникальных** элементов в кластере");
        }

        [TestMethod]
        public void AddTransactions()
        {
            Clusters.Cluster cluster = new Clusters.Cluster(0);

            foreach (var tr in this.trS)
            {
                cluster.AddTransaction(tr);
            }

            Assert.AreEqual(3, cluster.N, "Неправильное количество трапнзакций в кластере");
            Assert.AreEqual(63, cluster.S, "Неправильное количество элементов транзакций в кластере");
            Assert.AreEqual(29, cluster.W, "Неправильное количество **уникальных** элементов в кластере");
        }

        [TestMethod]
        public void RemoveTransaction()
        {
            Clusters.Cluster cluster = new Clusters.Cluster(0);

            cluster.AddTransaction(trS[0]);

            Assert.AreEqual(1, cluster.N, "Неправильное количество трапнзакций в кластере");
            Assert.AreEqual(21, cluster.S, "Неправильное количество элементов транзакций в кластере");
            Assert.AreEqual(21, cluster.W, "Неправильное количество **уникальных** элементов в кластере");

            cluster.RemoveTransaction(trS[0]);

            Assert.AreEqual(0, cluster.N, "Неправильное количество трапнзакций в кластере");
            Assert.AreEqual(0, cluster.S, "Неправильное количество элементов транзакций в кластере");
            Assert.AreEqual(0, cluster.W, "Неправильное количество **уникальных** элементов в кластере");
        }

        [TestMethod]
        public void RemoveTransactions()
        {
            Clusters.Cluster cluster = new Clusters.Cluster(0);

            foreach (var tr in this.trS)
            {
                cluster.AddTransaction(tr);
            }

            Assert.AreEqual(3, cluster.N, "Неправильное количество трапнзакций в кластере");
            Assert.AreEqual(63, cluster.S, "Неправильное количество элементов транзакций в кластере");
            Assert.AreEqual(29, cluster.W, "Неправильное количество **уникальных** элементов в кластере");

            // удаление транзакции tr
            cluster.RemoveTransaction(trS[0]);

            Assert.AreEqual(2, cluster.N, "Неправильное количество трапнзакций в кластере");
            Assert.AreEqual(42, cluster.S, "Неправильное количество элементов транзакций в кластере");
            Assert.AreEqual(25, cluster.W, "Неправильное количество **уникальных** элементов в кластере");

            // удаление транзакции tr1
            cluster.RemoveTransaction(trS[1]);

            Assert.AreEqual(1, cluster.N, "Неправильное количество трапнзакций в кластере");
            Assert.AreEqual(21, cluster.S, "Неправильное количество элементов транзакций в кластере");
            Assert.AreEqual(21, cluster.W, "Неправильное количество **уникальных** элементов в кластере");
        }

        [TestMethod]
        public void RemoveAddRestore()
        {
            Clusters.Cluster cluster = new Clusters.Cluster(0);

            foreach (var tr in this.trS)
            {
                cluster.AddTransaction(tr);
            }

            int n = cluster.N;
            int s = cluster.S;
            int w = cluster.W;

            cluster.RemoveTransaction(trS[1]);
            cluster.AddTransaction(trS[1]);

            Assert.AreEqual(n, cluster.N, "N должен восстановиться после Remove + Add");
            Assert.AreEqual(s, cluster.S, "S должен восстановиться после Remove + Add");
            Assert.AreEqual(w, cluster.W, "W должен восстановиться после Remove + Add");
        }
    }
}
