# SqlQueryExpression.NET
An object oriented SQL Query Expression library which allows building simple and testable SQL queries in code

This library is inspired by Microsoft.Xrm.Sdk.Query
https://learn.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.query.queryexpression?view=dataverse-sdk-latest

It provides object oriented way of building T-SQL queries in .NET applications.

# Supported queries

Currently library supports simple SQL queries, which includes simple selects, where conditions, joins (INNER and LEFT), union and except all.

It's also possible to setup an OFFSET and FETCH NEXT ROWS as well as TOP count.

To lookup details on how to construct queries with SqlQueryExpression lookup project unit tests and Microsoft.Xrm.Sdk.Query docs.

<b>Current implementation only supports T-SQL generation (only suitable for MSSQL database)</b>

Extensions to execute query via SqlConnection also provided (async and sync versions)

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