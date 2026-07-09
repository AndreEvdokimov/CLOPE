namespace CLOPE.Clusters;

/// <summary>
/// Набор кластеров
/// </summary>
internal class ClusterSet
{
    /// <summary>
    /// Список кластеров
    /// </summary>
    private Dictionary<int, Cluster> Items;
    /// <summary>
    /// Количество кластеров в наборе
    /// </summary>
    internal int Count => this.Items.Count;
    /// <summary>
    /// Счетчик кластеров
    /// </summary>
    private int clustersCount;

    internal ClusterSet()
    {
        this.clustersCount = 0;
        this.Items = new Dictionary<int, Cluster>() { [0] = new Cluster(0) };
    }

    internal bool TryGet(int index, out Cluster cluster)
    {
        return this.Items.TryGetValue(index, out cluster!);
    }

    /// <summary>
    /// Добавляет в набор новый пустой кластер
    /// </summary>
    internal void AddCluster()
    {
        clustersCount++;
        this.Items.Add(clustersCount, new Cluster(clustersCount));
    }

    /// <summary>
    /// Удаляет пустые кластеры из набора
    /// </summary>
    internal void DeleteEmptyClusters()
    {
        foreach (int id in this.Items.Keys.ToList())
        {
            if (this.Items[id].N == 0) { this.Items.Remove(id); }
        }
    }

    public IEnumerator<Cluster> GetEnumerator() => this.Items.Values.GetEnumerator();
}