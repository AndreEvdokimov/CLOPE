using System.Diagnostics;
using CLOPE.Transactions;

namespace CLOPE.Clusters;
/// <summary>
/// Кластер
/// </summary>
internal class Cluster
{
    internal int Id { get; private set; }
    /// <summary>
    /// Количество транзакций в кластере
    /// </summary>
    internal int N { get; private set; }
    /// <summary>
    /// Количество элементов транзакций, которое содержит кластер (S)
    /// </summary>
    internal int S { get; private set; }
    /// <summary>
    /// Количество уникальных значений кластера
    /// </summary>
    internal int W => this.D.Count;
    /// <summary>
    /// Словарь для подсчёта множества уникальных объектов
    /// </summary>
    private Dictionary<int, int> D { get; }
    /// <summary>
    /// Индексы транзакций, которые содержит кластер
    /// </summary>
    internal int TransactionIds { get; set; }

    internal Cluster(int Id)
    {
        this.Id = Id;
        this.N = 0;
        this.S = 0;
        this.D = new Dictionary<int, int>();
    }

    /// <summary>
    /// Возвращает число вхождений объекта транзакции в кластер
    /// </summary>
    /// <param name="item">Элемент транзакции</param>
    /// <returns>Число вхождений объекта транзакции в кластер</returns>
    internal int Occ(int item)
    {
        return this.D.GetValueOrDefault(item, 0);
    }

    /// <summary>
    /// Добавляет транзакцию в кластер
    /// </summary>
    /// <param name="transaction">Транзакция</param>
    internal void AddTransaction(in Transaction transaction)
    {
        for (int i = 0; i < transaction.Count; i++)
        {
            if (this.D.ContainsKey(transaction[i]))
            {
                this.D[transaction[i]]++;
                this.S++;
            }
            else
            {
                this.D.Add(transaction[i], 1);
                this.S++;
            }
        }

        this.N++;
    }

    /// <summary>
    /// Удаляет транзакцию из кластера
    /// </summary>
    /// <param name="transaction">Транзакция</param>
    internal void RemoveTransaction(in Transaction transaction)
    {
        for (int i = 0; i < transaction.Count; i++)
        {
            if (this.D.ContainsKey(transaction[i]))
            {
                this.D[transaction[i]]--;
                this.S--;

                if (this.D[transaction[i]] == 0)
                {
                    this.D.Remove(transaction[i]);
                }
            }
        }

        this.N--;
    }

    /// <summary>
    /// Определяет стоимость добавления транзакции в кластер
    /// </summary>
    /// <param name="cluster">Кластер</param>
    /// <param name="transaction">Транзакция</param>
    /// <returns>Стоимость добавления транзакции в кластер</returns>
    internal double DeltaAdd(in Transaction transaction, double repulsion)
    {
        double result;

        int newS = this.S + transaction.Count;
        int newW = this.W;

        for (int i = 0; i < transaction.Count; i++)
        {
            if (this.Occ(transaction[i]) == 0)
            {
                newW++;
            }
        }

        if (this.N == 0) // Если в кластере не останется элементов
        {
            result = newS / Math.Pow(newW, repulsion);
        }
        else
        {
            result = (newS * (this.N + 1) / Math.Pow(newW, repulsion)) - (this.S * this.N) / Math.Pow(this.W, repulsion);
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
    internal double DeltaRemove(in Transaction transaction, double repulsion)
    {
        double result;

        int newS = this.S - transaction.Count;
        int newW = this.W;

        for (int i = 0; i < transaction.Count; i++)
        {
            if (this.Occ(transaction[i]) == 1)
            {
                newW--;
            }
        }

        if (newW == 0) // Если в кластере не останется элементов
        {
            result = 0.0;
        }
        else
        {
            result = (newS * (this.N - 1) / Math.Pow(newW, repulsion)) - (this.S * this.N) / Math.Pow(this.W, repulsion);
        }

        Debug.Assert(!double.IsNaN(result));
        Debug.Assert(!double.IsInfinity(result));

        // Значение нужно округлить в меньшеую сторону до двух разрядов, иначе можно попасть в бесконечный цикл,
        // в котором одна транзакция будет метаться между несколькими кластерами с ценой перемещения 0,007454...
        return Math.Round(result, 2);
    }
}