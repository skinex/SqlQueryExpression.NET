namespace Ap.Tools.SqlQueryExpression;

public sealed class ConditionExpression
{
    public string AttributeName { get; }
    public ConditionOperator Operator { get; }
    public object Value { get; }
    
    public ConditionExpression(FilterExpression nestedFilter)
    {
        Value = nestedFilter;
    }
    
    public ConditionExpression(string attributeName, ConditionOperator conditionOperator, object value)
    {
        AttributeName = attributeName;
        Operator = conditionOperator;
        Value = value;
    }

    internal string BuildCondition(string alias)
    {
        if (Value is FilterExpression nestedFilter)
        {
            return nestedFilter.BuildFilter(alias);
        }
        
        var operatorString = Operator switch
        {
            ConditionOperator.Equal => "=",
            ConditionOperator.NotEqual => "!=",
            ConditionOperator.GreaterThan => ">",
            ConditionOperator.LessThan => "<",
            ConditionOperator.Like => "LIKE",
            _ => throw new NotImplementedException()
        };

        var valueString = Value is string ? $"'{Value}'" : Value.ToString();
        var prefixedAttributeName = $"{alias}.{AttributeName}";
        
        return $"{prefixedAttributeName} {operatorString} {valueString}";
    }
}

public enum ConditionOperator
{
    Equal,
    NotEqual,
    GreaterThan,
    LessThan,
    Like
}