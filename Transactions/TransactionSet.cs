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
    internal string NullValue { get; init; } = "?";
    /// <summary>
    /// Индекс поля, содержащего индексы транзакций
    /// </summary>
    internal int ColIds { get; init; } = 0;
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

    internal TransactionSet(TextFile textFile, TransactionSetParams dataSetParams)
    {
        this.transactions = new Dictionary<string, Transaction>();

        this.LoadTransactions(textFile, dataSetParams);
    }

    /// <summary>
    /// Загружает транзакции
    /// Метод работает в том числе с неотсортированными по индексу транзакциями и обычной таблицей.
    /// Для корректной работы набор транзакций должен содержать индексы транзакций в отдельном поле.
    /// </summary>
    /// <param name="textFile">Набор данных текстового файла</param>
    /// <param name="transactionSetParams">Параметры набора транзакций</param>
    private void LoadTransactions(TextFile textFile, TransactionSetParams transactionSetParams)
    {
        Dictionary<object, int> uniqValues = new(); // записи <уникальное значение транзакции: уникальный индекс>

        int uniqIndex = 0; // Уникальный индекс элемента транзации

        foreach (var transaction in textFile.GetRow())
        {
            var items = transaction.Split(transactionSetParams.Delimiter); // Разбиваем строку по указанному разделителю - получаем транзакцию

            //Console.WriteLine(items.Length);

            if (items.Length == 1) // Пропустим массив, состоящего из одного элемента, т.к. у транзакции должен быть индекс и минимум один элемент
            {
                continue;
            }

            string transactionId = items[transactionSetParams.ColIds];

            if (!this.transactions.ContainsKey(transactionId)) // Добавляем транзакцию по id транзакции для загрузки данных
            {
                this.transactions.Add(transactionId, new Transaction(transactionId));
            }

            for (int i = 0; i < items.Length; i++)
            {
                if (i != transactionSetParams.ColIds && items[i] != transactionSetParams.NullValue) // Пропускаем пустые значения и столбец с индексами транзакций
                {
                    if (uniqValues.TryGetValue(items[i], out int index))
                    {
                        this.transactions[transactionId].Add(index);
                    }
                    else
                    {
                        this.transactions[transactionId].Add(uniqIndex);
                        uniqValues.Add(items[i], uniqIndex);
                        uniqIndex++;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Выводит в консоль транзакции. По умолчанию выводит 50 транзакций
    /// </summary>
    /// <param name="transactions">Транзакции</param>
    internal void PrintTransactions()
    {
        if (this.transactions.Count == 0)
        {
            Console.WriteLine("Набор транзакций пуст");
            return;
        }

        Console.WriteLine(String.Format("|{0,15}|{1,15}", "ID Транзакции", "ID кластера"));

        foreach (var transaction in this.transactions.Values)
        {
            Console.WriteLine(String.Format("|{0,15}|{1,15}", transaction.Id, transaction.ClusterId));
        }

        Console.WriteLine(String.Format("|{0,15}|{1,15}", "Кол-во транзакций", transactions.Count));
    }

    public IEnumerator<Transaction> GetEnumerator() => this.transactions.Values.GetEnumerator();
}
