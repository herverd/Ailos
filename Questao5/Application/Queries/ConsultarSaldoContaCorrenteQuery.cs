using MediatR;
using Questao5.Application.Responses;

namespace Questao5.Application.Queries
{
    public class ConsultarSaldoContaCorrenteQuery : IRequest<SaldoContaCorrenteResponse>
    {
        public Guid IdContaCorrente { get; set; }
    }
}