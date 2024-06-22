using System.Data;
using System.Text;

namespace Ap.Tools.SqlQueryExpression;

public sealed class ConditionExpression
{
    public string AttributeName { get; }
    public ConditionOperator Operator { get; }
    public object Value { get; private set; }
    
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
    
    public ConditionExpression(string attributeName, ConditionOperator conditionOperator, Array value)
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
        
        var prefixedAttributeName = $"{alias}.{AttributeName}";
        
        return Operator switch
        {
            ConditionOperator.Equal => BuildDefaultCondition("=", prefixedAttributeName),
            ConditionOperator.NotEqual => BuildDefaultCondition("!=", prefixedAttributeName),
            ConditionOperator.GreaterThan => BuildDefaultCondition(">", prefixedAttributeName),
            ConditionOperator.LessThan => BuildDefaultCondition("<", prefixedAttributeName),
            ConditionOperator.Like => BuildDefaultCondition("LIKE", prefixedAttributeName),
            ConditionOperator.In => BuildInCondition(prefixedAttributeName),
            _ => throw new NotImplementedException()
        };
    }

    private string BuildDefaultCondition(string operatorString, string attributeName)
    {
        var valueString = Value is string ? $"'{Value}'" : Value.ToString();
        return $"{attributeName} {operatorString} {valueString}";
    }

    private string BuildInCondition(string attributeName)
    {
        if (Value is not Array values)
        {
            throw new ConstraintException("IN operator must be supplied with array of values");
        }

        var inValues = new List<string>();
        
        foreach (var value in values)
        {
            var valueType = value.GetType();

            switch (value)
            {
                case string strValue:
                    inValues.Add($"'{strValue}'");
                    continue;
                case DateTime dt:
                    inValues.Add($"'{dt.ToString("O")}'");
                    continue;
                case Guid gd:
                    inValues.Add($"'{gd.ToString("D")}'");
                    continue;
            }

            if (valueType.IsPrimitive)
            {
                inValues.Add(value.ToString());
            }
        }

        var valueString = $"({string.Join(',', inValues)})";

        return $"{attributeName} IN {valueString}";
    }
}

public enum ConditionOperator
{
    Equal,
    NotEqual,
    GreaterThan,
    LessThan,
    Like,
    In
}