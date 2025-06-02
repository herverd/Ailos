using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;


namespace Questao5.Infrastructure.Sqlite
{
    public class SqliteConnectionFactory
    {
        private readonly string _connectionString;

        public SqliteConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }

}
