using System.Data;
using System.Data.SqlClient;
using Ap.Tools.SqlQueryExpressions;
using Ap.Tools.SqlQueryExpressions.Expressions;
using Ap.Tools.SqlQueryExpressions.Models;
using Ap.Tools.SqlQueryExpressions.Query;
using Ap.Tools.TSqlService.Abstractions.Contract;
using Ap.Tools.TSqlService.Abstractions.Extensions;
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
            PrimaryAttribute<TPrimary> primaryAttribute,
            ColumnSet columns)
        {
            var query = new SqlQueryExpression(tableName, columns)
            {
                Top = 1
            };
        
            query.Filter.AddCondition(new ConditionExpression(primaryAttribute.AttributeName, ConditionOperator.Equal, primaryAttribute.AttributeValue));
        
            var result = await RetrieveMultipleAsync(query);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<TSqlTable>> RetrieveMultipleAsync(SqlQueryExpression query)
        {
            var dt = await _sqlConnection.ExecuteQueryAsync(query);
        
            return dt.AsEnumerable().Select(x => new TSqlTable
            {
                [x.Field<string>(0)] = x.Field<object>(1)
            }).ToList();
        }
    }
}