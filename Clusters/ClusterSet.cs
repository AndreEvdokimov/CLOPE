namespace CLOPE.Clusters;

/// <summary>
/// Набор кластеров
/// </summary>
internal class ClusterSet
{
    /// <summary>
    /// Список кластеров
    /// </summary>
    private List<Cluster> ClusterList { get; set; }
    /// <summary>
    /// Количество кластеров в наборе
    /// </summary>
    internal int Count => this.ClusterList.Count;
    /// <summary>
    /// Счетчик кластеров
    /// </summary>
    private int clustersCount;
    /// <summary>
    /// Индексатор
    /// </summary>
    /// <param name="index">Индекс</param>
    /// <returns>Кластер</returns>
    internal Cluster this[int index] => this.ClusterList[index];

    internal ClusterSet()
    {
        this.clustersCount = 0;
        this.ClusterList = new List<Cluster>() { new Cluster(0) };
    }

    /// <summary>
    /// Добавляет в набор новый пустой кластер и возвращает его id
    /// </summary>
    internal int AddCluster()
    {
        clustersCount++;
        this.ClusterList.Add(new Cluster(clustersCount));
        return clustersCount;
    }

    internal void DeleteEmptyClusters()
    {
        for (int i = this.ClusterList.Count - 1; i >= 0; i--)
        {
            if (this.ClusterList[i].N == 0)
            {
                this.ClusterList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Выводит в консоль характеристики кластеров
    /// </summary>
    internal void PrintClustersCharacteristicsTable()
    {
        if (this.Count == 0)
        {
            Console.WriteLine("Набор кластеров пуст");
            return;
        }

        Console.WriteLine(String.Format("|{0,10}|{1,10}|{2,10}|{3,10}|", "Кластер", "N", "W", "S"));

        Console.WriteLine();

        for (int i = 0; i < this.ClusterList.Count; i++)
        {
            Console.WriteLine(String.Format("|{0,10}|{1,10}|{2,10}|{3,10}|", i, this.ClusterList[i].N, this.ClusterList[i].W, this.ClusterList[i].S));
        }

        Console.WriteLine();
    }

    public IEnumerator<Cluster> GetEnumerator() => this.ClusterList.GetEnumerator();
}