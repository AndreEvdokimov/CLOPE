/// <summary>
/// Транзакция
/// </summary>
interface ITransaction
{
    /// <summary>
    /// Индекс
    /// </summary>
    object Id { get; }
    /// <summary>
    /// Список данных
    /// </summary>
    List<int> Items { get; }
    /// <summary>
    /// Количество элементов
    /// </summary>
    int Count => Items.Count;
    /// <summary>
    /// Возвращает элемент списка данных
    /// </summary>
    /// <param name="index">Индекс</param>
    /// <returns></returns>
    int this[int index] => Items[index];
}

/// <summary>
/// Набор транзакций
/// </summary>
internal class TransactionData
{
    /// <summary>
    /// Транзакция
    /// </summary>
    internal class Transaction : ITransaction
    {
        public object Id { get; }
        public List<int> Items { get; private set; }
        public int this[int index] => Items[index];
        public int Count => Items.Count;

        internal Transaction(object Id, List<int>? data)
        {
            this.Id = Id;
            this.Items = data ?? new();
        }
    }

    /// <summary>
    /// Индексы транзакций
    /// </summary>
    private List<object> indices;
    /// <summary>
    /// Элементы транзакций
    /// </summary>
    private List<int> items;

    internal TransactionData(in DataSet data, in int colIds, in int colData)
    {
        this.indices = new List<object>(data[colIds].Count);
        this.items = new List<int>(data[colData].Count);
        DataSet sorted = Sorting.SortASC(data, colIds); // При необходимости выполним сортировку по возрастанию по колонке с ID транзакций

        this.NormolizeData(sorted, colIds, colData);
    }

    /// <summary>
    /// Возвращает нормализованные данные: итоговые данные содержат индексы, которые соответствуют своим уникальным значениям входных данных
    /// </summary>
    /// <param name="data">Данные</param>
    /// <param name="colIndices">Индекс поля, содержащего индексы транзакций</param>
    /// <param name="colItems">Индекс поля, содержащего элементы транзакций</param>
    private void NormolizeData(DataSet data, in int colIndices, in int colItems)
    {
        Dictionary<object, int> uniqIndices = new Dictionary<object, int>(); // каждый словарь хранит записи <уникальное значеное: индекс>
        IColumn indexColumn = data[colIndices]; // поле с идексами транзакций
        IColumn itemColumn = data[colItems];

        int index = 0; // Уникальный индекс элемента транзации
        int offset = 0; // смещение

        while (offset < indexColumn.Count)
        {
            int stringId = 0; // номер строки транзакции
            int checkIndex = offset;

            while (checkIndex < indexColumn.Count && indexColumn[checkIndex].Equals(indexColumn[offset]))
            {
                if (!itemColumn[checkIndex].Equals("NULL")) // пропускаем пустые значения
                {
                    this.indices.Add(indexColumn[checkIndex]);

                    if (uniqIndices.TryGetValue(itemColumn[checkIndex], out int uniqIndex))
                    {
                        this.items.Add(uniqIndex);
                    }
                    else
                    {
                        uniqIndices.Add(itemColumn[checkIndex], index);
                        this.items.Add(index);
                        index++;
                    }
                }

                stringId++;
                checkIndex++;
            }

            offset += stringId; // Переходим к следующей транзакции
        }
    }

    /// <summary>
    /// Последовательно возвращает транзакции
    /// </summary>
    /// <returns>Транзакция</returns>
    internal IEnumerable<Transaction> GetTransaction() // только для отсортированных транзакций
    {
        if (this.items.Count == 0) { yield break; }

        int offset = 0; // Смещение

        while (offset < this.indices.Count)
        {
            int itemsCount = 0; // Количество элементов текущей транзакции
            int checkIndex = offset; // Индекс текущей транзации

            while (checkIndex < this.indices.Count && 
                this.indices[checkIndex].Equals(this.indices[offset]))
            {
                itemsCount++;
                checkIndex++;
            }

            yield return new Transaction(this.indices[offset], this.items.GetRange(offset, itemsCount)); // Возвращаем элементы транзакции
            offset += itemsCount; // Переход к следующей транзакции
        }
    }
}
