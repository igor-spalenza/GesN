using Microsoft.Data.Sqlite;
using System.Data;

namespace GesN.Web.Data
{
    public class ProjectDataContext
    {
        private readonly string _connectionString;

        public ProjectDataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Connection
        {
            get
            {
                var connection = new SqliteConnection(_connectionString);
                connection.Open();
                return connection;
            }
        }
    }
}
