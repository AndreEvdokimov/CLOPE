using System.Diagnostics;

namespace CLOPE
{
    internal class Clope
    {
        // Коэффициент отталкивания
        private readonly double Repulsion;
        // Массив кластеров
        private List<Cluster> Clusters = new() { new Cluster() };
        // Индексы кластеров (i - это индекс транзакции)
        private List<int> ClusterIndexes;
        
        internal List<Cluster> GetClusters()
        {
            return this.Clusters;
        }

        internal Clope(in double repulsion, in List<List<int>> transactions)
        {
            Debug.Assert(repulsion > 1.00);
            
            this.Repulsion = repulsion;
            
            this.ClusterIndexes = new(new int[transactions.Count]);

            this.Init(transactions);

            this.Iteration(transactions);
        }

        private double DeltaAdd(in Cluster cluster, in List<int> transaction)
        {
            double result;

            int newS = cluster.GetS() + transaction.Count;
            int newW = cluster.GetW();

            for (int i = 0; i < transaction.Count(); i++)
            {
                if (cluster.Occ(transaction[i]) == 0)
                {
                    newW++;
                }
            }

            if (cluster.GetN() == 0)
            {
                result = newS / Math.Pow(newW, this.Repulsion);
            }
            else
            {
                result = (newS * (cluster.GetN() + 1) / Math.Pow(newW, this.Repulsion)) - (cluster.GetS() * cluster.GetN()) / Math.Pow(cluster.GetW(), this.Repulsion);
            }

            Debug.Assert(!double.IsNaN(result));
            Debug.Assert(!double.IsInfinity(result));

            // Значение нужно округлить в меньшеую сторону до двух разрядов, иначе можно попасть в бесконечный цикл,
            // в котором одна транзакция будет метаться между несколькими кластерами с ценой перемещения 0,07454...
            return Math.Round(result, 2);
        }

        private double DeltaRemove(in Cluster cluster, in List<int> transaction)
        {
            double result;

            int newS = cluster.GetS() - transaction.Count;
            int newW = cluster.GetW();

            for (int i = 0; i < transaction.Count; i++)
            {
                if (cluster.Occ(transaction[i]) == 1)
                {
                    newW--;
                }
            }

            // Если в кластере не останется элементов
            if (newW == 0)
            {
                result = 0.0;
            }
            else
            {
                result = (newS * (cluster.GetN() - 1) / Math.Pow(newW, this.Repulsion)) - (cluster.GetS() * cluster.GetN()) / Math.Pow(cluster.GetW(), this.Repulsion);
            }

            Debug.Assert(!double.IsNaN(result));
            Debug.Assert(!double.IsInfinity(result));

            // Значение нужно округлить в меньшеую сторону до двух разрядов, иначе можно попасть в бесконечный цикл,
            // в котором одна транзакция будет метаться между несколькими кластерами с ценой перемещения 0,007454...
            return Math.Round(result, 2);
        }

        private void Init(in List<List<int>> transactions)
        {
            for (int i = 0; i < transactions.Count; i++)
            {
                // id кластера с максимальной ценой добавления
                int maxProfitClusterId = -1;
                // Стоимость максимального добавления
                double maxCostAdd = 0.0;

                for (int j = 0; j < this.Clusters.Count; j++)
                {
                    // Стоимость добавления транзакции i в кластер j
                    double сostAdd = DeltaAdd(this.Clusters[j], transactions[i]);

                    if (сostAdd > maxCostAdd)
                    {
                        maxCostAdd = сostAdd;    // Максимальный DeltaAdd увеличивает Profit
                        maxProfitClusterId = j;
                    }
                }

                if (maxProfitClusterId == -1) // значит для текущей транзакции не нашли кластер, который бы увеличивал Profit
                {
                    continue;
                }

                Debug.Assert(maxProfitClusterId > -1);

                // если лучший кластер - это пустой кластер, то добавим новый пустой кластер
                if (this.Clusters[maxProfitClusterId].GetN() == 0)
                {
                    this.Clusters.Add(new Cluster());
                }

                // Добавим транзакцию в кластер с наибольшей стоимостью добавления
                this.Clusters[maxProfitClusterId].AddTransaction(transactions[i]);

                // За транзакцией запишем индекс кластера, в который она входит
                this.ClusterIndexes[i] = maxProfitClusterId;
            }
        }

        private void Iteration(in List<List<int>> transactions)
        {
            bool moved;

            do
            {
                moved = false;

                for (int i = 0; i < transactions.Count; i++)
                {
                    // Кластер с максимальной стоимостью добавления транзакции
                    int maxProfitClusterId = this.ClusterIndexes[i];
                    // Стоимость удаления
                    double removeCoast = this.DeltaRemove(this.Clusters[this.ClusterIndexes[i]], transactions[i]);
                    // Максимальная стоимость перемещения транзакции i
                    double maxMoveCost = 0.00; 
                    
                    for (int j = 0; j < this.Clusters.Count; j++)
                    {                        
                        // стоимость перемещения транзакции в кластер j
                        double currentMoveCost = this.DeltaAdd(this.Clusters[j], transactions[i]) + removeCoast;

                        // если стоимость добавления в кластер j больше, чем в кластер i, то обновим максимальную стоимость
                        if (currentMoveCost > maxMoveCost)
                        {
                            maxMoveCost = currentMoveCost;
                            maxProfitClusterId = j;
                        }
                    }

                    // если кластер j это не кластер i, который записан за транзакцией i
                    if (this.ClusterIndexes[i] != maxProfitClusterId)
                    {
                        this.Clusters[this.ClusterIndexes[i]].RemoveTransaction(transactions[i]);
                        this.Clusters[maxProfitClusterId].AddTransaction(transactions[i]);

                        // за транзакцией i закрепляем кластер j
                        this.ClusterIndexes[i] = maxProfitClusterId;

                        moved = true;
                    }
                }
            } while (moved);

            // Удаляем пустые кластеры
            for (int i = 0; i < this.Clusters.Count; i++)
            {
                if (this.Clusters[i].GetN() == 0)
                {
                    this.Clusters.Remove(this.Clusters[i]);
                }
            }
        }
    }
}
