using System.Text;

internal class ImportText
{
    private static int DefineBufferSize(in string path)
    {
        try
        {
            FileInfo fileInfo = new(path);

            long fileSize = fileInfo.Length;

            // Console.WriteLine($"File size: {fileSize / 1024 / 1024}MB, Buffer size: {bufferSize / 1024}KB");

            if (fileSize < 1024 * 1024) // < 1MB
                return 4096; // 4KB
            else if (fileSize < 10 * 1024 * 1024) // < 10MB
                return 8192; // 8KB
            else if (fileSize < 100 * 1024 * 1024) // < 100MB
                return 16384; // 16KB
            else if (fileSize < 1024 * 1024 * 1024) // < 1GB
                return 32768; // 32KB
            else
                return 65536; // 64KB для очень больших файлов
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return 8192; // Значение по умолчанию
        }
    }

    // Определяет разделитель. Если не удалось определить разделитель, вернет разделитель ','
    private static char DefineDelimiter(in string path, in Encoding encoding, in int linesCountAnalize)
    {
        char comma = ',';
        char semicolon = ';';
        char pipeline = '|';

        Dictionary<char, int> possibleDelimitersCount = new()
        {
            { comma, 0 },
            { semicolon, 0 },
            { pipeline, 0 },
        };

        try
        {
            using StreamReader reader = new(path, encoding);

            int count = 0;

            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                count++;

                if (count == linesCountAnalize)
                {
                    break;
                }

                for (int i = 0; i < line.Length; i++)
                {
                    switch (line[i])
                    {
                        case ',':
                            possibleDelimitersCount[comma]++;
                            break;
                        case ';':
                            possibleDelimitersCount[semicolon]++;
                            break;
                        case '|':
                            possibleDelimitersCount[pipeline]++;
                            break;
                    }
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception($"При чтении файла возникла ошибка. {e.Message}");
        }

        KeyValuePair<char, int> foundedDelimiter = possibleDelimitersCount.Aggregate((l, r) => l.Value > r.Value ? l : r);

        if (foundedDelimiter.Value == 0)
        {
            return ',';
        }

        return foundedDelimiter.Key;
    }

    private static int DefineFieldsCount(in string path, in char delimiter, in int linesCountAnalize, in Encoding encoding)
    {
        int res = 0;

        try
        {
            using StreamReader streamReader = new(path, encoding);

            int count = 0;

            string? line;


            while ((line = streamReader.ReadLine()) != null)
            {
                count++;

                if (count == linesCountAnalize)
                {
                    break;
                }

                // разбиваем строку на массив элементов
                string[] rowItems = line.Split(delimiter);

                if (rowItems.Length > res)
                {
                    res = rowItems.Length;
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception($"При чтении файла возникла ошибка. {e.Message}");
        }

        return res;
    }

    // Возвращает массив данных файла в котором каждый массив это поле (столбец)
    private static List<List<string>> GetFields(in string path, in int fieldsCount, in Encoding encoding, in char delimiter = ',', in char nullValue = '?')
    {
        List<List<string>> fields = new List<List<string>>(fieldsCount);

        for (int i = 0; i < fieldsCount; i++)
        {
            fields.Add(new List<string>());
        }

        try
        {
            // Размер буффера
            int bufferSize = DefineBufferSize(path);

            using FileStream fileStream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using BufferedStream bufferedStream = new(fileStream, bufferSize);
            using StreamReader reader = new(bufferedStream, encoding, true);

            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                // разбиваем строку на массив элементов
                string[] rowItems = line.Split(delimiter);

                int rowLength = 0;

                if (rowItems.Length > fieldsCount)
                {
                    rowLength = fieldsCount;
                }
                else
                {
                    rowLength = rowItems.Length;
                }

                // каждый вложенный массив - это поле (столбец файла)
                for (int i = 0; i < rowLength; i++)
                {
                    if (string.IsNullOrWhiteSpace(rowItems[i]) || (rowItems[i] == nullValue.ToString()))
                    {
                        fields[i].Add("NULL");
                    }
                    else
                    {
                        fields[i].Add(rowItems[i].Trim());
                    }
                }

                if (rowLength < fieldsCount)
                {
                    // Если количество символов (столбцов) в строке меньше определенного методом DefineFieldsCount(),
                    // то в место нехватающих символом добавим NULL - пустое значение
                    for (int i = rowItems.Length; i <= fieldsCount - 1; i++)
                    {
                        fields[i].Add("NULL");
                    }
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception($"При чтении файла возникла ошибка. {e.Message}");
        }

        return fields;
    }

    internal static List<List<string>> FileData(in string path, in Encoding encoding, in int linesCountAnalize = 50)
    { 
        char delimiter = DefineDelimiter(path, encoding, linesCountAnalize);

        int fieldsCount = DefineFieldsCount(path, delimiter, linesCountAnalize, encoding);

        return GetFields(path, fieldsCount, encoding, delimiter);
    }

    internal static List<List<string>> FileData(in string path, in char delimiter, in char nullValue, in Encoding encoding, in int linesCountAnalize = 50)
    {
        int fieldsCount = DefineFieldsCount(path, delimiter, linesCountAnalize, encoding);

        return GetFields(path, fieldsCount, encoding, delimiter, nullValue);
    }
}
