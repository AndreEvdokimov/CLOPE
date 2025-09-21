/// <summary>
/// Импорт текстового файла
/// </summary>
internal class ImportText
{
    /// <summary>
    /// "Читатель" текстового файла
    /// </summary>
    private class TextFileReader : IDisposable
    {
        /// <summary>
        /// Поток файла
        /// </summary>
        readonly private FileStream fileStream;
        /// <summary>
        /// Поток чтения
        /// </summary>
        readonly private StreamReader reader;
        /// <summary>
        /// Количество пропускаемых строк начиная с первой строки
        /// </summary>
        readonly private int linesCountSkipped;
        /// <summary>
        /// "Утилизирован" ли класс
        /// </summary>
        private bool isDisposed = false;
        /// <summary>
        /// "Утилизирован" ли класс
        /// </summary>
        internal bool IsDisposed => this.isDisposed;

        internal TextFileReader(in string filePath, int linesCountSkipped = 0)
        {
            this.linesCountSkipped = linesCountSkipped;

            try
            {
                int bufferSize = DefineBufferSize(filePath);

                this.fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize);
                this.reader = new(fileStream, true);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Определяем размер буффера в зависимости от размера файла
        /// </summary>
        /// 
        /// \ъ
        /// <param name="path">Путь до файла</param>
        /// <returns></returns>
        private int DefineBufferSize(in string path)
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
                Console.WriteLine(e.Message);
                return 8192; // Значение по умолчанию
            }
        }

        /// <summary>
        /// Возвращает строки файла поочереди. Пустые строки пропускаются
        /// </summary>
        /// <returns>Строка файла</returns>
        internal IEnumerable<string> ReadLineSync()
        {
            ThrowIfDisposed();

            int lineId = 0;

            string? line;
            while ((line = this.reader.ReadLine()) != null)
            {
                if (this.linesCountSkipped != 0)
                {
                    lineId++;

                    if (lineId < this.linesCountSkipped)
                    {
                        continue;
                    }
                }

                if (string.IsNullOrEmpty(line)) // пропускаем пустые строки
                {
                    continue;
                }

                yield return line;
            }
        }

        /// <summary>
        /// Сброс указателя на начало потока
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        internal void Reset()
        {
            ThrowIfDisposed();

            try
            {
                this.reader.DiscardBufferedData();
                this.fileStream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при сбросе позиции файла: {ex.Message}");
            }
        }

