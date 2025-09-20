using System.Diagnostics;

/// <summary>
/// Алгоритм CLOPE
/// </summary>
internal class Clope
{
    /// <summary>
    /// Движок алгоритма
    /// </summary>
    protected class ClopeEngine
    {
        /// <summary>
        /// Кластер
        /// </summary>
        internal class Cluster
        {
            /// <summary>
            /// Количество транзакций в кластере
            /// </summary>
            private int n;
            /// <summary>
            /// Количество элементов транзакций, которое содержит кластер (S)
            /// </summary>
            private int s;
            /// <summary>
            /// Словарь для подсчёта множества уникальных объектов
            /// </summary>
            private Dictionary<object, int> d;
            /// <summary>
            /// Количество транзакций в кластере
            /// </summary>
            internal int N => this.n;
            /// <summary>
            /// Количество уникальных значений кластера
            /// </summary>
            internal int W => this.d.Count;
            /// <summary>
            /// Количество элементов транзакций, которое содержит кластер (S)
            /// </summary>
            internal int S => this.s;

            internal Cluster()
            {
                this.d = new Dictionary<object, int>();
                this.n = 0;
                this.s = 0;
            }

            /// <summary>
            /// Возвращает число вхождений объекта транзакции в кластер
            /// </summary>
            /// <param name="item">Элемент транзакции</param>
            /// <returns>Число вхождений объекта транзакции в кластер</returns>
            internal int Occ(object item)
            {
                return this.d.GetValueOrDefault(item, 0);
            }

            /// <summary>
            /// Добавляет транзакцию в кластер
            /// </summary>
            /// <param name="transaction">Транзакция</param>
            internal void AddTransaction(in ITransaction transaction)
            {
                for (int i = 0; i < transaction.Count; i++)
                {
                    if (this.d.ContainsKey(transaction[i]))
                    {
                        this.d[transaction[i]]++;
                        this.s++;
                    }
                    else
                    {
                        this.d.Add(transaction[i], 1);
                        this.s++;
                    }
                }

                this.n++;
            }

            /// <summary>
            /// Удаляет транзакцию из кластера
            /// </summary>
            /// <param name="transaction">Транзакция</param>
            internal void RemoveTransaction(in ITransaction transaction)
            {
                for (int i = 0; i < transaction.Count; i++)
                {
                    if (this.d.ContainsKey(transaction[i]))
                    {
                        this.d[transaction[i]]--;
                        this.s--;

                        if (this.d[transaction[i]] == 0)
                        {
                            this.d.Remove(transaction[i]);
                        }
                    }
                }

                this.n--;
            }
        }

        /// <summary>
        /// Коэффициент отталкивания
        /// </summary>
        private readonly double repulsion;
        /// <summary>
        /// Список кластеров
        /// </summary>
        private readonly List<Cluster> clusters;
        /// <summary>
        /// Словарь [id транзакции : id кластера]
        /// </summary>
        private Dictionary<object, int> clustersMap;
        /// <summary>
        /// Список кластеров
        /// </summary>
        internal List<Cluster> CLusters => this.clusters;
        /// <summary>
        /// Словарь [id транзакции : id кластера]
        /// </summary>
        internal Dictionary<object, int> ClustersMap => this.clustersMap;

        internal ClopeEngine(in double repulsion, in TransactionData transactions)
        {
            if (repulsion < 1.00)
            {
                throw new Exception($"Для репульсии допустимо значение больше 1.0. Передано значение: ${repulsion}");
            }

            this.repulsion = repulsion;
            this.clusters = new List<Cluster>() { new Cluster() };
            this.clustersMap = new Dictionary<object, int>();

            this.Run(transactions);
        }

        /// <summary>
        /// Определяет стоимость добавления транзакции в кластер
        /// </summary>
        /// <param name="cluster">Кластер</param>
        /// <param name="transaction">Транзакция</param>
        /// <returns>Стоимость добавления транзакции в кластер</returns>
        private double DeltaAdd(in Cluster cluster, in ITransaction transaction)
        {
            double result;

            int newS = cluster.S + transaction.Count;
            int newW = cluster.W;

            for (int i = 0; i < transaction.Count; i++)
            {
                if (cluster.Occ(transaction[i]) == 0)
                {
                    newW++;
                }
            }

            if (cluster.N == 0)
            {
                result = newS / Math.Pow(newW, this.repulsion);
            }
            else
            {
                result = (newS * (cluster.N + 1) / Math.Pow(newW, this.repulsion)) - (cluster.S * cluster.N) / Math.Pow(cluster.W, this.repulsion);
            }

            Debug.Assert(!double.IsNaN(result));
            Debug.Assert(!double.IsInfinity(result));

            // Значение нужно округлить в меньшеую сторону до двух разрядов, иначе можно попасть в бесконечный цикл,
            // в котором одна транзакция будет метаться между несколькими кластерами с ценой перемещения 0,07454...
            return Math.Round(result, 2);
        }

        /// <summary>
        /// Определяет стоимость удаления транзакции из кластера
        /// </summary>
        /// <param name="cluster">Кластер</param>
        /// <param name="transaction">Транзакция</param>
        /// <returns>Стоимость удаления транзакции из кластера</returns>
        private double DeltaRemove(in Cluster cluster, in ITransaction transaction)
        {
            double result;

            int newS = cluster.S - transaction.Count;
            int newW = cluster.W;

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
                result = (newS * (cluster.N - 1) / Math.Pow(newW, this.repulsion)) - (cluster.S * cluster.N) / Math.Pow(cluster.W, this.repulsion);
            }

