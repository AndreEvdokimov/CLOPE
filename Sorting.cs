/// <summary>
/// Сортировка данных
/// </summary>
internal static class Sorting
{
    /// <summary>
    /// Возвращает true, если столбец с индексами dataSet-а отсортирован по возрастанию, иначе вернет false
    /// </summary>
    /// <param name="data"></param>
    /// <returns>true - данные отсортированы по возрастанию, false - в противном случае</returns>
    internal static bool IsSortedAscending(DataSet data)
    {
        for (int i = 0; i < data[0].Count; i++) // поидее не должны вылезти за границы массива
        {
            if (i + 1 >= data[0].Count) { break; }

            if (data[0][i] is IComparable comp1 && data[0][i + 1] is IComparable comp2)
            {
                if (comp1.CompareTo(comp2) > 0)
                {
                    return false;
                }
            }
            else
            {
                if (data[0][i].ToString().CompareTo(data[0][i + 1].ToString()) > 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Сортирует данные по указанному столбцу
    /// </summary>
    internal static DataSet SortASC(DataSet data, int targetColumn = 0)
    {
        if (IsSortedAscending(data)) { return data; }

        DataSet sortedData = new DataSet(data.Count);

        for (int i = 0; i < data.Count; i++)
        {
            sortedData.AddColumn(data[i].Name);
        }

        // Создаем список индексов для сортировки
        List<int> sortedIndices = [.. Enumerable.Range(0, data[targetColumn].Count)];

        // Сортируем индексы по значениям в первой колонке (ID транзакций)
        sortedIndices.Sort((i, j) =>
        {
            // Сравниваем значения в зависимости от их типа
            if (data[targetColumn][i] is IComparable comp1 && data[targetColumn][j] is IComparable comp2)
            {
                return comp1.CompareTo(comp2);
            }

            return data[targetColumn][i].ToString().CompareTo(data[targetColumn][j].ToString()); // для случаев, когда типы не совпадают
        });


        foreach (int i in sortedIndices)
        {
            for (int j = 0; j < sortedData.Count; j++)  // нужно проверить этот цикл!
            {
                sortedData[j].AddValue(data[j][i]);
            }
        }

        return sortedData;
    }

}

