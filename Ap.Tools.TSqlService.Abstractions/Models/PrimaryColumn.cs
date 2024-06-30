namespace Ap.Tools.TSqlService.Abstractions.Models
{
    /// <summary>
    /// An representation of primary key column
    /// it holds name of column and it's typed value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PrimaryColumn<T>
    {
        public string ColumnName { get; }
        public T ColumnValue { get; }

        public PrimaryColumn(string columnName, T columnValue)
        {
            ColumnName = columnName;
            ColumnValue = columnValue;
        }
    }
}