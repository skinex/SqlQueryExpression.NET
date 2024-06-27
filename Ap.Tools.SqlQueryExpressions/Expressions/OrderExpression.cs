namespace Ap.Tools.SqlQueryExpressions.Expressions;

public sealed class OrderExpression
{
    public string AttributeName { get; }
    public SortingType SortingType { get; }

    public OrderExpression(string attributeName, SortingType sortingType)
    {
        AttributeName = attributeName;
        SortingType = sortingType;
    }

    internal string BuildOrder(string alias = null)
    {
        var prefixedAttributeName = alias != null ? $"{alias}.{AttributeName}" : AttributeName;
        return $"{prefixedAttributeName} {(SortingType)}";
    }
}