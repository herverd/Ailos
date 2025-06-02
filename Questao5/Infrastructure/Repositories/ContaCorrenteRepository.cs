using Dapper;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Interfaces;
using Questao5.Infrastructure.Sqlite;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Questao5.Infrastructure.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly SqliteConnectionFactory _connectionFactory;

        public ContaCorrenteRepository(SqliteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ContaCorrente> GetByIdAsync(Guid id)
        {
            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT idcontacorrente, numero, nome, ativo FROM contacorrente WHERE idcontacorrente = @Id";
                return await connection.QueryFirstOrDefaultAsync<ContaCorrente>(sql, new { Id = id.ToString().ToUpper() });
            }
        }
    }
}


