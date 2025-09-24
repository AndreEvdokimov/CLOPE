namespace CLOPE.Transactions
{
    /// <summary>
    /// Транзакция
    /// </summary>
    internal class Transaction
    {
        /// <summary>
        /// 
        /// </summary>
        internal string Id { get; }
        /// <summary>
        ///
        /// </summary>
        private List<int> Items { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal int this[int index] => Items[index];
        /// <summary>
        /// 
        /// </summary>
        internal int Count => Items.Count;
        /// <summary>
        /// 
        /// </summary>
        internal int ClusterId { get; set; }

        internal Transaction(string Id, List<int>? data = null)
        {
            this.Id = Id;
            this.Items = data ?? new();
            this.ClusterId = -1;
        }

        internal void Add(int item)
        {
            this.Items.Add(item);
        }
    }
}
