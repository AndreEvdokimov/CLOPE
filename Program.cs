using System;
internal class Program
{
    static void Main(string[] args)
    {
        double repulsion = 2.6;

        // Получаем путь до файла из консоли
        string filePath = Helpers.GetFilePathFromConsole();

        // Загружаем данные из файла
        ImportText importText = new ImportText(filePath);

        DataSet dataSet = importText.DataSet;

        int colIndicesId = 0; // индекс поля, которое содержит индексы транзакций

        if (dataSet.Count < 2)
        {
            throw new Exception("Данные должны содержать две и более колонки");
        }
        else if (dataSet.Count > 2) // если в данных по типу грибов нет поля с индексами транзакций, то добавим его
        {
            Helpers.AddColumnIds(dataSet);

            colIndicesId = dataSet.Count - 1;
        }

        Transactions transactions = new Transactions(dataSet, colIndicesId);

        Clope clope = new(repulsion, transactions); // Запускаем Clope

        //Preview.PrintTable(clope.OutputTable);
        Preview.PrintTable(clope.ClusterCharacteristicsTable);
    }
}
