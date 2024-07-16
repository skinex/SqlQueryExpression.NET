using Ap.Tools.SqlQueryExpressions.Expressions;
using Ap.Tools.SqlQueryExpressions.Query;
using Ap.Tools.SqlQueryExpressions.Query.FluentQueries;

namespace Ap.Tools.SqlQueryExpressions.Tests;

[TestClass]
public class FluentSqlExpressionTests
{
    [TestMethod]
    public void BuildQuery_ColumnSet_True()
    {
        var query = FluentSqlQueryExpression.QueryTable("Accounts")
            .Select(true)
            .WithAlias("a1");

        var sqlQuery = query.BuildSql();
        
        const string expectedQuery = "SELECT * FROM Accounts a1";

        Assert.AreEqual(expectedQuery, sqlQuery);
    }
    
    [TestMethod]
    public void BuildQuery_Top10()
    {
        var query = FluentSqlQueryExpression.QueryTable("Accounts")
            .Select(true)
            .WithTopCount(10)
            .WithAlias("a1");
        
        var sqlQuery = query.BuildSql();
        
        const string expectedQuery = "SELECT TOP 10 * FROM Accounts a1";

        Assert.AreEqual(expectedQuery, sqlQuery);
    }
    
    [TestMethod]
    public void BuildQuery_BasicFilter()
    {
        var query = FluentSqlQueryExpression.QueryTable("Accounts")
            .Select("Name", "Email")
            .Where(new ConditionExpression("Status", ConditionOperator.Equal, "Active"))
            .WithAlias("a1");

        var sqlQuery = query.BuildSql();
        
        const string expectedQuery = "SELECT a1.Name, a1.Email FROM Accounts a1 WHERE (a1.Status = 'Active')";

        Assert.AreEqual(expectedQuery, sqlQuery);
    }
    
    [TestMethod]
    public void BuildQuery_BasicFilterWithPagination()
    {
        var query = FluentSqlQueryExpression.QueryTable("Accounts")
            .Select("Name", "Email")
            .Where(new ConditionExpression("Status", ConditionOperator.Equal, "Active"))
            .WithPageNumber(2)
            .WithPageSize(10)
            .WithAlias("a1");
        
        var sqlQuery = query.BuildSql();
        
        const string expectedQuery = "SELECT a1.Name, a1.Email FROM Accounts a1 WHERE (a1.Status = 'Active') OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY";

        Assert.AreEqual(expectedQuery, sqlQuery);
    }
    
    [TestMethod]
    public void BuildQuery_NestedFilterWithAndOrCondition()
    {
        var query = FluentSqlQueryExpression.QueryTable("Accounts")
            .WithAlias("a1")
            .Select("Name", "Email")
            .Where(new ConditionExpression("Status", ConditionOperator.Equal, "Active"))
            .Where(new FilterExpression(LogicalOperator.OR)
            {
                Conditions =
                {
                    new ConditionExpression("Type", ConditionOperator.Equal, "Customer"),
                    new ConditionExpression("Type", ConditionOperator.Equal, "Vendor")
                }
            });

        var sqlQuery = query.BuildSql();
        
        const string expectedQuery = "SELECT a1.Name, a1.Email FROM Accounts a1 " +
                                     "WHERE (a1.Status = 'Active' AND (a1.Type = 'Customer' OR a1.Type = 'Vendor'))";

        Assert.AreEqual(expectedQuery, sqlQuery);
    }
    
    [TestMethod]
    public void BuildQuery_ComplexJoinWithFilters()
    {
        var query = FluentSqlQueryExpression.QueryTable("Accounts")
            .WithAlias("a1")
            .Select("Name", "Email")
            .Where(new ConditionExpression("Status", ConditionOperator.Equal, "Active"))
            .Join("Contacts", "AccountId", "AccountId", "c1", JoinType.Inner)
                .Select("FirstName", "LastName")
                .Where(new ConditionExpression("Status", ConditionOperator.Equal, "Active"))
            .CloseJoin();

        var sqlQuery = query.BuildSql();
        
        const string expectedQuery = "SELECT a1.Name, a1.Email, c1.FirstName, c1.LastName FROM Accounts a1 " +
                                     "INNER JOIN Contacts c1 ON a1.AccountId = c1.AccountId " +
                                     "WHERE (c1.Status = 'Active') AND (a1.Status = 'Active')";

        Assert.AreEqual(expectedQuery, sqlQuery);
    }
    
    [TestMethod]
    public void BuildQuery_ComplexJoinWithNestedJoinAndFilters()
    {
        var query = FluentSqlQueryExpression.QueryTable("Accounts", alias:"a1")
            .Select("Name", "Email")
            .Where(new ConditionExpression("Status", ConditionOperator.Equal, "Active"))
            .Join("Contacts", "AccountId", "AccountId", "c1", JoinType.Left)
            .Select("FirstName", "LastName")
            .Where(new ConditionExpression("Status", ConditionOperator.Equal, "Active"))
                .SubJoin(JoinType.Inner, "Orders", "ContactId", "ContactId", "o1")
                    .Select("OrderId", "TotalAmount")
                    .Where(new ConditionExpression("Status", ConditionOperator.Equal, "Completed"))
                .CloseSubJoin()
            .CloseJoin();

        var sqlQuery = query.BuildSql();
        
        const string expectedQuery = "SELECT a1.Name, a1.Email, o1.OrderId, o1.TotalAmount, c1.FirstName, c1.LastName FROM Accounts a1 " +
                                     "INNER JOIN Orders o1 ON c1.ContactId = o1.ContactId " +
                                     "LEFT JOIN Contacts c1 ON a1.AccountId = c1.AccountId " +
                                     "WHERE (o1.Status = 'Completed') AND (c1.Status = 'Active') AND (a1.Status = 'Active')";

        Assert.AreEqual(expectedQuery, sqlQuery);
    }
}