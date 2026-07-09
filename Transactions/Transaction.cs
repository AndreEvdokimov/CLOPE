namespace CLOPE.Transactions
{
    /// <summary>
    /// Транзакция
    /// </summary>
    internal class Transaction
    {
        /// <summary>
        /// Индекс транзакции
        /// </summary>
        internal string Id { get; }
        /// <summary>
        /// Элементы транзакции
        /// </summary>
        private List<int> Items { get; }
        /// <summary>
        /// Количество элементов транзакции
        /// </summary>
        internal int Count => Items.Count;

        internal Transaction(string Id)
        {
            this.Id = Id;
            this.Items = new List<int>();
        }

        /// <summary>
        /// Добавляет элемент в транзакцию
        /// </summary>
        /// <param name="item"></param>
        internal void Add(int item)
        {
            this.Items.Add(item);
        }

        public IEnumerator<int> GetEnumerator() => this.Items.GetEnumerator();
    }
}
