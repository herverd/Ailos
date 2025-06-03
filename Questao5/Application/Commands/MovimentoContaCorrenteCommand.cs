using MediatR;
using Questao5.Application.Responses;

namespace Questao5.Application.Commands
{
    public class MovimentoContaCorrenteCommand : IRequest<MovimentoContaCorrenteResponse>
    {
        public Guid IdRequisicao { get; set; } // Chave de idempotência
        public Guid IdContaCorrente { get; set; }
        public decimal Valor { get; set; }
        public char TipoMovimento { get; set; } // 'C' para Crédito, 'D' para Débito
    }
}