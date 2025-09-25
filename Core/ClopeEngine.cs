using CLOPE.Transactions;
using CLOPE.Clusters;

namespace CLOPE.Core;

/// <summary>
/// Движок алгоритма CLOPE
/// </summary>
internal static class ClopeEngine
{
    /// <summary>
    /// Выполнение алгоритма
    /// </summary>
    /// <param name="transactionsSet">Транзакции</param>
    internal static void Run(in TransactionSet transactionsSet, in ClusterSet clusters, in double repulsion)
    {
        if (transactionsSet.Count == 0)
        {
            Console.WriteLine("Набор транзакций пуст. Проверьте параметры набора транзакций (разделитель, и т.д.). Выходим из программы...");
            return;
        }

        if (repulsion <= 1.0)
        {
            Console.WriteLine($"Значение репульсии должно быть больше 1. Передано значение ${repulsion}");
            return;
        }

        IEnumerator<Transaction> transactions = transactionsSet.GetEnumerator();

        while (transactions.MoveNext()) // init
        {
            Transaction transaction = transactions.Current;

            int maxProfitClusterId = FindBestCluster(transaction, clusters, repulsion);

            clusters[maxProfitClusterId].AddTransaction(transaction);
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

                int maxProfitClusterId = FindBestCluster(transaction, clusters, repulsion);

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

    /// <summary>
    /// Возвращает для транзакции кластер, увеличивающий Profit.
    /// Если кластер не был найден, то вернет "-1".
    /// </summary>
    /// <param name="transaction">Транзакция</param>
    private static int FindBestCluster(Transaction transaction, ClusterSet clusters, double repulsion)
    {   
        double maxMoveCost = 0.00;
        int bestClusterId = transaction.ClusterId;

        foreach (Cluster cluster in clusters)
        {
            double costAdd = cluster.DeltaAdd(transaction, repulsion);

            if (costAdd > maxMoveCost)
            {
                maxMoveCost = costAdd;
                bestClusterId = cluster.Id;
            }
        }

        if (bestClusterId == -1) // Если не нашли лучший кластер, добавим его в новый (пустой) кластер
        {
            bestClusterId = clusters.AddCluster();
        }

        if (clusters[bestClusterId].N == 0) // Если лучший кластер это пустой кластер, то добавим новый кластер
        {
            clusters.AddCluster();
        }

        return bestClusterId;
    }
}
