using System.Text;

namespace CLOPE.Helpers;
/// <summary>
/// Хелперы для дебага, проверки результатов работы других классов и т.п.
/// </summary>
internal class Helpers
{
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
    /// Путь к файлу с данными из папки "DataStorage" данного проекта
    /// </summary>
    /// <param name="fileName">Название файла</param>
    /// <returns></returns>
    /// <exception cref="Exception">Файл не найден</exception>
    internal static string DataFilePath(in string fileName) 
    {
        var path = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "DataStorage", fileName));

        if (!File.Exists(path)) { throw new Exception($"Файл {fileName} не найден"); }

        return path;
    }
}