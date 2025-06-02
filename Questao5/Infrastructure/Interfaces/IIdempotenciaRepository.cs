using Questao5.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Questao5.Infrastructure.Interfaces
{
    public interface IIdempotenciaRepository
    {
        Task<Idempotencia> GetByIdAsync(Guid id);
        Task AddAsync(Idempotencia idempotencia);
        Task UpdateAsync(Idempotencia idempotencia);
    }
}

