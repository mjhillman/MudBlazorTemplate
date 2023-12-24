using Microsoft.Data.Sqlite;
using System.Data;

namespace MudBlazorTemplate.Services
{
    public class LogService
    {
        public static async Task<List<LogModel>> GetLog()
        {
                string sql = $"SELECT * FROM Log WHERE MessageDate > '{DateTime.Now.AddMonths(-1)}' ORDER BY MessageDate DESC;";
                return await GetDataTableAsListAsync<LogModel>(Program.ConnectionString, sql).ConfigureAwait(true);
        }

        public static async Task<int> InsertLogData(string message, string ip = "")
        {                
                string sql = $"INSERT INTO LOG (Ip, Message, MessageDate) VALUES ('{ip}', '{message}', '{DateTime.Now}')";
                return await ExecuteNonQueryAsync(Program.ConnectionString, sql).ConfigureAwait(true);
        }

        public static async Task<int> DeleteLog()
        {
            string sql = $"DELETE FROM Log;";
            return await ExecuteNonQueryAsync(Program.ConnectionString, sql).ConfigureAwait(true);
        }

        /// <summary>
        /// This method will retrieve data from the database as a List.
        /// </summary>
        /// <typeparam name="T">your data type that corresponds to the database data</typeparam>
        /// <param name="connectionString">the database connection string</param>
        /// <param name="sql">the stored procedure name or SQL statement</param>
        /// <param name="parameters">optional: the parameter dictionary</param>
        /// <param name="timeout">query timeout in seconds</param>
        /// <returns>List</returns>
        private static async Task<List<T>> GetDataTableAsListAsync<T>(string connectionString, string sql) where T : new()
        {
            DataTable dt = null;
            SqliteConnection connection = null;

            try
            {
                connection = new SqliteConnection(connectionString);
                await connection?.OpenAsync();
                using (SqliteCommand cmd = new SqliteCommand(sql, connection))
                {
                    cmd.CommandTimeout = 20;
                    cmd.CommandType = CommandType.Text;

                    using (SqliteDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt = new DataTable();
                        dt.Load(reader, LoadOption.OverwriteChanges);
                    }
                }
                return dt == null ? new List<T>() : dt.Copy().ToList<T>();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (!dt.IsEmpty())
                {
                    if (dt.HasErrors)
                    {
                        // Get an array of all rows with errors.
                        DataRow[] rowsInError = dt.GetErrors();
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (!string.IsNullOrWhiteSpace(rowsInError[0].GetColumnError(column)))
                            {
                                errorMessage += $" {rowsInError[0].GetColumnError(column)} ";
                            }
                        }
                    }
                }
                throw new Exception(errorMessage);
            }
            finally
            {
                if (connection != null)
                {
                    if (connection?.State != ConnectionState.Closed) await connection?.CloseAsync();
                }
            }
        }

        /// <summary>
        /// This method will execute a SQL Stored Procedure.
        /// </summary>
        /// <param name="connectionString">the database connection string</param>
        /// <param name="sql">the stored procedure name or SQL statement</param>
        /// <param name="parameters">optional: the parameter dictionary</param>
        /// <param name="timeout">query timeout in seconds</param>
        /// <returns>the number of rows affected</returns>
        private static async Task<int> ExecuteNonQueryAsync(string connectionString, string sql)
        {
            int rowsAffected = 0;
            SqliteConnection connection = null;

            try
            {
                connection = new SqliteConnection(connectionString);
                await connection.OpenAsync();
                using (SqliteCommand cmd = new SqliteCommand(sql, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 20;
                    rowsAffected = await cmd?.ExecuteNonQueryAsync();
                }
                await connection.CloseAsync();
                return rowsAffected;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (connection != null)
                {
                    if (connection?.State != ConnectionState.Closed) await connection?.CloseAsync();
                }
            }
        }
    }
}
