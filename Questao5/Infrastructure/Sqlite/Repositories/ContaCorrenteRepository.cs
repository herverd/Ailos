using Dapper;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace Questao5.Infrastructure.Sqlite.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly ConnectionFactory _connectionFactory;

        public ContaCorrenteRepository(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ContaCorrente> GetByIdAsync(Guid id)
        {
            using (var connection = _connectionFactory.GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<ContaCorrente>(
                    "SELECT idcontacorrente AS IdContaCorrente, numero AS Numero, nome AS Nome, ativo AS Ativo FROM contacorrente WHERE idcontacorrente = @Id",
                    new { Id = id.ToString().ToUpper() }); 
            }
        }

        public async Task<ContaCorrente> GetByNumberAsync(int accountNumber)
        {
            using (var connection = _connectionFactory.GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<ContaCorrente>(
                    "SELECT idcontacorrente AS IdContaCorrente, numero AS Numero, nome AS Nome, ativo AS Ativo FROM contacorrente WHERE numero = @Numero",
                    new { Numero = accountNumber });
            }
        }
    }
}