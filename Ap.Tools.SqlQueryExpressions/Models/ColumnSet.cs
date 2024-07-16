using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ap.Tools.SqlQueryExpressions.Models;

public sealed class ColumnSet
{
    public List<string> Columns { get; } = new();
    public bool IncludeAllColumns { get; set; } = false;

    public ColumnSet()
    {
    }

    public ColumnSet(bool includeAllColumns)
    {
        IncludeAllColumns = includeAllColumns;
    }

    public ColumnSet(params string[] columns)
    {
        Columns.AddRange(columns);
    }

    public ColumnSet Add(string column)
    {
        Columns.Add(column);
        return this;
    }

    public ColumnSet AddRange(params string[] columns)
    {
        Columns.AddRange(columns);
        return this;
    }

    internal StringBuilder BuildColumnSet(string alias)
    {
        var aliasedColumns = Columns.Select(column => $"{alias}.{column}").ToList();
        return new StringBuilder(string.Join(", ", aliasedColumns));
    }
}