            Debug.Assert(!double.IsNaN(result));
            Debug.Assert(!double.IsInfinity(result));

            // Значение нужно округлить в меньшеую сторону до двух разрядов, иначе можно попасть в бесконечный цикл,
            // в котором одна транзакция будет метаться между несколькими кластерами с ценой перемещения 0,007454...
            return Math.Round(result, 2);
        }

        /// <summary>
        /// Выполнение алгоритма
        /// </summary>
        /// <param name="transactions">Транзакция</param>
        private void Run(in TransactionData transactions)
        {
            foreach (ITransaction transaction in transactions.GetTransaction()) // init
            {
                int maxProfitClusterId = -1;
                double maxCostAdd = 0.00;

                for (int j = 0; j < this.clusters.Count; j++)
                {
                    double costAdd = DeltaAdd(this.clusters[j], transaction);

                    if (costAdd > maxCostAdd)
                    {
                        maxCostAdd = costAdd;
                        maxProfitClusterId = j;
                    }
                }

                if (maxProfitClusterId == -1) // значит для текущей транзакции не нашли кластер, который бы увеличивал Profit
                {
                    continue;
                }

                if (this.clusters[maxProfitClusterId].N == 0)
                {
                    this.clusters.Add(new Cluster());
                }

                this.clusters[maxProfitClusterId].AddTransaction(transaction);
                this.clustersMap.Add(transaction.Id, maxProfitClusterId);
            }

            bool moved;

            do // iter
            {
                moved = false;

                foreach (ITransaction transaction in transactions.GetTransaction())
                {
                    int maxProfitClusterId = this.clustersMap[transaction.Id];
                    double removeCost = this.DeltaRemove(this.clusters[this.clustersMap[transaction.Id]], transaction);
                    double maxMoveCost = 0.00;

                    for (int j = 0; j < this.clusters.Count; j++)
                    {
                        double currentMoveCost = this.DeltaAdd(this.clusters[j], transaction) + removeCost;

                        if (currentMoveCost > maxMoveCost)
                        {
                            maxMoveCost = currentMoveCost;
                            maxProfitClusterId = j;
                        }
                    }

                    if (this.clustersMap[transaction.Id] != maxProfitClusterId)
                    {
                        this.clusters[this.clustersMap[transaction.Id]].RemoveTransaction(transaction);
                        this.clusters[maxProfitClusterId].AddTransaction(transaction);
                        this.clustersMap[transaction.Id] = maxProfitClusterId; // за транзакцией закрепляем кластер j

                        moved = true;
                    }
                }
            } while (moved);

            for (int i = 0; i < this.clusters.Count; i++)
            {
                if (this.clusters[i].N == 0)
                {
                    this.clusters.Remove(this.clusters[i]);
                }
            }
        }
    }

    /// <summary>
    /// Таблица 1: Id транзакции, 2: Id кластера
    /// </summary>
    internal DataSet OutputTable { get; }
    /// <summary>
    /// Таблица c характеристиками кластеров
    /// </summary>
    internal DataSet ClusterCharacteristicsTable { get; }

    internal Clope(in double repulsion, in TransactionData transactions)
    {
        ClopeEngine engine = new(repulsion, transactions);

        OutputTable = new(2);
        this.ClusterCharacteristicsTable = new(4);

        this.PrepareClusterCharacteristicsTable(engine.CLusters);
        this.PrepareOutputTable(engine.ClustersMap);
    }

    /// <summary>
    /// Подготавливает таблицу с характеристиками кластеров
    /// </summary>
    private void PrepareClusterCharacteristicsTable(List<ClopeEngine.Cluster> clusters)
    {
        //int clusterCount = 0;
        //int n = 0;
        //int w = 0;
        //int s = 0;

        this.ClusterCharacteristicsTable.AddColumn("Номер кластера");
        this.ClusterCharacteristicsTable.AddColumn("N");
        this.ClusterCharacteristicsTable.AddColumn("W");
        this.ClusterCharacteristicsTable.AddColumn("S");

        for (int i = 0; i < clusters.Count; i++)
        {

            //clusterCount++;
            //n += this.clusters[i].N;
            //w += this.clusters[i].W;
            //s += this.clusters[i].S;

            this.ClusterCharacteristicsTable[0].AddValue(i);
            this.ClusterCharacteristicsTable[1].AddValue(clusters[i].N);
            this.ClusterCharacteristicsTable[2].AddValue(clusters[i].W);
            this.ClusterCharacteristicsTable[3].AddValue(clusters[i].S);

            //string str = $"Кластер № {i + 1}; N: {this.clusters[i].N}; W: {this.clusters[i].W}; S: {this.clusters[i].S}";
            //Console.WriteLine(str); // для отладки
        }

        //Console.WriteLine($"Всего Кластеров: {clusterCount}; N: {n}; W: {w}; S {s}"); // для отладки
    }

    /// <summary>
    /// Подготавливает таблице с результатами
    /// </summary>
    private void PrepareOutputTable(Dictionary<object, int> clustersMap)
    {
        this.OutputTable.AddColumn("Номер транзакции");
        this.OutputTable.AddColumn("Кластер");

        this.OutputTable[0].Data = [.. clustersMap.Keys];
        this.OutputTable[1].Data = [.. clustersMap.Values];
    }

}
