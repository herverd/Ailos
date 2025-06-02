using Questao5.Domain.Entities;
using System;
using System.Threading.Tasks;


namespace Questao5.Infrastructure.Interfaces
{
    public interface IContaCorrenteRepository
    {
        Task<ContaCorrente> GetByIdAsync(Guid id);
    }

}
