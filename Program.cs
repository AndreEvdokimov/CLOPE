using CLOPE.Transactions;
using CLOPE.Core;
using CLOPE.Clusters;
using CLOPE.Import;

internal class Program
{
    static void Main(string[] args)
    {
        double repulsion = 2.6;

        // Получаем путь до файла из консоли
        //string filePath = Helpers.GetFilePathFromConsole();

        // Путь до файла с параметрами грибов (ненормализрованного)
        string mooh = @"..\..\..\DataStorage\mooh_with_ids.txt";

        // Путь до файла с чеками
        string checks = @"..\..\..\DataStorage\checks_utf-8.txt";

        // Параметры набора транзакций с грибами
        TransactionSetParams moohSetParams = new TransactionSetParams() { ColIds = 0, Delimiter = ',' };

        // Параметры набора транзакций с грибами
        TransactionSetParams checksSetParams = new TransactionSetParams() { ColIds = 0, Delimiter = '\t', NullValue = "\t" };

        // Читаем строки текстового файла
        TextFile textFile = new TextFile(checks);

        // Подготавливаем набор транзакций
        TransactionSet transactions = new TransactionSet(textFile, checksSetParams);

        // Создаем набор кластеров
        ClusterSet clusters = new ClusterSet();

        // Запускаем Clope
        ClopeEngine.Run(transactions, clusters, repulsion);

        // Выводим в консоль таблицы <id транзакции : id кластера>
        //transactions.PrintTransactions();

        // Выводим в консоль характеристики кластера
        clusters.PrintClustersCharacteristicsTable();
    }
}
