using Dapper;
using Microsoft.Data.Sqlite;

namespace GesN.Web.Data.Migrations
{
    public class DbInit
    {
        private readonly string _connectionString;

        public DbInit(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Initialize()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var createClienteTable = @"
                CREATE TABLE IF NOT EXISTS Cliente (
                    ClienteId INTEGER NOT NULL UNIQUE,
                    ClienteIdGoogleContacts BLOB,
                    DataCadastro TEXT NOT NULL,
                    Nome TEXT NOT NULL,
                    Sobrenome TEXT NOT NULL,
                    Cpf TEXT,
                    TelefonePrincipal INTEGER NOT NULL,
                    DataModificacao TEXT NOT NULL,
                    PRIMARY KEY(ClienteId AUTOINCREMENT)
                );";

                var createPedidoTable = @"
                CREATE TABLE IF NOT EXISTS Pedido (
                    PedidoId INTEGER NOT NULL UNIQUE,
                    ClienteId INTEGER NOT NULL,
	                DataCadastro TEXT NOT NULL,
	                DataPedido TEXT NOT NULL,
	                DataModificacao	TEXT NOT NULL,
	                PRIMARY KEY(PedidoId AUTOINCREMENT)
                );";

                connection.Execute(createClienteTable);
                connection.Execute(createPedidoTable);
            }
        }
    }
}
