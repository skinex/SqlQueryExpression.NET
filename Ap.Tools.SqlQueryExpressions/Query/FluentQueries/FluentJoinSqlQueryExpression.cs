using System;
using Ap.Tools.SqlQueryExpressions.Expressions;

namespace Ap.Tools.SqlQueryExpressions.Query.FluentQueries;

public sealed class FluentJoinSqlQueryExpression
{
    private readonly FluentSqlQueryExpression _parentQuery;
    private readonly LinkTableExpression _linkTable;

    private readonly bool _isSubjoin = false;
    private readonly FluentJoinSqlQueryExpression _parentJoin = null;

    private FluentJoinSqlQueryExpression(FluentSqlQueryExpression parentQuery)
    {
        _parentQuery = parentQuery;
        _linkTable = new LinkTableExpression();
    }
    
    private FluentJoinSqlQueryExpression(FluentSqlQueryExpression parentQuery, FluentJoinSqlQueryExpression parentJoin)
    {
        _parentQuery = parentQuery;
        _parentJoin = parentJoin;
        _isSubjoin = true;
        _linkTable = new LinkTableExpression();
    }

    internal static FluentJoinSqlQueryExpression CreateJoin(FluentSqlQueryExpression queryExpression,
        JoinType joinType,
        string sourceTableAlias, string targetTableName,
        string fromColumnName, string toColumnName,
        string joinAlias)
    {
        var expression = new FluentJoinSqlQueryExpression(queryExpression)
        {
            _linkTable = 
            {
                JoinType = joinType,
                LinkFromTableAlias = sourceTableAlias,
                LinkToTableName = targetTableName,
                LinkFromColumnName = fromColumnName,
                LinkToColumnName = toColumnName,
                Alias = joinAlias
            }
        };

        return expression;
    }
    
    internal static FluentJoinSqlQueryExpression CreateSubJoin(FluentSqlQueryExpression queryExpression,
        FluentJoinSqlQueryExpression parentJoin,
        JoinType joinType,
        string sourceTableAlias, string targetTableName,
        string fromColumnName, string toColumnName,
        string joinAlias)
    {
        var expression = new FluentJoinSqlQueryExpression(queryExpression, parentJoin)
        {
            _linkTable = 
            {
                JoinType = joinType,
                LinkFromTableAlias = sourceTableAlias,
                LinkToTableName = targetTableName,
                LinkFromColumnName = fromColumnName,
                LinkToColumnName = toColumnName,
                Alias = joinAlias
            }
        };

        return expression;
    }

    public FluentJoinSqlQueryExpression Select(params string[] columns)
    {
        _linkTable.Columns.AddRange(columns);
        return this;
    }
    
    public FluentJoinSqlQueryExpression Select(bool includeAllColumns, params string[] columns)
    {
        _linkTable.Columns.IncludeAllColumns = includeAllColumns;
        _linkTable.Columns.AddRange(columns);
        return this;
    }
    
    public FluentJoinSqlQueryExpression Where(ConditionExpression condition)
    {
        _linkTable.LinkCriteria.AddCondition(condition);
        return this;
    }

    public FluentJoinSqlQueryExpression Where(FilterExpression nestedFilter)
    {
        _linkTable.LinkCriteria.AddCondition(new ConditionExpression(nestedFilter));
        return this;
    }

    public FluentJoinSqlQueryExpression SubJoin(
        JoinType joinType, string targetTableName,
        string fromColumnName, string toColumnName,
        string joinAlias)
    {
        return CreateSubJoin(_parentQuery, this, joinType, _linkTable.Alias, targetTableName, fromColumnName, toColumnName,
            joinAlias);
    }

    public FluentJoinSqlQueryExpression CloseSubJoin()
    {
        if (_parentJoin is null)
        {
            throw new InvalidOperationException("Parent join is not defined");
        }
        
        var innerQuery = _parentQuery.InnerQuery;
        innerQuery.LinkTables.Add(_linkTable);
        return _parentJoin;
    }

    public FluentSqlQueryExpression CloseJoin()
    {
        var innerQuery = _parentQuery.InnerQuery;
        innerQuery.LinkTables.Add(_linkTable);
        return _parentQuery;
    }
}