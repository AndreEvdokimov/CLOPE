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

    internal Cluster this[int index] => this.ClusterList[index];

    internal ClusterSet()
    {
        this.ClusterList = new List<Cluster>() { };
        this.clustersCount = 0;
    }

    /// <summary>
    /// Добавляет в набор новый пустой кластер
    /// </summary>
    internal void AddCluster()
    {
        this.ClusterList.Add(new Cluster(clustersCount));
        clustersCount++;
    }

    internal void DeleteEmptyClusters()
    {
        for (int i = 0; i < this.ClusterList.Count; i++)
        {
            if (this.ClusterList[i].N == 0)
            {
                this.ClusterList.Remove(ClusterList[i]);
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

        Console.WriteLine(String.Format("|{0,7}|{1,5}|{2,5}|{3,5}|", "Кластер", "N", "W", "S"));

        Console.WriteLine();

        for (int i = 0; i < this.ClusterList.Count; i++)
        {
            Console.WriteLine(String.Format("|{0,7}|{1,5}|{2,5}|{3,5}|", i, this.ClusterList[i].N, this.ClusterList[i].W, this.ClusterList[i].S));
        }

        Console.WriteLine();
    }

    public IEnumerator<Cluster> GetEnumerator() => this.ClusterList.GetEnumerator();
}