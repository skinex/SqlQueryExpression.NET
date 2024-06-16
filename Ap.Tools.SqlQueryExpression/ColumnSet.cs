using System.Text;

namespace Ap.Tools.SqlQueryExpression;

public sealed class ColumnSet
{
    public List<string> Columns { get; } = new();
    public bool IncludeAllColumns { get; }

    public ColumnSet(bool includeAllColumns)
    {
        IncludeAllColumns = includeAllColumns;
    }

    public ColumnSet(params string[] columns)
    {
        Columns.AddRange(columns);
    }

    internal StringBuilder BuildColumnSet(string alias)
    {
        var aliasedColumns = Columns.Select(column => $"{alias}.{column}").ToList();
        return new StringBuilder(string.Join(", ", aliasedColumns));
    }
}