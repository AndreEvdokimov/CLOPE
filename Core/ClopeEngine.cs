using CLOPE.Transactions;
using CLOPE.Clusters;

namespace CLOPE.Core;

/// <summary>
/// Движок алгоритма
/// </summary>
internal static class ClopeEngine
{
    /// <summary>
    /// Выполнение алгоритма
    /// </summary>
    /// <param name="transactionsSet">Транзакции</param>
    internal static void Run(in TransactionSet transactionsSet, in ClusterSet clusters, in double repulsion)
    {
        if (clusters.Count == 0) { clusters.AddCluster(); }

        IEnumerator<Transaction> transactions = transactionsSet.GetEnumerator();

        while (transactions.MoveNext()) // init
        {
            Transaction transaction = transactions.Current;
            int maxProfitClusterId = -1;
            double maxCostAdd = 0.00;

            foreach (Cluster cluster in clusters)
            {
                double costAdd = cluster.DeltaAdd(transaction, repulsion);

                if (costAdd > maxCostAdd)
                {
                    maxCostAdd = costAdd;
                    maxProfitClusterId = cluster.Id;
                }
            }

            if (maxProfitClusterId == -1) // значит для текущей транзакции не нашли кластер, который бы увеличивал Profit
            {
                continue;
            }

            if (clusters[maxProfitClusterId].N == 0)
            {
                clusters.AddCluster();
            }

            clusters[maxProfitClusterId].AddTransaction(transactions.Current);
            transaction.ClusterId = maxProfitClusterId;
        }

        bool moved;

        do // iter
        {
            transactions.Reset();
            
            moved = false;

            while (transactions.MoveNext())
            {
                Transaction transaction = transactions.Current;
                int maxProfitClusterId = transaction.ClusterId;
                double removeCost = clusters[maxProfitClusterId].DeltaRemove(transaction, repulsion);
                double maxMoveCost = 0.00;

                foreach (Cluster cluster in clusters)
                {
                    double costAdd = cluster.DeltaAdd(transaction, repulsion) + removeCost;

                    if (costAdd > maxMoveCost)
                    {
                        maxMoveCost = costAdd;
                        maxProfitClusterId = cluster.Id;
                    }
                }

                if (transaction.ClusterId != maxProfitClusterId)
                {
                    clusters[transaction.ClusterId].RemoveTransaction(transaction);
                    clusters[maxProfitClusterId].AddTransaction(transaction);
                    transaction.ClusterId = maxProfitClusterId;
                    moved = true;
                }
            }
        } while (moved);

        clusters.DeleteEmptyClusters(); // удалить пустые кластеры
    }
}
