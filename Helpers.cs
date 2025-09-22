using System.Diagnostics;
using System.Text;

/// <summary>
/// Хелперы для дебага, проверки результатов работы других классов и т.п.
/// </summary>
internal class Helpers 
{
     /// <summary>
    /// Выводит в консоль таблицк уникальных значений
    /// </summary>
    /// <param name="uniqValues"></param>
    internal static void PrintColumnsUniqValues(in List<Dictionary<string, int>> uniqValues)
    {
        for (int i = 0; i < uniqValues.Count; i++)
        {
            Console.WriteLine($"filed: {i}");

            for (int j = 0; j < uniqValues[i].Count; j++)
            {
                Console.WriteLine($"key: {uniqValues[i].ElementAt(j).Key}, value: {uniqValues[i].ElementAt(j).Value}");
            }
        }
    }

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
    /// Записывает в текстовый файл таблицу
    /// </summary>
    /// <param name="path"></param>
    /// <param name="data"></param>
    /// <param name="encoding"></param>
    /// <exception cref="Exception"></exception>
    internal static void ExportTxt(in string path, in List<List<int>> data, Encoding encoding)
    {
        if (!File.Exists(path))
        {
            File.Create(path);
        }

        try
        {
            StreamWriter r = new(path, false, encoding);

            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].Count; j++)
                {
                    r.Write(data[i][j] + ", ");
                }
                r.WriteLine();
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
    /// Добавляет в набор данных поле с индесами строк
    /// </summary>
    /// <param name="dataSet">Данные</param>
    internal static void AddColumnIds(DataSet dataSet)
    {
        dataSet.AddColumn("ID");

        for (int i = 0; i < dataSet[0].Count; i++)
        {
            dataSet[^1].Data.Add(i.ToString());
        }
    }

    /// <summary>
    /// Выводит в консоль транзакции. По умолчанию выводит 50 транзакций
    /// </summary>
    /// <param name="transactions">Транзакции</param>
    internal static void F(Transactions transactions, int count = 50)
    {
        int j = 0;
        
        foreach (var transaction in transactions.GetTransaction())
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