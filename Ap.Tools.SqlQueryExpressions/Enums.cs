namespace Ap.Tools.SqlQueryExpressions;

public enum ConditionOperator
{
    Equal,
    NotEqual,
    GreaterThan,
    LessThan,
    Like,
    In
}

public enum JoinType
{
    Inner,
    Left
}

public enum LogicalOperator
{
    AND,
    OR
}

public enum SortingType
{
    ASC,
    DESC
}

public enum UnionType
{
    Default,
    All
}