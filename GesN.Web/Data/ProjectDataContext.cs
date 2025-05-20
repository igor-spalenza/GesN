using Dapper;
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
                connection.Execute("PRAGMA foreign_keys = ON;");
                return connection;
            }
        }
    }
}
