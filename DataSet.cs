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
    object this[int index] { get; }
    /// <summary>
    /// Список данных
    /// </summary>
    List<object> Data { get; set; }
    /// <summary>
    /// Количестов элементов
    /// </summary>
    int Count => Data.Count;
    /// <summary>
    /// Добавляет значение в поле
    /// </summary>
    /// <param name="value"></param>
    void AddValue(object value);
    /// <summary>
    /// Возвращает диапазон значения
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<object> GetRange(int index, int count);
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
        public List<object> Data { get; set; }
        public int Count => this.Data.Count;
        internal Column(string name)
        {
            this.Name = name;
            this.Data = new List<object>();
        }
        public object this[int index] => this.Data[index];

        public void AddValue(object value)
        {
            //Type type = value.GetType();

            //if (value is string || value is int || value is char)
            //{
            //    this.Data.Add(value);
            //}
            //else
            //{
            //    throw new Exception($"Тип '{type}' значения '{value}' не соответствует допустимым типам: string, int и char");
            //}

            this.Data.Add(value);
        }

        public List<object> GetRange(int index, int count)
        {
            return this.Data.GetRange(index, count);
        }
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
    internal IColumn this[int index]
    {
        get
        {
            if (index < 0 || index >= this.Columns.Count) { throw new ArgumentOutOfRangeException(nameof(index)); }

            return this.Columns[index];
        }
    }

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
