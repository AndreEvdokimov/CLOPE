using CLOPE.Import;

namespace CLOPE.Transactions;

/// <summary>
/// Параметры набора транзакций
/// </summary>
internal class TransactionSetParams
{
    /// <summary>
    /// Разделитель строк
    /// </summary>
    internal char Delimiter { get; init; } = ',';
    /// <summary>
    /// Пустое значение 
    /// </summary>
    internal string[] NullValues { get; init; } = { "?" };
    /// <summary>
    /// Индекс поля, содержащего индексы транзакций
    /// </summary>
    internal int ColIds { get; init; } = 0;
    /// <summary>
    /// Индексы столбцов, которые нужно пропустить
    /// </summary>
    internal int[] SkippedCols { get; init; } = { 0 };
}

/// <summary>
/// Набор транзакций
/// </summary>
internal class TransactionSet
{
    /// <summary>
    /// Транзакции: <id, транзакция>
    /// </summary>
    readonly private Dictionary<string, Transaction> transactions;
    /// <summary>
    /// Количество транзакций в наборе
    /// </summary>
    internal int Count => transactions.Count;
    /// Индексатор
    /// </summary>
    /// <param name="index">Индекс</param>
    /// <returns>Транзакция</returns>
    internal Transaction this[string id] => this.transactions[id];

    internal TransactionSet(TextFile textFile, TransactionSetParams dataSetParams)
    {
        this.transactions = new Dictionary<string, Transaction>();

        this.LoadTransactions(textFile, dataSetParams);
    }

    /// <summary>
    /// Загружает транзакции
    /// </summary>
    /// <param name="textFile">Набор данных текстового файла</param>
    /// <param name="transactionSetParams">Параметры набора транзакций</param>
    private void LoadTransactions(TextFile textFile, TransactionSetParams transactionSetParams)
    {
        var uniqValues = new List<Dictionary<string, int>>(); // записи <уникальное значение транзакции: уникальный индекс>

        var uniqIndex = 0; // Уникальный индекс элемента транзации

        foreach (var transaction in textFile.GetRow())
        {
            var items = transaction.Split(transactionSetParams.Delimiter); // Разбиваем строку по указанному разделителю - получаем транзакцию

            if (items.Length == 1) // Пропустим массив, состоящего из одного элемента (который является id транзакции), т.к. у транзакции должен быть индекс и минимум один элемент
            {
                continue;
            }

            var transactionId = items[transactionSetParams.ColIds];

            if (!this.transactions.ContainsKey(transactionId)) // Добавляем транзакцию по id транзакции для загрузки данных
            {
                this.transactions.Add(transactionId, new Transaction(transactionId));
            }

            for (int i = 0; i < items.Length; i++)
            {
                if (uniqValues.Count <= i) { uniqValues.Add(new Dictionary<string, int>()); }

                if (!transactionSetParams.SkippedCols.Contains(i) && !transactionSetParams.NullValues.Contains(items[i])) // Пропускаем пустые значения и столбец с индексами транзакций
                {
                    if (uniqValues[i].TryGetValue(items[i], out int index))
                    {
                        this.transactions[transactionId].Add(index);
                    }
                    else
                    {
                        this.transactions[transactionId].Add(uniqIndex);
                        uniqValues[i].Add(items[i], uniqIndex);
                        uniqIndex++;
                    }
                }
            }
        }
    }

    public IEnumerator<Transaction> GetEnumerator() => this.transactions.Values.GetEnumerator();
}
