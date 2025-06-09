using CLOPE;
using System.Text;

internal class Helpers
{
    // Считает количество транзакций, закрепленных за кластером
    private static Dictionary<int, int> CountsNumberTransactions(in Dictionary<int, int> map)
    {
        Dictionary<int, int> keyValuePairs = new();

        List<int> keys = map.Keys.ToList();
        List<int> values = map.Values.ToList();

        for (int i = 0; i < values.Count; i++)
        {
            int count = 0;

            for (int j = 0; j < keys.Count; j++)
            {
                map.TryGetValue(keys[j], out int clusterId);

                if (clusterId == i)
                {
                    count++;
                }
            }

            if (count == 0)
            {
                continue;
            }

            keyValuePairs.Add(i, count);
        }

        return keyValuePairs;
    }

    /// <summary>
    ///  Выводит в консоль словарь <кластер; количество транзакция>
    /// </summary>
    /// <param name="map"></param>
    // Счет кластеров и количества транзакций ведется с нуля!
    internal static void PrintClusterMap(in Dictionary<int, int> map)
    {
        Dictionary<int, int> keyValuePairs = CountsNumberTransactions(map);

        for (int i = 0; i < keyValuePairs.Count; i++)
        {
            Console.WriteLine($"Кластер: {keyValuePairs.ElementAt(i).Key}; количество транзаций: {keyValuePairs.ElementAt(i).Value}");
        }

        Console.WriteLine($"Всего кластеров: {keyValuePairs.Count}");
    }

    /// <summary>
    /// Записывает в файл словарь
    /// </summary>
    /// <param name="map"></param>
    /// <param name="filePath"></param>
    /// <exception cref="Exception"></exception>
    internal static void WriteClusterMapFile(in Dictionary<int, int> map, string filePath)
    {
        Dictionary<int, int> keyValuePairs = CountsNumberTransactions(map);

        int clusterNumber = 0;

        try
        {
            using StreamWriter writer = new(filePath, false);
            {
                foreach (var (key, value) in keyValuePairs)
                {
                    clusterNumber++;

                    writer.WriteLine($"Кластер: {clusterNumber}; количество транзакций: {value}");
                }

                writer.Close();
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Во время записи файла возникла ошибка. {e.Message}");
        }
    }

    /// <summary>
    /// Выводит таблицу в консоль
    /// </summary>
    /// <param name="data"></param>
    /// <param name="printedRows"></param>
    internal static void PrintTable(in List<List<string>> data, in int printedRows = 10)
    {
        int columnsCount = data.Count;
        int rowsCount = data[0].Count;

        for (int i = 0; i < rowsCount; i++)
        {
            Console.WriteLine();

            for (int j = 0; j < columnsCount; j++)
            {
                Console.Write(data[j][i]); // поля
                //Console.Write(data[i][j]); // строки

                if (i < data.Count)
                {
                    Console.Write(',');
                }
            }

            if (i == printedRows)
            {
                return;
            }
        }
    }

    /// <summary>
    /// Выводит таблицу в консоль
    /// </summary>
    /// <param name="data"></param>
    /// <param name="printedRows"></param>
    internal static void PrintTable(in List<List<int>> data, in int printedRows = 100)
    {
        int columnsCount = data.Count;
        int rowsCount = data[0].Count;

        for (int i = 0; i < rowsCount; i++)
        {
            Console.WriteLine();

            for (int j = 0; j < columnsCount; j++)
            {
                Console.Write(data[j][i]); // поля
                //Console.Write(data[i][j]); // строки

                if (i < data.Count)
                {
                    Console.Write(',');
                }
            }

            if (i == printedRows)
            {
                return;
            }
        }
    }

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
    /// Выводит в консоль параметры кластеров
    /// </summary>
    /// <param name="clusters"></param>
    internal static void PrintClustersParams(in List<Cluster> clusters)
    {
        int n = 0;
        int w = 0;
        int s = 0;

        for (int i = 0; i < clusters.Count(); i++)
        {
            n += clusters[i].GetN();
            w += clusters[i].GetW();
            s += clusters[i].GetS();
            string str = $"Кластер № {i + 1}; N: {clusters[i].GetN()}; W: {clusters[i].GetW()}; S: {clusters[i].GetS()}";
            Console.WriteLine(str);
        }

        Console.WriteLine($"Всего N: {n}; W: {w}; S {s}");
    }

    /// <summary>
    /// Проверяет расширение файла и выводит в консоль сообщение, если расширение не соответствует текстовому файлу
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="Exception"></exception>
    internal static void CheckFileExtension(string path)
    {
        string[] textExtensions = { ".txt", ".csv", ".tsv", ".tab", ".data" };

        try
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();

            if (!(textExtensions.Contains(extension)))
            {
                Console.WriteLine("Ожидалось, что будет передан текстовый файл.");
                Console.WriteLine($"Вместо него передан файл с расширением '{extension}'.");
            }
        }
        catch (Exception e)
        {
            throw new Exception($"При проверке файла возникла ошибка. {e.Message}");
        }
    }

    /// <summary>
    /// Запрашивает в консоли путь до файла с данными и возвращает его в виде строки
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    internal static string GetFilePathFromConsole()
    {
        try
        {
            Console.WriteLine("Введите путь до файла с данными или укажите путь в коде (файл Programm.cs)");

            string? path = Console.ReadLine();

            if (path != null)
            {
                Console.WriteLine($"Указанный путь: '{path}'");

                CheckFileExtension(path);
            }

            return path;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

}