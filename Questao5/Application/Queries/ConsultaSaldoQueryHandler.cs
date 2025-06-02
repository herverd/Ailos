using MediatR;
using Questao5.Application.Common;
using Questao5.Domain.Exceptions;
using Questao5.Infrastructure.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Questao5.Application.Queries
{
    public class ConsultaSaldoQueryHandler : IRequestHandler<ConsultaSaldoQuery, ResultadoOperacao<ConsultaSaldoResponse>>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public ConsultaSaldoQueryHandler(IContaCorrenteRepository contaCorrenteRepository, IMovimentoRepository movimentoRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<ResultadoOperacao<ConsultaSaldoResponse>> Handle(ConsultaSaldoQuery request, CancellationToken cancellationToken)
        {
   
            var conta = await _contaCorrenteRepository.GetByIdAsync(request.IdContaCorrente);
            if (conta == null)
            {
                throw new BusinessException("Conta corrente não encontrada.", "INVALID_ACCOUNT");
            }
            if (!conta.Ativo)
            {
                throw new BusinessException("Conta corrente inativa.", "INACTIVE_ACCOUNT");
            }

            var movimentos = await _movimentoRepository.GetByContaCorrenteIdAsync(request.IdContaCorrente);

            double creditos = movimentos.Where(m => m.TipoMovimento == 'C').Sum(m => m.Valor);
            double debitos = movimentos.Where(m => m.TipoMovimento == 'D').Sum(m => m.Valor);

            double saldoAtual = creditos - debitos;

            var response = new ConsultaSaldoResponse
            {
                NumeroContaCorrente = conta.Numero,
                NomeTitular = conta.Nome,
                DataHoraConsulta = DateTime.Now,
                SaldoAtual = saldoAtual
            };

            return ResultadoOperacao<ConsultaSaldoResponse>.SucessoResult(response);
        }
    }
}
