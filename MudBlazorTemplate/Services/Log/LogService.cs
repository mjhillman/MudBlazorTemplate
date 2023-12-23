using HITS.LIB.Sqlite;

namespace MudBlazorTemplate.Services
{
    public class LogService
    {
        public static async Task<List<LogModel>> GetLog(string connectionString)
        {
            using (SqlAsync dal = new SqlAsync())
            {
                string sql = $"SELECT * FROM Log WHERE MessageDate > '{DateTime.Now.AddMonths(-1)}' ORDER BY MessageDate DESC;";
                return await dal.GetDataTableAsListAsync<LogModel>(connectionString, sql).ConfigureAwait(true);
            }
        }

        public static async Task<int> InsertLogData(string connectionString, string message, string ip = "")
        {
            using (SqlAsync dal = new SqlAsync())
            {
                var parameters = new Dictionary<string, object> { { "$message", message } };
                string sql = $"INSERT INTO LOG (Ip, Message, MessageDate) VALUES ('{ip}', $message, '{DateTime.Now}')";
                return await dal.ExecuteNonQueryAsync(connectionString, sql, parameters).ConfigureAwait(true);
            }
        }

        public static async Task<int> DeleteLog(string connectionString)
        {
            using (SqlAsync dal = new SqlAsync())
            {
                string sql = $"DELETE FROM Log;";
                return await dal.ExecuteNonQueryAsync(connectionString, sql).ConfigureAwait(true);
            }
        }

        public LogService() { }

    }
}
