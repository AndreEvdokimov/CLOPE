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
        private List<int> Items { get; set; }
        /// <summary>
        /// Индексатор
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Элемент транзакции</returns>
        internal int this[int index] => Items[index];
        /// <summary>
        /// Количество элементов транзакции
        /// </summary>
        internal int Count => Items.Count;
        /// <summary>
        /// Индекс кластера, который закреплен за транзакцией
        /// </summary>
        internal int ClusterId { get; set; }

        internal Transaction(string Id)
        {
            this.Id = Id;
            this.Items = new List<int>();
            this.ClusterId = -1;
        }

        /// <summary>
        /// Добавляет элемент в транзакцию
        /// </summary>
        /// <param name="item"></param>
        internal void Add(int item)
        {
            this.Items.Add(item);
        }
    }
}
