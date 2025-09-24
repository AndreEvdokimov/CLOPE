namespace CLOPE.Import;

/// <summary>
/// Импорт текстового файла
/// </summary>
internal class TextFile
{
    /// <summary>
    /// Путь к файлу
    /// </summary>
    readonly private string filePath;
    /// <summary>
    /// 
    /// </summary>
    readonly private List<string> DataSet;

    internal TextFile(string filePath)
    {
        this.CheckFileExtension(filePath);

        this.filePath = filePath;

        this.DataSet = new List<string>();

        this.ReadRows();
    }

    /// <summary>
    /// Проверяет расширение файла и выводит в консоль сообщение, если его расширение не соответствует ожидаемому
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

    /// <summary>
    /// Читает строки текстовго файла и записывает из в список
    /// </summary>
    private void ReadRows()
    {
        try
        {
            foreach (string line in TextFileReader.ReadLineSync(this.filePath))
            {
                this.DataSet.Add(line);
            }
        }
        catch (Exception e) 
        { 
            throw new Exception($"Во время чтения файла произошла ошибка. {e.Message}");
        }
    }

    /// <summary>
    /// Возвращает строку текстовго файла
    /// </summary>
    /// <returns></returns>
    internal IEnumerable<string> GetRow()
    {
        if (DataSet.Count == 0) { yield break; }

        foreach (string row in DataSet) 
        {
            yield return row;
        }
    }
}
