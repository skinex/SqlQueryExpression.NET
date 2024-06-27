using System.Collections.Generic;
using System.Text;

namespace Ap.Tools.SqlQueryExpressions.Expressions;

public sealed class FilterExpression
{
    public List<ConditionExpression> Conditions { get; } = new();
    public LogicalOperator LogicalOperator { get; }

    public FilterExpression(LogicalOperator logicalOperator = LogicalOperator.AND)
    {
        LogicalOperator = logicalOperator;
    }

    public void AddCondition(ConditionExpression condition)
    {
        Conditions.Add(condition);
    }

    internal string BuildFilter(string alias = null)
    {
        if (Conditions.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        sb.Append('(');

        for (var i = 0; i < Conditions.Count; i++)
        {
            if (i > 0)
            {
                sb.Append($" {LogicalOperator} ");
            }
            
            sb.Append(Conditions[i].BuildCondition(alias));
        }

        sb.Append(')');
        return sb.ToString();
    }
}