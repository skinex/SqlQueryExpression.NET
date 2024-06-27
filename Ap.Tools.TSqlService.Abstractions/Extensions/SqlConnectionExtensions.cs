using System.Data;
using System.Data.SqlClient;
using Ap.Tools.SqlQueryExpressions.Query;

namespace Ap.Tools.TSqlService.Abstractions.Extensions
{
    /// <summary>
    /// Extensions for sql connection object
    /// to run query based on given query expression
    /// </summary>
    public static class SqlConnectionExtensions
    {
        /// <summary>
        /// Run query expression asynchronously and return datatable as a result
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<DataTable> ExecuteQueryAsync(this SqlConnection sqlConnection, 
            SqlQueryExpression query, 
            CancellationToken cancellationToken = default)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                await sqlConnection.OpenAsync(cancellationToken);
            }
        
            var sql = query.BuildQuery();
            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = sql;

            var result = new DataTable();

            var dataReader = await sqlCommand.ExecuteReaderAsync(cancellationToken);
            result.Load(dataReader);

            await sqlConnection.CloseAsync();

            return result;
        }

        /// <summary>
        /// Run query expression asynchronously and return specific type
        /// build by mapping func
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="query"></param>
        /// <param name="mappingFunc"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> ExecuteQueryAsync<T>(this SqlConnection sqlConnection, 
            SqlQueryExpressions.Query.SqlQueryExpression query, 
            Func<IDataReader, T> mappingFunc, 
            CancellationToken cancellationToken = default)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                await sqlConnection.OpenAsync(cancellationToken);
            }
        
            var sql = query.BuildQuery();
            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = sql;

            var result = new List<T>();

            var dataReader = await sqlCommand.ExecuteReaderAsync(cancellationToken);
        
            while (await dataReader.ReadAsync(cancellationToken))
            {
                result.Add(mappingFunc(dataReader));
            }

            await sqlConnection.CloseAsync();

            return result;
        }
    
        /// <summary>
        /// Run query expression synchronously and return datatable as a result
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static DataTable ExecuteQuery(this SqlConnection sqlConnection, SqlQueryExpressions.Query.SqlQueryExpression query)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
        
            var sql = query.BuildQuery();
            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = sql;

            var result = new DataTable();

            var dataReader = sqlCommand.ExecuteReader();
            result.Load(dataReader);

            sqlConnection.Close();

            return result;
        }
    
        /// <summary>
        /// Run query expression synchronously and return specific type
        /// build by mapping func
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="query"></param>
        /// <param name="mappingFunc"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExecuteQuery<T>(this SqlConnection sqlConnection, 
            SqlQueryExpressions.Query.SqlQueryExpression query, 
            Func<IDataReader, T> mappingFunc)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
        
            var sql = query.BuildQuery();
            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = sql;

            var result = new List<T>();

            var dataReader = sqlCommand.ExecuteReader();
        
            while (dataReader.Read())
            {
                result.Add(mappingFunc(dataReader));
            }

            sqlConnection.Close();

            return result;
        }
    }
}