using CLOPE.Transactions;
using CLOPE.Core;
using CLOPE.Clusters;
using CLOPE.Import;
using CLOPE.Helpers;

internal class Program
{
    static void Main(string[] args)
    {
        double repulsion = 2.6;

        // Путь до файла с параметрами грибов
        string mooh = Helpers.DataFilePath("mooh_with_ids.txt");

        // Параметры набора транзакций с грибами
        TransactionSetParams moohSetParams = new TransactionSetParams() { SkippedCols = new int[] { 0, 1 }, Delimiter = ',', NullValues = new string[] { "?" } };

        // Читаем строки текстового файла
        TextFile moohTxt = new TextFile(mooh);

        // Подготавливаем набор транзакций
        TransactionSet transactions = new TransactionSet(moohTxt, moohSetParams);

        // Создаем набор кластеров
        ClusterSet clusters = new ClusterSet();

        // Запускаем Clope
        var clope = new ClopeEngine();

        // Результат работы алгоритма
        var clopeRes = clope.Run(transactions, clusters, repulsion);

        // Вывод в консоль разбиаения
        PrintToConsole.ClopeResultTable(clopeRes);

        // Выводим в консоль характеристики кластера
        PrintToConsole.ClusterCharacteristics(clusters);
    }
}
