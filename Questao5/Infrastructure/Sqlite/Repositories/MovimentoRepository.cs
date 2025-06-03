using Dapper;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Questao5.Infrastructure.Sqlite.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly ConnectionFactory _connectionFactory;

        public MovimentoRepository(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(Movimento movimento)
        {
            using (var connection = _connectionFactory.GetOpenConnection())
            {
                var sql = "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";
                await connection.ExecuteAsync(sql, new
                {
                    IdMovimento = movimento.IdMovimento.ToString().ToUpper(),
                    IdContaCorrente = movimento.IdContaCorrente.ToString().ToUpper(),
                    movimento.DataMovimento,
                    movimento.TipoMovimento,
                    movimento.Valor
                });
            }
        }

        public async Task<IEnumerable<Movimento>> GetMovimentosByContaCorrenteIdAsync(Guid idContaCorrente)
        {
            using (var connection = _connectionFactory.GetOpenConnection())
            {
                var sql = "SELECT idmovimento AS IdMovimento, idcontacorrente AS IdContaCorrente, datamovimento AS DataMovimento, tipomovimento AS TipoMovimento, valor AS Valor FROM movimento WHERE idcontacorrente = @IdContaCorrente";
                return await connection.QueryAsync<Movimento>(sql, new { IdContaCorrente = idContaCorrente.ToString().ToUpper() });
            }
        }
    }
}