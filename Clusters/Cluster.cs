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
    internal int W => this.ItemCounts.Count;
    /// <summary>
    /// Словарь для подсчёта множества уникальных объектов (D)
    /// </summary>
    private Dictionary<int, int> ItemCounts { get; }

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

    public override string ToString()
    {
        return $"ID:{this.Id}; N:{this.N}; W:{this.W}; S:{this.S};";
    }
}