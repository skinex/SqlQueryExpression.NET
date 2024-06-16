using System.Text;

namespace Ap.Tools.SqlQueryExpression.Extensions;

public static class SqlQueryExpressionExtensions
{
    public static string Union(this SqlQueryExpression query, 
        UnionType unionType = UnionType.Default, 
        params SqlQueryExpression[] unions)
    {
        var sb = new StringBuilder();

        sb.Append(query.BuildQuery());

        var unionStatement = unionType == UnionType.Default ? "\nUNION\n" : "\nUNION ALL\n";

        foreach (var union in unions)
        {
            var unionQuery = union.BuildQuery();
            sb.Append(unionStatement);
            sb.Append(unionQuery);
        }

        return sb.ToString();
    }
    
    public static string ExceptAll(this SqlQueryExpression query, 
        params SqlQueryExpression[] unions)
    {
        var sb = new StringBuilder();

        sb.Append(query.BuildQuery());

        foreach (var union in unions)
        {
            var unionQuery = union.BuildQuery();
            sb.Append("\nEXCEPT ALL\n");
            sb.Append(unionQuery);
        }

        return sb.ToString();
    }
}

public enum UnionType
{
    Default,
    All
}