using System.Collections.Generic;
using System.Text;
using Ap.Tools.SqlQueryExpressions.Models;

namespace Ap.Tools.SqlQueryExpressions.Expressions;

public class LinkTableExpression
{
    public string LinkFromTableAlias { get; init; }
    public string LinkFromColumnName { get; init; }
    public string LinkToTableName { get; init; }
    public string LinkToColumnName { get; init; }
    public ColumnSet Columns { get; init; }
    public FilterExpression LinkCriteria { get; init; } = new();
    public List<LinkTableExpression> LinkTables { get; init; } = new();
    public JoinType JoinType { get; init; }
    public string Alias { get; init; }

    /// <summary>
    /// public ctor for property initialization
    /// </summary>
    public LinkTableExpression()
    {
    }

    public LinkTableExpression(string linkFromTableAlias, 
        string linkFromColumnName, 
        string linkToTableName, 
        string linkToColumnName, 
        ColumnSet columns, 
        JoinType joinType = JoinType.Left, 
        string alias = null)
    {
        LinkFromTableAlias = linkFromTableAlias;
        LinkFromColumnName = linkFromColumnName;
        LinkToTableName = linkToTableName;
        LinkToColumnName = linkToColumnName;
        Columns = columns;
        JoinType = joinType;
        Alias = alias ?? linkToTableName;
    }

    internal string BuildLinkTable(StringBuilder selectClauseBuilder, StringBuilder whereClauseBuilder)
    {
        var joinTypeString = JoinType == JoinType.Inner ? "INNER JOIN" : "LEFT JOIN";
        var linkAlias = Alias ?? LinkToTableName;
        
        var sb = new StringBuilder();
        sb.Append($"{joinTypeString} {LinkToTableName} {linkAlias} ON {LinkFromTableAlias}.{LinkFromColumnName} = {linkAlias}.{LinkToColumnName}");

        if (LinkCriteria.Conditions.Count > 0)
        {
            if (whereClauseBuilder.Length > 0)
            {
                whereClauseBuilder.Append(" AND ");
            }
            
            whereClauseBuilder.Append(LinkCriteria.BuildFilter(linkAlias));
        }

        if (Columns.Columns.Count != 0)
        {
            selectClauseBuilder.Append(", ");
            selectClauseBuilder.Append(Columns.BuildColumnSet(Alias));
        }

        foreach (var linkTable in LinkTables)
        {
            sb.Append(' ');
            sb.Append(linkTable.BuildLinkTable(selectClauseBuilder, whereClauseBuilder));
        }
        
        return sb.ToString();
    }
}