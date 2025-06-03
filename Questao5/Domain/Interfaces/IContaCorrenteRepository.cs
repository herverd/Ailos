using Questao5.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Questao5.Domain.Interfaces
{
    public interface IContaCorrenteRepository
    {
        Task<ContaCorrente> GetByIdAsync(Guid id);
        Task<ContaCorrente> GetByNumberAsync(int accountNumber); // Potencialmente útil mais tarde
    }
}