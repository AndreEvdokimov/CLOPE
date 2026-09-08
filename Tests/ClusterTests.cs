using System.Runtime.CompilerServices;
using CLOPE.Clusters;
using NUnit;
using NUnit.Framework;

namespace CLOPE.Tests
{
    struct ExpectedClusterStats 
    {
        internal int N;
        internal int S;
        internal int W;
    }

    [TestFixture]
    public class ClusterTests
    {
        private double repulsion26 = 2.6;
        private List<Transactions.Transaction> trs = new() { new Transactions.Transaction("0"), new Transactions.Transaction("1"), new Transactions.Transaction("2") };

        [OneTimeSetUp]
        public void SetTransactions()
        {
            List<int> tr = new List<int>()  { 45, 34, 3, 36, 5, 6, 7, 44, 38, 51, 11, 12, 59, 14, 15, 16, 17, 18, 19, 43, 52 };
            List<int> tr1 = new List<int>() { 45, 46, 3, 36, 5, 6, 7, 25, 38, 51, 11, 12, 13, 14, 15, 16, 17, 18, 31, 49, 52 };
            List<int> tr2 = new List<int>() { 27, 46, 3, 36, 5, 6, 7, 42, 38, 51, 11, 12, 59, 62, 15, 16, 17, 18, 31, 49, 52 };
            
            foreach (var item in tr)
            {
                this.trs[0].Add(item);
            }

            foreach (var item in tr1)
            {
                this.trs[1].Add(item);
            }

            foreach (var item in tr2)
            {
                this.trs[2].Add(item);
            }

            foreach (var transaction in this.trs)
            {
                Assert.That(transaction.Count, Is.Positive);
            }
        }

        private void CheckClusterStats(Cluster actualCluster, ExpectedClusterStats expectedCluster) 
        {
            Assert.Multiple(new Action(() =>
            {
                Assert.That(actualCluster.N, Is.EqualTo(expectedCluster.N), $"Количество транзакций в кластере {actualCluster.N}, должно быть {expectedCluster.N}");
                Assert.That(actualCluster.S, Is.EqualTo(expectedCluster.S), $"Количество элементов транзакций в кластере {actualCluster.S}, должно быть {expectedCluster.S}");
                Assert.That(actualCluster.W, Is.EqualTo(expectedCluster.W), $"Количество **уникальных** элементов в кластере {actualCluster.W}, должно быть {expectedCluster.W}");
            }));
        }

        private static double Profit(int s, int n, int w, double r)
        {
            return s * n / Math.Pow(w, r);
        }

        [Test]
        public void AddTransaction()
        {
            Cluster cluster = new Cluster(0);

            cluster.AddTransaction(trs[0]);

            CheckClusterStats(cluster, new ExpectedClusterStats() { N = 1, S = 21, W = 21 });
        }

        [Test]
        public void AddTransactions()
        {
            Cluster cluster = new Cluster(0);

            foreach (var tr in this.trs)
            {
                cluster.AddTransaction(tr);
            }

            CheckClusterStats(cluster, new ExpectedClusterStats() { N = 3, S = 63, W = 29 });
        }

        [Test]
        public void RemoveTransaction()
        {
            Cluster cluster = new Cluster(0);

            cluster.AddTransaction(trs[0]);

            CheckClusterStats(cluster, new ExpectedClusterStats() { N = 1, S = 21, W = 21 });

            cluster.RemoveTransaction(trs[0]);

            CheckClusterStats(cluster, new ExpectedClusterStats() { N = 0, S = 0, W = 0 });
        }

        [Test]
        public void RemoveTransactions()
        {
            Cluster cluster = new Cluster(0);

            foreach (var tr in this.trs)
            {
                cluster.AddTransaction(tr);
            }

            CheckClusterStats(cluster, new ExpectedClusterStats() { N = 3, S = 63, W = 29 });

            // удаление транзакции tr
            cluster.RemoveTransaction(trs[0]);

            CheckClusterStats(cluster, new ExpectedClusterStats() { N = 2, S = 42, W = 25 });
            
            // удаление транзакции tr1
            cluster.RemoveTransaction(trs[1]);

            CheckClusterStats(cluster, new ExpectedClusterStats() { N = 1, S = 21, W = 21 });
        }

        [Test]
        public void RemoveAddRestore()
        {
            Cluster cluster = new Cluster(0);

            foreach (var tr in this.trs)
            {
                cluster.AddTransaction(tr);
            }

            int n = cluster.N;
            int s = cluster.S;
            int w = cluster.W;

            cluster.RemoveTransaction(trs[1]);
            cluster.AddTransaction(trs[1]);

            CheckClusterStats(cluster, new ExpectedClusterStats() { N = n, S = s, W = w });
        }

        [Test]
        public void DeltaAddEmptyCluster()
        {
            Cluster cluster = new Cluster(0);

            double expected = Profit(21, 1, 21, this.repulsion26);

            Assert.That(cluster.DeltaAdd(trs[0], this.repulsion26), Is.EqualTo(expected).Within(1e-9));
        }

        [Test]
        public void DeltaAddNonEmptyCluster()
        {
            Cluster cluster = new Cluster(0);

            cluster.AddTransaction(trs[0]);

            // newS=42, newW=26 (5 новых уникальных: 46, 25, 13, 31, 49)
            double expected = Profit(42, 2, 26, this.repulsion26) - Profit(21, 1, 21, this.repulsion26);

            Assert.That(cluster.DeltaAdd(trs[1], this.repulsion26), Is.EqualTo(expected).Within(1e-9));
        }

        [Test]
        public void DeltaRemoveEmptyCluster()
        {
            Cluster cluster = new Cluster(0);

            Assert.That(cluster.DeltaRemove(trs[0], this.repulsion26), Is.EqualTo(0).Within(1e-9));
        }

        [Test]
        public void DeltaRemoveLastTransaction()
        {
            Cluster cluster = new Cluster(0);

            cluster.AddTransaction(trs[0]);

            double expected = Profit(21, 1, 21, this.repulsion26);

            Assert.That(cluster.DeltaRemove(trs[0], this.repulsion26), Is.EqualTo(expected).Within(1e-9));
        }

        [Test]
        public void DeltaRemoveFromClusterWithThree()
        {
            Cluster cluster = new Cluster(0);

            foreach (var tr in this.trs)
            {
                cluster.AddTransaction(tr);
            }

            // W падает на 4 уникальных только у trs[0]: 34, 44, 19, 43
            double expected = Profit(42, 2, 25, this.repulsion26) - Profit(63, 3, 29, this.repulsion26);

            Assert.That(cluster.DeltaRemove(trs[0], this.repulsion26), Is.EqualTo(expected).Within(1e-9));
        }
    }
}