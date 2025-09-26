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

    internal TextFile(string filePath)
    {
        this.CheckFileExtension(filePath);

        this.filePath = filePath;
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
    /// Возвращает строки текстовго файла
    /// </summary>
    /// <returns></returns>
    internal IEnumerable<string> GetRow()
    {
        foreach (string row in TextFileReader.ReadLineSync(this.filePath)) 
        {
            yield return row;
        }
    }
}
