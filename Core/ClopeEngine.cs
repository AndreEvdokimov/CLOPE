using CLOPE.Transactions;
using CLOPE.Clusters;
using System.Diagnostics;

namespace CLOPE.Core;

/// <summary>
/// Движок алгоритма CLOPE
/// </summary>
internal class ClopeEngine
{   
    /// <summary>
    /// Таблица с назначениями [id транзакции : id кластера]
    /// </summary>
    private TransactionClusterMap Assignments = new TransactionClusterMap();

    /// <summary>
    /// Запускает алгоритм
    /// </summary>
    /// <param name="transactionSet">Набор транзакций</param>
    /// <param name="clusterSet">Кластеры</param>
    /// <param name="repulsion">Коэф. отталкивания</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    internal TransactionClusterMap Run(in TransactionSet transactionSet, in ClusterSet clusterSet, in double repulsion)
    {
        this.Assignments = new TransactionClusterMap(transactionSet.Count);

        if (transactionSet.Count == 0)
        {
            throw new ArgumentException("Набор транзакций пуст. Проверьте параметры набора транзакций (разделитель, и т.д.)");
        }

        if (repulsion <= 1.0)
        {
            throw new ArgumentException($"Значение репульсии должно быть больше 1. Передано значение ${repulsion}");
        }

        this.Init(transactionSet, clusterSet, repulsion);
        this.Iter(transactionSet, clusterSet, repulsion);

        return this.Assignments;
    }

    /// <summary>
    /// Считает стоимость добавления транзакции
    /// </summary>
    /// <param name="transaction">Транзакция</param>
    /// <param name="clusters">Кластер</param>
    /// <param name="repulsion">Коэф. отталкивания</param>
    private double DeltaAdd(in Transaction transaction, Cluster cluster, double repulsion)
    {
        double result;

        int newS = cluster.S + transaction.Count;
        int newW = cluster.W;

        foreach (var item in transaction)
        {
            if (cluster.Occ(item) == 0)
            {
                newW++;
            }
        }

        if (cluster.N == 0) // Если в кластере не останется элементов
        {
            result = newS / Math.Pow(newW, repulsion);
        }
        else
        {
            result = (newS * (cluster.N + 1) / Math.Pow(newW, repulsion)) - (cluster.S * cluster.N) / Math.Pow(cluster.W, repulsion);
        }

        Debug.Assert(!double.IsNaN(result));
        Debug.Assert(!double.IsInfinity(result));

        return result;
    }

    /// <summary>
    /// Считает стоимость удаления транзакции
    /// </summary>
    /// <param name="transaction">Транзакция</param>
    /// <param name="cluster">Кластер</param>
    /// <param name="repulsion">Коэф. отталкивания</param>
    private double DeltaRemove(in Transaction transaction, Cluster cluster, double repulsion)
    {
        double result;

        if (cluster.N == 0)
        {
            return 0;
        }

        int newS = cluster.S - transaction.Count;
        int newN = cluster.N - 1;
        int newW = cluster.W;

        foreach (var item in transaction)
        {
            if (cluster.Occ(item) == 1)
            {
                newW--;
            }
        }

        if (newN == 0 || newW == 0)
        {
            result = (cluster.S * cluster.N) / Math.Pow(cluster.W, repulsion);
        }
        else
        {
            result = ((newS * newN) / Math.Pow(newW, repulsion)) - (cluster.S * cluster.N) / Math.Pow(cluster.W, repulsion);
        }

        Debug.Assert(!double.IsNaN(result));
        Debug.Assert(!double.IsInfinity(result));

        return result;
    }

    /// <summary>
    /// Запускает фазу инициализации
    /// </summary>
    /// <param name="transactionSet">Набор транзакций</param>
    /// <param name="clusterSet">Кластеры</param>
    /// <param name="repulsion">Коэф. отталкивания</param>
    private void Init(in TransactionSet transactionSet, in ClusterSet clusterSet, in double repulsion)
    {
        foreach (var transaction in transactionSet) // init
        {
            double maxMoveCost = 0;
            int bestClusterId = -1;

            foreach (var cluster in clusterSet)
            {
                double costAdd = this.DeltaAdd(transaction, cluster, repulsion);

                if (costAdd > maxMoveCost)
                {
                    maxMoveCost = costAdd;
                    bestClusterId = cluster.Id;
                }
            }

            clusterSet.TryGet(bestClusterId, out Cluster bestCluster);

            if (bestCluster.N == 0) // Если лучший кластер это пустой кластер, то добавим новый пустой кластер
            {
                clusterSet.AddCluster();
            }

            bestCluster.AddTransaction(transaction);

            this.Assignments.SetValue(transaction.Id, bestClusterId);
        }
    }

    /// <summary>
    /// Запускает фазу итераций
    /// </summary>
    /// <param name="transactionSet">Набор транзакций</param>
    /// <param name="clusterSet">Кластеры</param>
    /// <param name="repulsion">Коэф. отталкивания</param>
    private void Iter(in TransactionSet transactionSet, in ClusterSet clusterSet, in double repulsion)
    {
        bool moved;

        do // iter
        {
            moved = false;

            foreach (var transaction in transactionSet)
            {
                this.Assignments.TryGetClusterIdFor(transaction.Id, out int currentClusterId);

                Debug.Assert(currentClusterId != -1);

                clusterSet.TryGet(currentClusterId, out Cluster currentCluster);

                double maxMoveCost = 0;
                double remCost = this.DeltaRemove(transaction, currentCluster, repulsion);
                int bestClusterId = currentClusterId;

                foreach (var cluster in clusterSet)
                {
                    if (cluster.Id == currentClusterId)
                    {
                        continue;
                    }

                    double moveCost = this.DeltaAdd(transaction, cluster, repulsion) + remCost;

                    if (moveCost > maxMoveCost)
                    {
                        maxMoveCost = moveCost;
                        bestClusterId = cluster.Id;
                    }
                }

                if (maxMoveCost > 0)
                {
                    clusterSet.TryGet(bestClusterId, out Cluster bestCluster);

                    if (bestCluster.N == 0)
                    {
                        clusterSet.AddCluster();
                    }

                    currentCluster.RemoveTransaction(transaction);
                    bestCluster.AddTransaction(transaction);

                    this.Assignments.SetValue(transaction.Id, bestClusterId);
                    moved = true;
                }
            }
        } while (moved);

        clusterSet.DeleteEmptyClusters();
    }
}
