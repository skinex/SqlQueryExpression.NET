namespace Ap.Tools.SqlQueryExpression;

using System.Collections.Generic;
using System.Text;

/// <summary>
/// Class that holds information about
/// query including column set,
/// filters, orders and link tables and provide a method
/// to build sql based on current expression for MSSQL db
/// <remarks>
/// This class is inspired by Microsoft.Xrm.Sdk.Query
/// https://learn.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.query.queryexpression?view=dataverse-sdk-latest
/// </remarks>
/// </summary>
public sealed class SqlQueryExpression
{
    public string TableName { get; }
    public string Alias { get; }
    public ColumnSet ColumnSet { get; }
    public FilterExpression Filter { get; set; } = new FilterExpression();
    public List<OrderExpression> SortingOrders { get; } = new List<OrderExpression>();
    public List<LinkTable> LinkTables { get; } = new List<LinkTable>();
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public int? Top { get; set; }

    public SqlQueryExpression(string tableName, ColumnSet columnSet, string alias = null)
    {
        TableName = tableName;
        ColumnSet = columnSet;
        Alias = alias ?? tableName;
    }

    /// <summary>
    /// Method to build query based on expression
    /// perhaps would be best to make it virtual as
    /// it currently bounded to T-SQL
    /// </summary>
    /// <returns></returns>
    public string BuildQuery()
    {
        var sb = new StringBuilder();
        var whereClauseBuilder = new StringBuilder();

        var selectClauseBuilder = ColumnSet.BuildColumnSet(Alias);

        var linkTables = LinkTables
            .Select(i => i.BuildLinkTable(selectClauseBuilder, whereClauseBuilder))
            .ToList();

        sb.Append("SELECT ");
        
        if (Top.HasValue)
        {
            sb.Append($"TOP {Top} ");
        }
        
        sb.Append(ColumnSet.IncludeAllColumns ? '*' : selectClauseBuilder);
        sb.Append(" FROM ");
        sb.Append($"{TableName} {Alias}");

        foreach (var linkTable in linkTables)
        {
            sb.Append(' ');
            sb.Append(linkTable);
        }

        if (Filter.Conditions.Count > 0)
        {
            if (whereClauseBuilder.Length > 0)
            {
                whereClauseBuilder.Append(" AND ");
            }
            
            whereClauseBuilder.Append(Filter.BuildFilter(Alias));
        }

        if (whereClauseBuilder.Length > 0)
        {
            sb.Append(" WHERE ");
            sb.Append(whereClauseBuilder);
        }

        if (SortingOrders.Count > 0)
        {
            sb.Append(" ORDER BY ");
            
            for (var i = 0; i < SortingOrders.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(SortingOrders[i].BuildOrder(Alias));
            }
        }

        if (PageNumber.HasValue && PageSize.HasValue)
        {
            var offset = (PageNumber.Value - 1) * PageSize.Value;
            sb.Append($" OFFSET {offset} ROWS FETCH NEXT {PageSize.Value} ROWS ONLY");
        }

        return sb.ToString();
    }
}
    

