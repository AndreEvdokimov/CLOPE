/// < summary >
/// Поле данных
/// </ summary >
internal interface IColumn
{
    /// <summary>
    /// Название поля
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Элемент списка данных
    /// </summary>
    /// <param name="index">Индекс элемента</param>
    /// <returns></returns>
    string this[int index] { get; }
    /// <summary>
    /// Список данных
    /// </summary>
    List<string> Data { get; }
    /// <summary>
    /// Количестов элементов
    /// </summary>
    int Count => Data.Count;
}

/// <summary>
/// Набор данных string, int или double
/// </summary>
internal class DataSet
{
    /// <summary>
    /// Поле
    /// Допустимые типы данных: 
    /// </summary>
    protected class Column : IColumn
    {
        public string Name { get; private set; }
        public List<string> Data { get; }
        public int Count => this.Data.Count;
        internal Column(string name)
        {
            this.Name = name;
            this.Data = new List<string>();
        }
        public string this[int index] => this.Data[index];
    }

    /// <summary>
    /// Набор полей
    /// </summary>
    private readonly List<IColumn> Columns;
    /// <summary>
    /// Количество полей в наборе
    /// </summary>
    public int Count => this.Columns.Count;

    internal DataSet(int? size = null) // Конструктор
    {
        this.Columns = new List<IColumn>(size ?? 0);
    }

    /// <summary>
    /// Возвращает поле из набора по индексу
    /// </summary>
    /// <param name="index">Индекс</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal IColumn this[int index] => this.Columns[index];

    /// <summary>
    /// Добавляет поле в набор
    /// </summary>
    /// <param name="index">Индекс поля</param>
    /// <param name="name">Название поля</param>
    /// <param name="dataType">Тип поля</param>
    internal void AddColumn(string name)
    {
        this.Columns.Add(new Column(name));
    }

    /// <summary>
    /// Удаляет поле из набора
    /// </summary>
    /// <param name="index"></param>
    internal void RemoveColumn(int index)
    {
        bool res = this.Columns.Remove(this.Columns[index]);

        if (!res)
        {
            Console.WriteLine($"Не удалось удалить поле по указанному индексу: {index}");
        }
    }

}
