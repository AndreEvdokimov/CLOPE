using System.Diagnostics;

namespace CLOPE
{
    internal class Clope
    {
        protected class Cluster
        {
            // Количество транзакций в кластере
            protected int N;
            // Количество элементов транзакций, которое содержит кластер (S)
            protected int S;
            // Словарь для подсчёта множества уникальных объектов
            protected Dictionary<int, int> D = new();

            // Возвращает количество транзакций кластере
            internal int GetN()
            {
                return N;
            }

            // Возвращает ширину кластера (длина). Т.е. количество уникальных элементов кластера
            internal int GetW()
            {
                return this.D.Count;
            }

            // Количество всех элементов кластера
            internal int GetS()
            {
                return this.S;
            }

            // Конструктор, создающий новый кластер
            internal Cluster()
            {
                this.N = 0;
                this.S = 0;
            }

            // Число вхождений объекта i в кластер
            internal int Occ(int item)
            {
                this.D.TryGetValue(item, out int res);

                return res;
            }

            // Добавляем элементы транзакции в коллекцию D (уникальные значения)
            internal void AddTransaction(in List<int> transaction)
            {
                for (int i = 0; i < transaction.Count; i++)
                {
                    int item = transaction[i];

                    // Если элемент транзакции уже содержится в кластере, увеличим количество вхождений
                    if (this.D.ContainsKey(item))
                    {
                        this.D[item]++;
                        this.S++;
                    }
                    else
                    {
                        this.D.Add(item, 1);
                        // Увеличиваем общее количество элементов кластера
                        this.S++;
                    }
                }

                // Увеличиваем количество транзакций в кластере
                this.N++;
            }

            internal void RemoveTransaction(in List<int> transaction)
            {
                for (int i = 0; i < transaction.Count; i++)
                {
                    int item = transaction[i];

                    // Если элемент транзакции содержится в кластере, уменьшаем количество
                    if (this.D.ContainsKey(item))
                    {
                        this.D[item]--;
                        // Обновляем количество элементов кластера
                        this.S--;

                        if (this.D[item] == 0)
                        {
                            this.D.Remove(item);
                        }
                    }
                }

                // Уменьшаем общее количество транзакций в кластере
                this.N--;
            }
        }

        // Коэффициент отталкивания
        private readonly double Repulsion;
        // Массив кластеров
        private List<Cluster> Clusters = new() { new Cluster() };
        // Индексы кластеров (i - это индекс транзакции)
        private List<int> ClusterIndexes;

        internal Clope(in double repulsion, in List<List<int>> transactions)
        {
            Debug.Assert(repulsion > 1.00);
            
            this.Repulsion = repulsion;
            
            this.ClusterIndexes = new(new int[transactions.Count]);

            this.Init(transactions);

            this.Iteration(transactions);
        }

        private void Init(in List<List<int>> transactions)
        {
            for (int i = 0; i < transactions.Count; i++)
            {
                // id кластера с максимальной ценой добавления
                int maxProfitClusterId = -1;
                // Стоимость максимального добавления
                double maxCostAdd = 0.00;

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

        protected double DeltaAdd(in Cluster cluster, in List<int> transaction)
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

        protected double DeltaRemove(in Cluster cluster, in List<int> transaction)
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

        // Выводит в консоль параметры кластеров (хелпер для удобства проверки результатов)
        internal void PrintClustersParams() 
        {
            int n = 0;
            int w = 0;
            int s = 0;

            for (int i = 0; i < this.Clusters.Count; i++)
            {
                n += this.Clusters[i].GetN();
                w += this.Clusters[i].GetW();
                s += this.Clusters[i].GetS();
                string str = $"Кластер № {i + 1}; N: {this.Clusters[i].GetN()}; W: {this.Clusters[i].GetW()}; S: {this.Clusters[i].GetS()}";
                Console.WriteLine(str);
            }

            Console.WriteLine($"Всего N: {n}; W: {w}; S {s}");
        }
    }
}
