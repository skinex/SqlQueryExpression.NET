namespace Ap.Tools.SqlQueryExpression.Tests;

[TestClass]
public class SqlQueryExpressionTests
{
    [TestMethod]
    public void BuildQuery_ColumnSet_True()
    {
        var query = new SqlQueryExpression(
            "Accounts",
            new ColumnSet(true),
            "a1"
        );

        var sqlQuery = query.BuildQuery();
        
        const string expectedQuery = "SELECT * FROM Accounts a1";

        Assert.AreEqual(expectedQuery, sqlQuery);
    }
    
    [TestMethod]
    public void BuildQuery_Top10()
    {
        var query = new SqlQueryExpression(
            "Accounts",
            new ColumnSet(true),
            "a1"
        );

        query.Top = 10;

        var sqlQuery = query.BuildQuery();
        
        const string expectedQuery = "SELECT TOP 10 * FROM Accounts a1";

        Assert.AreEqual(expectedQuery, sqlQuery);
    }
    
    [TestMethod]
    public void BuildQuery_BasicFilter()
    {
        var query = new SqlQueryExpression(
            "Accounts",
            new ColumnSet("Name", "Email"),
            "a1"
        );

        query.Filter.AddCondition(new ConditionExpression("Status", ConditionOperator.Equal, "Active"));

        var sqlQuery = query.BuildQuery();
        
        const string expectedQuery = "SELECT a1.Name, a1.Email FROM Accounts a1 WHERE (a1.Status = 'Active')";

        Assert.AreEqual(expectedQuery, sqlQuery);
    }

    [TestMethod]
    public void BuildQuery_BasicFilterWithPagination()
    {
        var query = new SqlQueryExpression(
            "Accounts",
            new ColumnSet("Name", "Email"),
            "a1"
        );

        query.Filter.AddCondition(new ConditionExpression("Status", ConditionOperator.Equal, "Active"));
        query.PageNumber = 2;
        query.PageSize = 10;

        var sqlQuery = query.BuildQuery();
        
        const string expectedQuery = "SELECT a1.Name, a1.Email FROM Accounts a1 WHERE (a1.Status = 'Active') OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY";

        Assert.AreEqual(expectedQuery, sqlQuery);
    }

    [TestMethod]
    public void BuildQuery_ComplexJoinWithFilters()
    {
        var query = new SqlQueryExpression(
            "Accounts",
            new ColumnSet("Name", "Email"),
            "a1"
        );

        query.Filter.AddCondition(new ConditionExpression("Status", ConditionOperator.Equal, "Active"));

        var linkEntity = new LinkTable(
            query.Alias,
            "AccountId",
            "Contacts",
            "AccountId",
            new ColumnSet("FirstName", "LastName"),
            JoinType.Inner,
            "c1"
        );

        linkEntity.LinkCriteria.AddCondition(new ConditionExpression("Status", ConditionOperator.Equal, "Active"));
        query.LinkTables.Add(linkEntity);

        var sqlQuery = query.BuildQuery();
        
        const string expectedQuery = "SELECT a1.Name, a1.Email, c1.FirstName, c1.LastName FROM Accounts a1 " +
                                     "INNER JOIN Contacts c1 ON a1.AccountId = c1.AccountId " +
                                     "WHERE (c1.Status = 'Active') AND (a1.Status = 'Active')";

        Assert.AreEqual(expectedQuery, sqlQuery);
    }

    [TestMethod]
    public void BuildQuery_ComplexJoinWithNestedJoinAndFilters()
    {
        var query = new SqlQueryExpression(
            "Accounts",
            new ColumnSet("Name", "Email"),
            "a1"
        );

        query.Filter.AddCondition(new ConditionExpression("Status", ConditionOperator.Equal, "Active"));

        var linkEntity = new LinkTable(
            "a1",
            "AccountId",
            "Contacts",
            "AccountId",
            new ColumnSet("FirstName", "LastName"),
            JoinType.Left,
            "c1"
        );

        linkEntity.LinkCriteria.AddCondition(new ConditionExpression("Status", ConditionOperator.Equal, "Active"));

        var nestedLinkEntity = new LinkTable(
            "c1",
            "ContactId",
            "Orders",
            "ContactId",
            new ColumnSet("OrderId", "TotalAmount"),
            JoinType.Inner,
            "o1"
        );

        nestedLinkEntity.LinkCriteria.AddCondition(new ConditionExpression("Status", ConditionOperator.Equal, "Completed"));

        linkEntity.LinkTables.Add(nestedLinkEntity);
        query.LinkTables.Add(linkEntity);

        var sqlQuery = query.BuildQuery();
        
        const string expectedQuery = "SELECT a1.Name, a1.Email, c1.FirstName, c1.LastName, o1.OrderId, o1.TotalAmount FROM Accounts a1 " +
                                     "LEFT JOIN Contacts c1 ON a1.AccountId = c1.AccountId " +
                                     "INNER JOIN Orders o1 ON c1.ContactId = o1.ContactId " +
                                     "WHERE (c1.Status = 'Active') AND (o1.Status = 'Completed') AND (a1.Status = 'Active')";

        Assert.AreEqual(expectedQuery, sqlQuery);
    }
    
    [TestMethod]
    public void BuildQuery_NestedFilterWithAndOrCondition()
    {
        var query = new SqlQueryExpression(
            "Accounts",
            new ColumnSet("Name", "Email"),
            "a1"
        );

        var filter = new FilterExpression(LogicalOperator.AND);
        filter.AddCondition(new ConditionExpression("Status", ConditionOperator.Equal, "Active"));

        var nestedFilter = new FilterExpression(LogicalOperator.OR);
        nestedFilter.AddCondition(new ConditionExpression("Type", ConditionOperator.Equal, "Customer"));
        nestedFilter.AddCondition(new ConditionExpression("Type", ConditionOperator.Equal, "Vendor"));

        filter.AddCondition(new ConditionExpression(nestedFilter));

        query.Filter = filter;

        var sqlQuery = query.BuildQuery();
        
        const string expectedQuery = "SELECT a1.Name, a1.Email FROM Accounts a1 " +
                                     "WHERE (a1.Status = 'Active' AND (a1.Type = 'Customer' OR a1.Type = 'Vendor'))";

        Assert.AreEqual(expectedQuery, sqlQuery);
    }
}