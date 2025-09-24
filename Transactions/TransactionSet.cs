using CLOPE.Import;
using System.Runtime.CompilerServices;
using System.Transactions;

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
    /// <summary>
    /// Количество пропускаемых строк (на случай, если набор данных содержит заголовки и т.п.)
    /// </summary>
    internal int SkippedLinesCount { get; init; } = 0;
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
    /// <param name="reader">Читатель текстовых файлов</param>
    /// <param name="transactionSetParams">Параметры набора транзакций</param>
    private void LoadTransactions(TextFile textFile, TransactionSetParams transactionSetParams)
    {
        Dictionary<object, int> uniqValues = new(); // записи <уникальное значение транзакции: уникальный индекс>

        int uniqIndex = 0; // Уникальный индекс элемента транзации
        int lineId = 0; // Счетчик строк

        foreach (var transaction in textFile.GetRow())
        {
            var items = transaction.Split(transactionSetParams.Delimiter); // Разбиваем строку по указанному разделителю

            string transactionId = items[transactionSetParams.ColIds];

            if (lineId < transactionSetParams.SkippedLinesCount && transactionSetParams.SkippedLinesCount != 0) // Пропускаем заданное количестов строк
            {
                lineId++;
                continue;
            }

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

        Console.WriteLine(String.Format("|{0,13}|{1,13}", "ID Транзакции", "ID кластера" ));

        foreach (var transaction in this.transactions.Values)
        {
            Console.WriteLine(String.Format("|{0,5}|{1,5}", transaction.Id, transaction.ClusterId));
        }
    }

    public IEnumerator<Transaction> GetEnumerator() => this.transactions.Values.GetEnumerator();
}
