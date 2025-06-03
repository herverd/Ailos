using MediatR;
using Questao5.Application.Exceptions;
using Questao5.Application.Responses;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using Questao5.Infrastructure.Idempotency;
using FluentValidation;
using System.Text.Json;

namespace Questao5.Application.Commands
{
    public class MovimentoContaCorrenteCommandHandler : IRequestHandler<MovimentoContaCorrenteCommand, MovimentoContaCorrenteResponse>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;
        private readonly IValidator<MovimentoContaCorrenteCommand> _validator;

        public MovimentoContaCorrenteCommandHandler(
            IContaCorrenteRepository contaCorrenteRepository,
            IMovimentoRepository movimentoRepository,
            IIdempotenciaRepository idempotenciaRepository,
            IValidator<MovimentoContaCorrenteCommand> validator)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
            _idempotenciaRepository = idempotenciaRepository;
            _validator = validator;
        }

        public async Task<MovimentoContaCorrenteResponse> Handle(MovimentoContaCorrenteCommand request, CancellationToken cancellationToken)
        {
            // 1. Verifica a Idempotência
            var existingResult = await _idempotenciaRepository.GetResultAsync(request.IdRequisicao);
            if (existingResult != null)
            {
                return JsonSerializer.Deserialize<MovimentoContaCorrenteResponse>(existingResult);
            }

            // 2. Valida a requisição de entrada
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.First();
                throw new BusinessException(error.ErrorMessage, error.ErrorCode);
            }

            // 3. Validações de Negócio
            var contaCorrente = await _contaCorrenteRepository.GetByIdAsync(request.IdContaCorrente);

            if (contaCorrente == null)
            {
                throw new BusinessException("Conta corrente não encontrada.", "INVALID_ACCOUNT");
            }

            if (!contaCorrente.Ativo)
            {
                throw new BusinessException("Conta corrente inativa.", "INACTIVE_ACCOUNT");
            }

            // As validações específicas de valor e tipo são tratadas pelo FluentValidation,
            // mas podem ser adicionadas aqui se houver validações cruzadas mais complexas.

            // 4. Cria o Movimento
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid(),
                IdContaCorrente = request.IdContaCorrente,
                DataMovimento = DateTime.Now.ToString("dd/MM/yyyy"), // De acordo com o formato do script
                TipoMovimento = request.TipoMovimento.ToString(),
                Valor = request.Valor
            };

            // 5. Persiste o Movimento
            await _movimentoRepository.AddAsync(movimento);

            // 6. Armazena o resultado da Idempotência
            var response = new MovimentoContaCorrenteResponse { IdMovimento = movimento.IdMovimento };
            await _idempotenciaRepository.StoreResultAsync(request.IdRequisicao, JsonSerializer.Serialize(response));

            return response;
        }
    }
}
