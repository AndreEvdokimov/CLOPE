using System.Diagnostics;

/// <summary>
/// Транзакция
/// </summary>
interface ITransaction
{
    /// <summary>
    /// Индекс
    /// </summary>
    string Id { get; }
    /// <summary>
    /// Элементы
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
internal class Transactions
{
    /// <summary>
    /// Транзакция
    /// </summary>
    internal class Transaction : ITransaction
    {
        public string Id { get; }
        public List<int> Items { get; private set; }
        public int this[int index] => Items[index];
        public int Count => Items.Count;

        internal Transaction(string Id, List<int>? data = null)
        {
            this.Id = Id;
            this.Items = data ?? new();
        }
    }

    /// <summary>
    /// Транзакции
    /// </summary>
    Dictionary<string, Transaction> transactions;

    internal Transactions(in DataSet data, in int colIndicesId)
    {
        this.transactions = new();

        this.NormolizeData(data, colIndicesId);
    }

    /// <summary>
    /// Нормализует данные: уникальные значения таблицы заменяются на соответствующие индексы.
    /// Метод работает с неотсортированными данными типа Транзакция и обычной таблицей.
    /// Для корректной работы данные должны содержать индексы транзакций в отдельном поле
    /// </summary>
    /// <param name="data">Данные</param>
    /// <param name="colIndicesId">Индекс поля, содержащего индексы транзакций</param>
    private void NormolizeData(DataSet data, in int colIndicesId)
    {
        Debug.Assert(data.Count != 0);
        Debug.Assert(data[0].Count != 0);

        Dictionary<object, int> uniqValues = new(); // записи <уникальное значение транзакции: уникальный индекс>

        int index = 0; // Уникальный индекс элемента транзации

        for (int i = 0; i < data[0].Count; i++) // Строки
        {
            string transactionId = data[colIndicesId][i];

            if (!transactions.ContainsKey(transactionId))
            {
                transactions.Add(transactionId, new Transaction(transactionId));
            }

            for (int j = 0; j < data.Count; j++) // Поля
            {
                if (j != colIndicesId && data[j][i] != "NULL") // Пропускаем пустые значения и столбец с индексами транзакций
                {
                    if (!uniqValues.TryGetValue(data[j][i], out int uniqIndex))
                    {
                        uniqValues.Add(data[j][i], index);
                        index++;
                    }
                    
                    this.transactions[transactionId].Items.Add(uniqIndex);
                }
            }
        }
    }

    /// <summary>
    /// Последовательно возвращает транзакции
    /// </summary>
    /// <returns>Транзакция</returns>
    internal IEnumerable<Transaction> GetTransaction()
    {
        if (this.transactions.Count == 0) { yield break; }

        foreach (var transaction in this.transactions.Values)
        {
            yield return transaction; // Возвращаем элементы транзакции
        }        
    }
}
