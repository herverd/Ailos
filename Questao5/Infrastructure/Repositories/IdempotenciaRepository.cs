using Dapper;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Interfaces;
using Questao5.Infrastructure.Sqlite;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Questao5.Infrastructure.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly SqliteConnectionFactory _connectionFactory;

        public IdempotenciaRepository(SqliteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Idempotencia> GetByIdAsync(Guid id)
        {
            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT chave_idempotencia, requisicao, resultado FROM idempotencia WHERE chave_idempotencia = @Id";
                return await connection.QueryFirstOrDefaultAsync<Idempotencia>(sql, new { Id = id.ToString().ToUpper() });
            }
        }

        public async Task AddAsync(Idempotencia idempotencia)
        {
            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado)
                            VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)";
                await connection.ExecuteAsync(sql, new
                {
                    ChaveIdempotencia = idempotencia.ChaveIdempotencia.ToString().ToUpper(),
                    idempotencia.Requisicao,
                    idempotencia.Resultado
                });
            }
        }

        public async Task UpdateAsync(Idempotencia idempotencia)
        {
            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE idempotencia SET requisicao = @Requisicao, resultado = @Resultado
                            WHERE chave_idempotencia = @ChaveIdempotencia";
                await connection.ExecuteAsync(sql, new
                {
                    idempotencia.Requisicao,
                    idempotencia.Resultado,
                    ChaveIdempotencia = idempotencia.ChaveIdempotencia.ToString().ToUpper()
                });
            }
        }
    }
}
