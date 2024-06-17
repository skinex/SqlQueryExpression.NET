# SqlQueryExpression.NET
An object oriented SQL Query Expression library which allows building simple and testable SQL queries in code

This library is inspired by Microsoft.Xrm.Sdk.Query
https://learn.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.query.queryexpression?view=dataverse-sdk-latest

It provides object oriented way of building T-SQL queries in .NET applications.

# Supported queries

Currently library supports simple SQL queries, which includes simple selects, where conditions, joins (INNER and LEFT), union and except all.

It's also possible to setup an OFFSET and FETCH NEXT ROWS as well as TOP count.

To lookup details on how to construct queries with SqlQueryExpression look into the Samples and Microsoft.Xrm.Sdk.Query docs.

<b>Current implementation only supports T-SQL generation (only suitable for MSSQL database)</b>

Extensions to execute query via SqlConnection also provided (async and sync versions)

# Samples

<b>Simple SELECT QUERY</b>
```c#
var query = new SqlQueryExpression("Accounts",new ColumnSet(true),"a1");
-----
SQL - SELECT * FROM Accounts a1
```

<b>Simple SELECT QUERY WITH OFFSET and FETCH NEXT ROWS:</b>
```c#
var query = new SqlQueryExpression("Accounts", new ColumnSet("Name", "Email"), "a1");
query.Filter.AddCondition(new ConditionExpression("Status", ConditionOperator.Equal, "Active"));
query.PageNumber = 2;
query.PageSize = 10;
------
SQL - "SELECT a1.Name, a1.Email FROM Accounts a1 WHERE (a1.Status = 'Active') OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY";
```

<b>SELECT QUERY with Nested Filters:</b>
```c#
var query = new SqlQueryExpression("Accounts", new ColumnSet("Name", "Email"), "a1");
var filter = new FilterExpression(LogicalOperator.AND);
filter.AddCondition(new ConditionExpression("Status", ConditionOperator.Equal, "Active"));
var nestedFilter = new FilterExpression(LogicalOperator.OR);
nestedFilter.AddCondition(new ConditionExpression("Type", ConditionOperator.Equal, "Customer"));
nestedFilter.AddCondition(new ConditionExpression("Type", ConditionOperator.Equal, "Vendor"));
filter.AddCondition(new ConditionExpression(nestedFilter));
query.Filter = filter;
------
SQL - SELECT a1.Name, a1.Email FROM Accounts a1
      WHERE (a1.Status = 'Active' AND (a1.Type = 'Customer' OR a1.Type = 'Vendor'))     
```
<b>SELECT QUERY with Nested Joins:</b>

```c#
var query = new SqlQueryExpression("Accounts", new ColumnSet("Name", "Email"), "a1");

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
------
SQL - SELECT a1.Name, a1.Email, c1.FirstName, c1.LastName, o1.OrderId, o1.TotalAmount FROM Accounts a1
      LEFT JOIN Contacts c1 ON a1.AccountId = c1.AccountId 
      INNER JOIN Orders o1 ON c1.ContactId = o1.ContactId 
      WHERE (c1.Status = 'Active') AND (o1.Status = 'Completed') AND (a1.Status = 'Active')
```
You can find more examples in Ap.Tools.SqlQueryExpression.Tests project

# Not supported queries

- GROUP BY (and all aggregations)
- HAVING
- CTE (WITH)
- Inner queries
- INTERSECT
- CROSS JOIN
- APPLY (CROSS and OUTER)
- PIVOT
- QUERY HINTS
- WHERE IN

<i> Feel free to contribute towards implementation of any listed queries :) </i>