using System.Data.SqlClient;
using System.Text;
using Ap.Tools.SqlQueryExpressions;
using Ap.Tools.SqlQueryExpressions.Expressions;
using Ap.Tools.SqlQueryExpressions.Models;
using Ap.Tools.SqlQueryExpressions.Query;
using Ap.Tools.TSqlService.Abstractions.Contract;
using Ap.Tools.TSqlService.Abstractions.Models;

namespace Ap.Tools.TSqlService
{
    /// <summary>
    /// Sample of TSqlServiceClient implementation
    /// which calls SqlConnect extension methods
    /// to retrieve data
    /// </summary>
    public sealed class TSqlServiceClient : ITSqlService
    {
        private readonly SqlConnection _sqlConnection;

        public TSqlServiceClient(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public async Task<TSqlTable> RetrieveAsync<TPrimary>(string tableName, 
            PrimaryColumn<TPrimary> primaryColumn,
            ColumnSet columns)
        {
            var query = new SqlQueryExpression(tableName, columns)
            {
                Top = 1
            };
        
            query.Filter.AddCondition(new ConditionExpression(primaryColumn.ColumnName, ConditionOperator.Equal, primaryColumn.ColumnValue));
        
            var result = await RetrieveMultipleAsync(query);
            return result.FirstOrDefault();
        }

        public async Task<ICollection<TSqlTable>> RetrieveMultipleAsync(SqlQueryExpression query)
        {
            await _sqlConnection.OpenAsync();
            using var command = _sqlConnection.CreateCommand();
            using var reader = await command.ExecuteReaderAsync();

            var result = new List<TSqlTable>();

            while (await reader.ReadAsync())
            {
                var parsedTable = TSqlTable.From(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                result.Add(parsedTable);
            }

            await _sqlConnection.CloseAsync();

            return result;
        }

        public async Task<PrimaryColumn<T>> CreateAsync<T>(string tableName, 
            string primaryColumnName,
            TSqlTable data)
        {
            var columnsToInsert = string.Join(',', data.Keys);
            var valueParameters = data.Keys.ToArray().Select((s, index) => $"@p{index + 1}");
            var templatedValuesString = string.Join(',', valueParameters);
            var insertQuery = $"INSERT INTO {tableName} ({columnsToInsert}) VALUES ({templatedValuesString}) OUTPUT INSERTED.{primaryColumnName}";

            await _sqlConnection.OpenAsync();
            using var command = _sqlConnection.CreateCommand();
            command.CommandText = insertQuery;

            var sqlParameters = data.Values.ToArray()
                .Select((x, index) => new SqlParameter(parameterName: $"@p{index + 1}", value: x));
            
            command.Parameters.AddRange(sqlParameters.ToArray());

            var retValue = await command.ExecuteScalarAsync();

            await _sqlConnection.CloseAsync();

            return new PrimaryColumn<T>(primaryColumnName, (T)retValue);
        }

        public async Task UpdateAsync(string tableName,
            TSqlTable data)
        {
            var updateParameters = data.Keys.ToArray().Select((s, index) => $"{s}=@p{index + 1}");
            var updateParametersValue = string.Join(',', updateParameters);
            var updateQuery = $"UPDATE {tableName} SET ({updateParametersValue})";

            await _sqlConnection.OpenAsync();
            using var command = _sqlConnection.CreateCommand();
            command.CommandText = updateQuery;

            var sqlParameters = data.Values.ToArray()
                .Select((x, index) => new SqlParameter(parameterName: $"@p{index + 1}", value: x));
            
            command.Parameters.AddRange(sqlParameters.ToArray());

            var retValue = await command.ExecuteNonQueryAsync();

            await _sqlConnection.CloseAsync();
        }

        public async Task DeleteAsync<T>(string tableName, PrimaryColumn<T> primaryColumn)
        {
            var sb = new StringBuilder($"DELETE FROM {tableName} ");
            sb.Append($"WHERE {primaryColumn.ColumnName}=@p1");

            await _sqlConnection.OpenAsync();
            using var command = _sqlConnection.CreateCommand();
            command.CommandText = sb.ToString();
            command.Parameters.AddWithValue("@p1", primaryColumn.ColumnValue);

            await command.ExecuteNonQueryAsync();
            await _sqlConnection.CloseAsync();
        }
    }
}