using System.Text;
using CLOPE.Transactions;

namespace CLOPE.Helpers;
/// <summary>
/// Хелперы для дебага, проверки результатов работы других классов и т.п.
/// </summary>
internal class Helpers
{
    /// <summary>
    ///  Записывает построчно таблицу в файл
    /// </summary>
    /// <param name="path"></param>
    /// <param name="data"></param>
    /// <param name="encoding"></param>
    /// <exception cref="Exception"></exception>
    internal static void ExportTxt(in string path, in List<List<string>> data, in Encoding encoding)
    {
        try
        {
            using StreamWriter r = new(path, false, encoding);

            for (int i = 0; i < data.Count; i++)
            {
                r.WriteLine(data[i].ToString());
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Во время записи файла возникла ошибка. {e.Message}");
        }
    }

    /// <summary>
    /// Запрашивает в консоли путь до файла с данными и возвращает его в виде строки
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    internal static string GetFilePathFromConsole()
    {

        Console.WriteLine("Введите путь до файла с данными или укажите путь в коде (файл Programm.cs)");

        string? path = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(path)) { throw new Exception("Передано пустое значение"); }

        Console.WriteLine($"Указанный путь: '{path}'");

        return path;
    }

    /// <summary>
    /// Выводит в консоль транзакции. По умолчанию выводит 50 транзакций
    /// </summary>
    /// <param name="transactions">Транзакции</param>
    internal static void PrintTransactions(TransactionSet transactions, int count = 50)
    {
        int j = 0;

        foreach (var transaction in transactions)
        {
            j++;

            Console.Write(transaction.Id + " : ");

            for (int i = 0; i < transaction.Count; i++)
            {

                Console.Write(transaction[i].ToString() + ", ");
            }

            Console.WriteLine();

            if (j == count) { break; }
        }
    }
}