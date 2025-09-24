namespace CLOPE.Import;

/// <summary>
/// "Читатель" текстового файла
/// </summary>
internal static class TextFileReader
{
    /// <summary>
    /// Возвращает строки файла поочереди. Пустые строки пропускаются
    /// </summary>
    /// <returns>Строка файла</returns>
    internal static IEnumerable<string> ReadLineSync(string filePath)
    {
        string? line;

        using StreamReader reader = new(filePath, true);
        
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrEmpty(line)) // пропускаем пустые строки
            {
                continue;
            }

            yield return line;
        }
    }
}
