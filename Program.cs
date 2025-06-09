using CLOPE;
using System.Text;
internal class Program
{
    static void Main(string[] args)
    {
        double repulsion = 2.6;

        string mooh = "C:\\Users\\touch\\Desktop\\mushroom\\agaricus-lepiota.data";

        //List<List<string>> data = ImportText.FileData(mooh, Encoding.UTF8);

        // Получаем путь до файла из консоли
        string filePath = Helpers.GetFilePathFromConsole();

        // Загружаем данные из файла
        List<List<string>> data = ImportText.FileData(filePath, Encoding.UTF8);

        // Подготавливаем транзакции в виде взаимно однозначного отображения между множеством уникальных объектов таблицы и множеством целых чисел
        List<List<int>> transactions = OneToOneMappingTable.GetOneToOneMappingTable(data, [0]);

        // Запускаем Clope
        Clope clope = new(repulsion, transactions);

        // Получаем разбиение на кластеры
        Dictionary<int, int> clopeRes = clope.GetClusterMap();
        
        // Выводим разбиение в консоль
        Helpers.PrintClustersParams(clope.GetClusters());

        //Helpers.WriteClusterMapFile(clopeRes, resFilePath);

        //Helpers.PrintTable(data);
    }
}
