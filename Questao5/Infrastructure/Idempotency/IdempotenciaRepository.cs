using Dapper;
using Questao5.Infrastructure.Sqlite;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Questao5.Infrastructure.Idempotency
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly ConnectionFactory _connectionFactory;

        public IdempotenciaRepository(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<string> GetResultAsync(Guid key)
        {
            using (var connection = _connectionFactory.GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT resultado FROM idempotencia WHERE chave_idempotencia = @Key",
                    new { Key = key.ToString().ToUpper() });
            }
        }

        public async Task StoreResultAsync(Guid key, string result, string requestData = null)
        {
            using (var connection = _connectionFactory.GetOpenConnection())
            {
                var sql = "INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@Key, @RequestData, @Result) ON CONFLICT(chave_idempotencia) DO UPDATE SET resultado = @Result, requisicao = @RequestData;";
                await connection.ExecuteAsync(sql, new
                {
                    Key = key.ToString().ToUpper(),
                    RequestData = requestData,
                    Result = result
                });
            }
        }

        public async Task SaveRequestAsync(Guid key, string requestData)
        {
            using (var connection = _connectionFactory.GetOpenConnection())
            {
                var sql = "INSERT OR IGNORE INTO idempotencia (chave_idempotencia, requisicao) VALUES (@Key, @RequestData)";
                await connection.ExecuteAsync(sql, new
                {
                    Key = key.ToString().ToUpper(),
                    RequestData = requestData
                });
            }
        }
    }
}