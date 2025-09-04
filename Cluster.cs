namespace CLOPE
{
    internal class Cluster
    {
        // Количество транзакций в кластере
        private int N;

        // Количество элементов транзакций, которое содержит кластер (S)
        private int S;

        // Словарь для подсчёта множества уникальных объектов
        private Dictionary<int, int> D = new();

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

}
