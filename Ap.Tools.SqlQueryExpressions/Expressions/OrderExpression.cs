namespace Ap.Tools.SqlQueryExpressions.Expressions;

public sealed class OrderExpression
{
    public string ColumnName { get; }
    public SortingType SortingType { get; }

    public OrderExpression(string columnName, SortingType sortingType)
    {
        ColumnName = columnName;
        SortingType = sortingType;
    }

    internal string BuildOrder(string alias = null)
    {
        var prefixedAttributeName = alias != null ? $"{alias}.{ColumnName}" : ColumnName;
        return $"{prefixedAttributeName} {(SortingType)}";
    }
}