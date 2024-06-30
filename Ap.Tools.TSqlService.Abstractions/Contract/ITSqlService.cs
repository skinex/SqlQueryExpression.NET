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
        PrimaryColumn<TPrimary> primaryColumn,
        ColumnSet columns);

    Task<ICollection<TSqlTable>> RetrieveMultipleAsync(SqlQueryExpression query);

    Task<PrimaryColumn<T>> CreateAsync<T>(string tableName, 
        string primaryColumnName,
        TSqlTable data);

    Task UpdateAsync(string tableName,
        TSqlTable data);

    Task DeleteAsync<T>(string tableName, PrimaryColumn<T> primaryColumn);
}