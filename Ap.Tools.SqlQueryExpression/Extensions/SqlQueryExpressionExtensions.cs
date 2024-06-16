using System.Text;

namespace Ap.Tools.SqlQueryExpression.Extensions;

public static class SqlQueryExpressionExtensions
{
    public static string Union(this SqlQueryExpression query, 
        UnionType unionType = UnionType.Default, 
        params SqlQueryExpression[] queries)
    {
        var sb = new StringBuilder();

        sb.Append(query.BuildQuery());

        var newLine = Environment.NewLine;

        var unionStatement = unionType == UnionType.Default ? $"{newLine}UNION{newLine}" : $"{newLine}UNION ALL{newLine}";

        foreach (var union in queries)
        {
            var unionQuery = union.BuildQuery();
            sb.Append(unionStatement);
            sb.Append(unionQuery);
        }

        return sb.ToString();
    }
    
    public static string ExceptAll(this SqlQueryExpression query, 
        params SqlQueryExpression[] queries)
    {
        var sb = new StringBuilder();

        sb.Append(query.BuildQuery());
        
        var newLine = Environment.NewLine;

        foreach (var except in queries)
        {
            var exceptQuery = except.BuildQuery();
            sb.Append($"{newLine}EXCEPT ALL{newLine}");
            sb.Append(exceptQuery);
        }

        return sb.ToString();
    }
}

public enum UnionType
{
    Default,
    All
}