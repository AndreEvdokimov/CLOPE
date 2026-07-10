using System.Diagnostics;
using CLOPE.Transactions;

namespace CLOPE.Clusters;
/// <summary>
/// Кластер
/// </summary>
internal class Cluster
{
    internal int Id { get; }
    /// <summary>
    /// Количество транзакций в кластере
    /// </summary>
    internal int N { get; private set; } = 0;
    /// <summary>
    /// Количество элементов транзакций, которое содержит кластер (S)
    /// </summary>
    internal int S { get; private set; } = 0;
    /// <summary>
    /// Количество уникальных значений кластера
    /// </summary>
    internal int W  => this.ItemCounts.Count;
    /// <summary>
    /// Словарь для подсчёта множества уникальных объектов (D)
    /// </summary>
    internal Dictionary<int, int> ItemCounts { get; }

    internal Cluster(int id)
    {
        this.Id = id;
        this.ItemCounts = new Dictionary<int, int>();
    }

    /// <summary>
    /// Возвращает число вхождений объекта транзакции в кластер
    /// </summary>
    /// <param name="item">Элемент транзакции</param>
    /// <returns>Число вхождений объекта транзакции в кластер</returns>
    internal int Occ(int item)
    {
        return this.ItemCounts.GetValueOrDefault(item, 0);
    }

    /// <summary>
    /// Добавляет транзакцию в кластер
    /// </summary>
    /// <param name="transaction">Транзакция</param>
    internal void AddTransaction(in Transaction transaction)
    {
        foreach (var item in transaction)
        {
            if (this.ItemCounts.ContainsKey(item))
            {
                this.ItemCounts[item]++;
                this.S++;
            }
            else
            {
                this.ItemCounts.Add(item, 1);
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
        foreach (var item in transaction)
        {
            if (this.ItemCounts.ContainsKey(item))
            {
                Debug.Assert(this.ItemCounts[item] != 0);

                this.ItemCounts[item]--;
                this.S--;

                if (this.ItemCounts[item] == 0)
                {
                    this.ItemCounts.Remove(item);
                }
            }
        }

        this.N--;
    }

    /// <summary>
    /// Считает стоимость добавления транзакции
    /// </summary>
    /// <param name="transaction">Транзакция</param>
    /// <param name="clusters">Кластер</param>
    /// <param name="repulsion">Коэф. отталкивания</param>
    internal double DeltaAdd(in Transaction transaction, double repulsion)
    {
        double result;

        int newS = this.S + transaction.Count;
        int newW = this.W;

        foreach (var item in transaction)
        {
            if (this.Occ(item) == 0)
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

        return result;
    }

    /// <summary>
    /// Считает стоимость удаления транзакции
    /// </summary>
    /// <param name="transaction">Транзакция</param>
    /// <param name="cluster">Кластер</param>
    /// <param name="repulsion">Коэф. отталкивания</param>
    internal double DeltaRemove(in Transaction transaction, double repulsion)
    {
        double result;

        if (this.N == 0)
        {
            return 0;
        }

        int newS = this.S - transaction.Count;
        int newN = this.N - 1;
        int newW = this.W;

        foreach (var item in transaction)
        {
            if (this.Occ(item) == 1)
            {
                newW--;
            }
        }

        if (newN == 0 || newW == 0)
        {
            result = (this.S * this.N) / Math.Pow(this.W, repulsion);
        }
        else
        {
            result = ((newS * newN) / Math.Pow(newW, repulsion)) - (this.S * this.N) / Math.Pow(this.W, repulsion);
        }

        Debug.Assert(!double.IsNaN(result));
        Debug.Assert(!double.IsInfinity(result));

        return result;
    }

    public override string ToString()
    {
        return $"ID:{this.Id}; N:{this.N}; W:{this.W}; S:{this.S};";
    }
}