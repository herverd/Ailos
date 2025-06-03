using System;
using System.Threading.Tasks;

namespace Questao5.Infrastructure.Idempotency
{
    public interface IIdempotenciaRepository
    {
        Task<string> GetResultAsync(Guid key);
        Task StoreResultAsync(Guid key, string result, string requestData = null);
        Task SaveRequestAsync(Guid key, string requestData); 
    }
}