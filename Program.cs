using System;
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

        // Получаем путь до файла из консоли
        string filePath = Helpers.GetFilePathFromConsole();

        TextFile textFile = new TextFile(filePath);

        TransactionSetParams dataSetParams = new TransactionSetParams();

        TransactionSet transactions = new TransactionSet(textFile, dataSetParams);

        ClusterSet clusters = new ClusterSet();

        ClopeEngine.Run(transactions, clusters, repulsion); // Запускаем Clope


        clusters.PrintClustersCharacteristicsTable();
    }
}
