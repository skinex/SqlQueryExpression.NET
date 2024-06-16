using System.Text;

namespace Ap.Tools.SqlQueryExpression;

public class LinkTable
{
    public string LinkFromTableAlias { get; }
    public string LinkFromAttributeName { get; }
    public string LinkToTableName { get; }
    public string LinkToAttributeName { get; }
    public ColumnSet Columns { get; }
    public FilterExpression LinkCriteria { get; } = new();
    public List<LinkTable> LinkTables { get; } = new();
    public JoinType JoinType { get; }
    public string Alias { get; }

    public LinkTable(string linkFromTableAlias, 
        string linkFromAttributeName, 
        string linkToTableName, 
        string linkToAttributeName, 
        ColumnSet columns, 
        JoinType joinType = JoinType.Left, 
        string alias = null)
    {
        LinkFromTableAlias = linkFromTableAlias;
        LinkFromAttributeName = linkFromAttributeName;
        LinkToTableName = linkToTableName;
        LinkToAttributeName = linkToAttributeName;
        Columns = columns;
        JoinType = joinType;
        Alias = alias ?? linkToTableName;
    }

    internal string BuildLinkTable(StringBuilder selectClauseBuilder, StringBuilder whereClauseBuilder)
    {
        var joinTypeString = JoinType == JoinType.Inner ? "INNER JOIN" : "LEFT JOIN";
        var linkAlias = Alias ?? LinkToTableName;
        
        var sb = new StringBuilder();
        sb.Append($"{joinTypeString} {LinkToTableName} {linkAlias} ON {LinkFromTableAlias}.{LinkFromAttributeName} = {linkAlias}.{LinkToAttributeName}");

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