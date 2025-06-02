using Dapper;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Interfaces;
using Questao5.Infrastructure.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Questao5.Infrastructure.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly SqliteConnectionFactory _connectionFactory;

        public MovimentoRepository(SqliteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(Movimento movimento)
        {
            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor)
                            VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";

                await connection.ExecuteAsync(sql, new
                {
                    movimento.IdMovimento,
                    IdContaCorrente = movimento.IdContaCorrente.ToString().ToUpper(),
                    DataMovimento = movimento.DataMovimento.ToString("dd/MM/yyyy HH:mm:ss"), // Formato da data
                    movimento.TipoMovimento,
                    movimento.Valor
                });
            }
        }

        public async Task<IEnumerable<Movimento>> GetByContaCorrenteIdAsync(Guid idContaCorrente)
        {
            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                var sql = @"SELECT idmovimento, idcontacorrente, datamovimento, tipomovimento, valor
                            FROM movimento
                            WHERE idcontacorrente = @IdContaCorrente";

                var movimentosDb = await connection.QueryAsync<dynamic>(sql, new { IdContaCorrente = idContaCorrente.ToString().ToUpper() });

                var movimentos = new List<Movimento>();
                foreach (var m in movimentosDb)
                {
                    movimentos.Add(new Movimento
                    {
                        IdMovimento = Guid.Parse(m.idmovimento),
                        IdContaCorrente = Guid.Parse(m.idcontacorrente),
                        DataMovimento = DateTime.ParseExact(m.datamovimento, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                        TipoMovimento = char.Parse(m.tipomovimento),
                        Valor = m.valor
                    });
                }
                return movimentos;
            }
        }
    }
}