        /// <summary>
        /// Проверка на disposed состояние
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        private void ThrowIfDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(TextFileReader), "Объект уже освобожден");
            }
        }

        /// <summary>
        /// Реализация IDisposable
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Освобождает ресурсы
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                // Освобождаем управляемые ресурсы в правильном порядке
                reader.Dispose();
                fileStream.Dispose();
            }

            isDisposed = true;
        }

        /// <summary>
        /// Финализатор
        /// </summary>
        ~TextFileReader()
        {
            Dispose(false);
        }
    }

    /// <summary>
    /// Набор данных
    /// </summary>
    private DataSet dataSet;
    /// <summary>
    /// "Читатель" текстового файла
    /// </summary>
    readonly private TextFileReader reader;
    /// <summary>
    /// Набор данных
    /// </summary>
    internal DataSet DataSet => this.dataSet;
    /// <summary>
    /// Количество пропускаемых строк по умолчанию
    /// </summary>
    private readonly int defaultSkipedLinesCount = 0;
    /// <summary>
    /// Знак пустого значения по умолчанию
    /// </summary>
    private readonly char defaultCharNull = '?';


    internal ImportText(in string filePath, in char delimiter, in char nullValue, in int skippedlinesCount)
    { 
        this.CheckFileExtension(filePath);

        this.reader = new TextFileReader(filePath, skippedlinesCount);

        this.DefineFieldsCount(delimiter, out int fieldsCount);

        this.dataSet = new(fieldsCount);

        this.ReadFields(filePath, fieldsCount, delimiter, nullValue);
    }

    internal ImportText(in string filePath, in char? nullValue = null, in int? skippedlinesCount = null)
    {
        this.CheckFileExtension(filePath);

        this.reader = new TextFileReader(filePath, skippedlinesCount ?? this.defaultSkipedLinesCount);

        this.ParsLine(out char delimiter, out int fieldsCount);

        this.dataSet = new(fieldsCount);

        this.ReadFields(filePath, fieldsCount, delimiter, nullValue ?? this.defaultCharNull);
    }

    /// <summary>
    /// Парсит строку, возвращяя разделитель и количество полей
    /// </summary>
    /// <param name="delimiter"></param>
    /// <param name="fieldsCount"></param>
    private void ParsLine(out char delimiter, out int fieldsCount)
    {
        char[] delimiters = { ',', ';', '|', '\t', ' ' };
        int[] counts = new int[4]; // comma, semicolon, pipeline, tab, space
        string analyzedLine = "";

        foreach (string line in this.reader.ReadLineSync()) // из файла получаем строку для анализа
        {
            analyzedLine = line;

            this.reader.Reset(); // возвращаем указатель в начало файла

            break;
        }

        for (int i = 0; i < analyzedLine.Length; i++)
        {
            char c = analyzedLine[i];

            switch (c)
            {
                case ',':
                    counts[0]++;
                    break;
                case ';':
                    counts[1]++;
                    break;
                case '|':
                    counts[2]++;
                    break;
                case '\t':
                    counts[3]++;
                    break;
                case ' ':
                    counts[4]++;
                    break;
            }
        }

        int maxCount = 0;
        int maxIndex = 0;

        for (int i = 0; i < counts.Length; i++)
        {
            if (counts[i] > maxCount)
            {
                maxCount = counts[i];
                maxIndex = i;
            }
        }

        // Возвращаем разделитель с максимальным количеством или запятую по умолчанию
        delimiter = maxCount > 0 ? delimiters[maxIndex] : ',';

        fieldsCount = analyzedLine.Split(delimiter).Length;
    }

    /// <summary>
    /// Возвращает количество полей в файле
    /// </summary>
    /// <param name="delimiter">Разделить строк</param>
    /// <param name="fieldsCount">Количество полей</param>
    private void DefineFieldsCount(char delimiter, out int fieldsCount)
    {
        string analyzedLine = "";

        foreach (string line in this.reader.ReadLineSync()) // из файла получаем строку для анализа
        {
            analyzedLine = line;

            this.reader.Reset(); // возвращаем указатель в начало файла

            break;
        }

        fieldsCount = analyzedLine.Split(delimiter).Length;
    }

    /// <summary>
    /// Читает поля файла
    /// </summary>
    /// <param name="path">Путь к файлу</param>
    /// <param name="fieldsCount">Количество полей</param>
    /// <param name="delimiter">Разделитель строк</param>
    /// <param name="nullValue">Значение для null</param>
    /// <exception cref="Exception"></exception>
    private void ReadFields(string path, int fieldsCount, char delimiter, char nullValue)
    {
        try
        {
            for (int i = 0; i < fieldsCount; i++) // подготовим поля
            {
                this.dataSet.AddColumn($"COL{i}");
            }

            foreach (string line in this.reader.ReadLineSync())
            {
                // разбиваем строку на массив элементов
                string[] lineItems = line.Split(delimiter);

                // каждый вложенный массив - это поле (столбец файла)
                for (int i = 0; i < fieldsCount; i++)
                {
                    if (i > lineItems.Length)
                    {
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(lineItems[i]) || (lineItems[i] == nullValue.ToString()))
                    {
                        this.dataSet[i].AddValue("NULL");
                    }
                    else
                    {
                        this.dataSet[i].AddValue(lineItems[i].Trim());
                    }
                }

                if (lineItems.Length < fieldsCount)
                {
                    // Если количество символов (столбцов) в строке меньше определенного,
                    // то вместо нехватающих символом добавим NULL - пустое значение
                    for (int i = lineItems.Length; i < fieldsCount; i++)
                    {
                        this.dataSet[i].AddValue("NULL");
                    }
                }
            }

            this.reader.Dispose();
        }
        catch (Exception e)
        {
            throw new Exception($"Во время импорта произошла ошибка. {e.Message}");
        }
        finally
        {
            if (!reader.IsDisposed) { this.reader.Dispose(); }
        }
    }

    /// <summary>
    /// Проверяет расширение файла и выводит в консоль сообщение, если расширение не соответствует текстовому файлу
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="Exception"></exception>
    private void CheckFileExtension(string path)
    {
        string[] textExtensions = { ".txt", ".csv", ".tsv", ".tab", ".data" };

        try
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();

            if (!(textExtensions.Contains(extension)))
            {
                Console.WriteLine("Ожидалось, что будет передан текстовый файл.");
                Console.WriteLine($"Вместо него передан файл с расширением '{extension}'.");
                throw new Exception("Для чтения передан не текстовый файл");
            }
        }
        catch (Exception e)
        {
            throw new Exception($"При проверке файла возникла ошибка. {e.Message}");
        }
    }
}
