using Ap.Tools.SqlQueryExpressions.Models;
using Ap.Tools.SqlQueryExpressions.Query;
using Ap.Tools.TSqlService.Abstractions.Models;

namespace Ap.Tools.TSqlService.Abstractions.Contract;

/// <summary>
/// An interface which defines
/// a contract for SqlClients to query data
/// via SqlQueryExpressions
/// </summary>
public interface ITSqlService
{
    Task<TSqlTable> RetrieveAsync<TPrimary>(string tableName, 
        PrimaryAttribute<TPrimary> primaryAttribute,
        ColumnSet columns);

    Task<IEnumerable<TSqlTable>> RetrieveMultipleAsync(SqlQueryExpression query);
}