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
        ImportText importText = new ImportText(filePath);
        
        DataSet dataSet = importText.DataSet;

        //Preview.PrintTable(dataSet, 50);

        if (dataSet.Count < 2) 
        {
            throw new Exception("Данные должны содержать две и более колонки");
        } 
        else if (dataSet.Count > 2)
        {
            dataSet = Transpose.TransposeDataSet(dataSet);

        }

        //Preview.PrintTable(dataSet, 50);

        TransactionData transactions = new TransactionData(dataSet, 0, 1);

        // Запускаем Clope
        Clope clope = new(repulsion, transactions);

        //Preview.PrintTable(clope.OutputTable);
        Preview.PrintTable(clope.ClusterCharacteristicsTable);

        //Preview.PrintTable(tr.items, 50);

        //for (int i = 0; i < 66; i++)
        //{
        //    Console.WriteLine(tr.items[i].ToString());
        //}

        //int ifw = 0;
        //foreach (TransactionData.Transaction trans in tr.GetTransaction())
        //{
        //    ifw++;
        //    for (int i = 0; i < trans.Count; i++)
        //    {
        //        Console.WriteLine(trans[i].ToString());
        //    }

        //    if (ifw == 26) { break; }
        //}
    }
}
