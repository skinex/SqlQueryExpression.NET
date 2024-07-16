using Ap.Tools.SqlQueryExpressions.Expressions;
using Ap.Tools.SqlQueryExpressions.Models;

namespace Ap.Tools.SqlQueryExpressions.Query.FluentQueries;

public sealed class FluentSqlQueryExpression
{
    internal readonly SqlQueryExpression InnerQuery;

    private FluentSqlQueryExpression(string tableName)
    {
        InnerQuery = new SqlQueryExpression(tableName);
    }

    public static FluentSqlQueryExpression QueryTable(string tableName)
    {
        return new FluentSqlQueryExpression(tableName);
    }
    
    public static FluentSqlQueryExpression QueryTable(string tableName, string alias)
    {
        return new FluentSqlQueryExpression(tableName).WithAlias(alias);
    }

    public FluentSqlQueryExpression WithAlias(string alias)
    {
        InnerQuery.Alias = alias;
        return this;
    }

    public FluentSqlQueryExpression WithTopCount(int top)
    {
        InnerQuery.Top = top;
        return this;
    }

    public FluentSqlQueryExpression WithPageNumber(int pageNumber)
    {
        InnerQuery.PageNumber = pageNumber;
        return this;
    }

    public FluentSqlQueryExpression WithPageSize(int pageSize)
    {
        InnerQuery.PageSize = pageSize;
        return this;
    }

    public FluentSqlQueryExpression Select(ColumnSet columnSet)
    {
        InnerQuery.ColumnSet.IncludeAllColumns = columnSet.IncludeAllColumns;
        InnerQuery.ColumnSet.AddRange(columnSet.Columns.ToArray());
        return this;
    }
    
    public FluentSqlQueryExpression Select(params string[] columns)
    {
        InnerQuery.ColumnSet.AddRange(columns);
        return this;
    }
    
    public FluentSqlQueryExpression Select(bool includeAllColumns, params string[] columns)
    {
        InnerQuery.ColumnSet.IncludeAllColumns = includeAllColumns;
        InnerQuery.ColumnSet.AddRange(columns);
        return this;
    }

    public FluentSqlQueryExpression Where(ConditionExpression condition)
    {
        InnerQuery.Filter.AddCondition(condition);
        return this;
    }

    public FluentSqlQueryExpression Where(FilterExpression nestedFilter)
    {
        InnerQuery.Filter.AddCondition(new ConditionExpression(nestedFilter));
        return this;
    }

    public FluentSqlQueryExpression OrderBy(string columnName)
    {
        InnerQuery.SortingOrders.Add(new OrderExpression(columnName, SortingType.ASC));
        return this;
    }

    public FluentSqlQueryExpression OrderByDescending(string columnName)
    {
        InnerQuery.SortingOrders.Add(new OrderExpression(columnName, SortingType.DESC));
        return this;
    }

    public FluentJoinSqlQueryExpression Join(string tableName, 
        string fromColumnName, 
        string toColumnName, 
        string joinAlias = null,
        JoinType joinType = JoinType.Left)
    {
        return FluentJoinSqlQueryExpression.CreateJoin(this, joinType, InnerQuery.Alias, tableName,
            fromColumnName, toColumnName, joinAlias ?? tableName);
    }

    public SqlQueryExpression BuildExpression() => InnerQuery;
    public string BuildSql() => InnerQuery.BuildQuery();
}