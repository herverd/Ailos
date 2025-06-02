using Questao5.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Questao5.Infrastructure.Interfaces
{
    public interface IMovimentoRepository
    {
        Task AddAsync(Movimento movimento);
        Task<IEnumerable<Movimento>> GetByContaCorrenteIdAsync(Guid idContaCorrente);
    }
}




