using MediatR;
using Newtonsoft.Json;
using Questao5.Application.Common;
using Questao5.Domain.Entities;
using Questao5.Domain.Exceptions;
using Questao5.Infrastructure.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Questao5.Application.Commands
{
    public class MovimentacaoContaCommandHandler : IRequestHandler<MovimentacaoContaCommand, ResultadoOperacao<MovimentacaoResponse>>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;

        public MovimentacaoContaCommandHandler(IContaCorrenteRepository contaCorrenteRepository, IMovimentoRepository movimentoRepository, IIdempotenciaRepository idempotenciaRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
            _idempotenciaRepository = idempotenciaRepository;
        }

        public async Task<ResultadoOperacao<MovimentacaoResponse>> Handle(MovimentacaoContaCommand request, CancellationToken cancellationToken)
        {
            var idempotenciaRecord = await _idempotenciaRepository.GetByIdAsync(request.ChaveIdempotencia);

            if (idempotenciaRecord != null)
            {

                if (!string.IsNullOrEmpty(idempotenciaRecord.Resultado))
                {
                    var previousResult = JsonConvert.DeserializeObject<MovimentacaoResponse>(idempotenciaRecord.Resultado);
                    return ResultadoOperacao<MovimentacaoResponse>.SucessoResult(previousResult, "Requisição já processada (idempotente).");
                }

            }
            else
            {
                
                await _idempotenciaRepository.AddAsync(new Idempotencia
                {
                    ChaveIdempotencia = request.ChaveIdempotencia,
                    Requisicao = JsonConvert.SerializeObject(request),
                    Resultado = null 
                });
            }

   
            var conta = await _contaCorrenteRepository.GetByIdAsync(request.IdContaCorrente);
            if (conta == null)
            {
                throw new BusinessException("Conta corrente não encontrada.", "INVALID_ACCOUNT");
            }
            if (!conta.Ativo)
            {
                throw new BusinessException("Conta corrente inativa.", "INACTIVE_ACCOUNT");
            }

 
            if (request.Valor <= 0)
            {
                throw new BusinessException("Valor da movimentação deve ser positivo.", "INVALID_VALUE");
            }

           
            if (request.TipoMovimento != 'C' && request.TipoMovimento != 'D')
            {
                throw new BusinessException("Tipo de movimento inválido. Use 'C' para Crédito ou 'D' para Débito.", "INVALID_TYPE");
            }

            var novoMovimento = new Movimento
            {
                IdMovimento = Guid.NewGuid(),
                IdContaCorrente = request.IdContaCorrente,
                DataMovimento = DateTime.Now,
                TipoMovimento = request.TipoMovimento,
                Valor = request.Valor
            };

            await _movimentoRepository.AddAsync(novoMovimento);

            var response = new MovimentacaoResponse { IdMovimento = novoMovimento.IdMovimento };

           
            if (idempotenciaRecord == null) 
            {
                idempotenciaRecord = await _idempotenciaRepository.GetByIdAsync(request.ChaveIdempotencia);
            }
            if (idempotenciaRecord != null)
            {
                idempotenciaRecord.Resultado = JsonConvert.SerializeObject(response);
                await _idempotenciaRepository.UpdateAsync(idempotenciaRecord);
            }

            return ResultadoOperacao<MovimentacaoResponse>.SucessoResult(response);
        }
    }
}
