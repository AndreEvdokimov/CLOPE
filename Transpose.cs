/// <summary>
/// Транспонирование данных. Дополнительно добавляется поле с индексами строк
/// </summary>
internal static class Transpose
{
    /// <summary>
    /// Транспонирует таблицу и добавляет столбец с номерами строк
    /// </summary>
    /// <param name="data"></param>
    /// <returns>DataSet с двумя полями: номер строки и данные переданного dataSet-а</returns>
    internal static DataSet TransposeDataSet(in DataSet data)
    {
        DataSet res = new DataSet(2);

        res.AddColumn("ID");
        res.AddColumn("DATA");

        for (int i = 0; i < data[0].Count; i++) // строки
        {
            for (int j = 0; j < data.Count; j++) // поля
            {
                res[0].AddValue(i); // Или номера строк добавить во время импорта?
                res[1].AddValue(data[j][i]);
            }
        }

        return res;
    }
}

