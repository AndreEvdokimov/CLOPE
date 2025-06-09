using System.Diagnostics;

internal static class OneToOneMappingTable
{
    // Возвращает словарь с сопоставлением уникального элемента поля и его индекса
    private static List<Dictionary<string, int>> FindColumnsUniqValues(in List<List<string>> data)
    {
        Debug.Assert(data != null);

        // Количество полей
        int columnsCount = data.Count;
        // индекс уникального элемента поля
        int uniqItemId = 0;
        List<Dictionary<string, int>> columnsUniqValues = new(columnsCount);

        // поиск уникальных элементов в наборе по полям
        for (int i = 0; i < columnsCount; i++) // поле
        {
            columnsUniqValues.Add(new Dictionary<string, int>());

            for (int j = 0; j < data[i].Count; j++) // строка
            {
                string stringItem = data[i][j]; // элемент поля i строки j

                if ((!columnsUniqValues[i].ContainsKey(stringItem)) && (stringItem != "NULL")) // пропущенные (null) значения пропускаем
                {
                    columnsUniqValues[i].Add(stringItem, uniqItemId);
                    uniqItemId++;
                }
            }
        }

        return columnsUniqValues;
    }

    // Возвращает взаимно однозначное отображение между множеством уникальных объектов таблицы и множеством целых чисел
    // Каждый вложенный список - транзакция (строка таблицы)
    internal static List<List<int>> GetOneToOneMappingTable(in List<List<string>> data, in int[] excludedColumnsId)
    {
        Debug.Assert(data != null);

        // Количество полей
        int columnsCount = data.Count;
        // Количество строк
        int rowsCount = data[0].Count;
        // Результирующая таблица
        List<List<int>> res = new(rowsCount);
        // Уникальные элементы полей таблицы
        List<Dictionary<string, int>> uniqValues = OneToOneMappingTable.FindColumnsUniqValues(data);

        for (int i = 0; i < rowsCount; i++) // строка
        {
            res.Add([]);

            for (int j = 0; j < columnsCount; j++) // поле
            {
                if (excludedColumnsId.Contains(j))
                {
                    continue;
                }

                // элемент строки
                string rowItem = data[j][i];
                // словарь, который соответствует полю
                Dictionary<string, int> columnDict = uniqValues[j];

                if (columnDict.TryGetValue(rowItem, out int value))
                {
                    res[i].Add(value);
                }
            }
        }

        return res;
    }

}
