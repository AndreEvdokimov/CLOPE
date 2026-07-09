namespace CLOPE.Core;

internal class TransactionClusterMap
{
    private Dictionary<string, int> Rows { get; }

    internal TransactionClusterMap()
    {
        this.Rows = new Dictionary<string, int>();
    }

    internal TransactionClusterMap(int capacity)
    {
        this.Rows = new Dictionary<string, int>(capacity);
    }

    internal void Add(string transactionId, int clusterId)
    {
        this.Rows.Add(transactionId, clusterId);
    }

    internal bool TryGetClusterIdFor(string transactionId, out int clusterId)
    {
        return this.Rows.TryGetValue(transactionId, out clusterId);
    }

    internal void SetValue(string transactionId, int clusterId)
    {
        if (this.Rows.ContainsKey(transactionId))
        {
            this.Rows[transactionId] = clusterId;
        }
        else
        {
            this.Add(transactionId, clusterId);
        }
    }

    internal int RowsCount()
    {
        return this.Rows.Count;
    }

    public IEnumerator<KeyValuePair<string, int>> GetEnumerator() => this.Rows.GetEnumerator();
}