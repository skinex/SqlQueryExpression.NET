using System;
using System.Collections.Generic;
using System.Data;

namespace Ap.Tools.SqlQueryExpressions.Expressions;

public sealed class ConditionExpression
{
    public string ColumnName { get; }
    public ConditionOperator Operator { get; }
    public object Value { get; private set; }
    
    public ConditionExpression(FilterExpression nestedFilter)
    {
        Value = nestedFilter;
    }
    
    public ConditionExpression(string columnName, ConditionOperator conditionOperator, object value)
    {
        ColumnName = columnName;
        Operator = conditionOperator;
        Value = value;
    }
    
    public ConditionExpression(string columnName, ConditionOperator conditionOperator, Array value)
    {
        ColumnName = columnName;
        Operator = conditionOperator;
        Value = value;
    }

    internal string BuildCondition(string alias)
    {
        if (Value is FilterExpression nestedFilter)
        {
            return nestedFilter.BuildFilter(alias);
        }
        
        var prefixedColumnName = $"{alias}.{ColumnName}";
        
        return Operator switch
        {
            ConditionOperator.Equal => BuildDefaultCondition("=", prefixedColumnName),
            ConditionOperator.NotEqual => BuildDefaultCondition("!=", prefixedColumnName),
            ConditionOperator.GreaterThan => BuildDefaultCondition(">", prefixedColumnName),
            ConditionOperator.GreaterOrEqual => BuildDefaultCondition(">=", prefixedColumnName),
            ConditionOperator.LessOrEqual => BuildDefaultCondition("<=", prefixedColumnName),
            ConditionOperator.LessThan => BuildDefaultCondition("<", prefixedColumnName),
            ConditionOperator.Like => BuildDefaultCondition("LIKE", prefixedColumnName),
            ConditionOperator.In => BuildInCondition(prefixedColumnName),
            _ => throw new NotImplementedException()
        };
    }

    private string BuildDefaultCondition(string operatorString, string columnName)
    {
        var valueString = Value is string ? $"'{Value}'" : Value.ToString();
        return $"{columnName} {operatorString} {valueString}";
    }

    private string BuildInCondition(string columnName)
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

        return $"{columnName} IN {valueString}";
    }
}