/// <summary>
/// Предпросмотр данных в консоли
/// </summary>
internal static class Preview
{
    /// <summary>
    /// Выводит таблицу в консоль
    /// </summary>
    /// <param name="data"></param>
    /// <param name="printedRows"></param>
    internal static void PrintTable(in DataSet data, in int? printedRows = null)
    {
        for (int i = 0; i < data.Count; i++)
        {
            Console.Write(data[i].Name + "    ");
        }

        for (int i = 0; i < data[0].Count; i++)
        {
            Console.WriteLine();

            for (int j = 0; j < data.Count; j++)
            {
                Console.Write(data[j][i] + "    "); // поля
                //Console.Write(data[i][j]); // строки

                //if (i < data.Count)
                //{
                //    Console.Write(',');
                //}
            }

            if (printedRows != null && i == printedRows)
            {
                return;
            }
        }
        
        Console.WriteLine();
    }

}
