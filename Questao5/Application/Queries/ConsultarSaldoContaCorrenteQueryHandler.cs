using MediatR;
using Questao5.Application.Exceptions;
using Questao5.Application.Responses;
using Questao5.Domain.Interfaces;
using FluentValidation;

namespace Questao5.Application.Queries
{
    public class ConsultarSaldoContaCorrenteQueryHandler : IRequestHandler<ConsultarSaldoContaCorrenteQuery, SaldoContaCorrenteResponse>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IValidator<ConsultarSaldoContaCorrenteQuery> _validator;

        public ConsultarSaldoContaCorrenteQueryHandler(
            IContaCorrenteRepository contaCorrenteRepository,
            IMovimentoRepository movimentoRepository,
            IValidator<ConsultarSaldoContaCorrenteQuery> validator)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
            _validator = validator;
        }

        public async Task<SaldoContaCorrenteResponse> Handle(ConsultarSaldoContaCorrenteQuery request, CancellationToken cancellationToken)
        {
            // 1. Valida a requisição de entrada
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.First();
                throw new BusinessException(error.ErrorMessage, error.ErrorCode);
            }

            // 2. Validações de Negócio
            var contaCorrente = await _contaCorrenteRepository.GetByIdAsync(request.IdContaCorrente);

            if (contaCorrente == null)
            {
                throw new BusinessException("Conta corrente não encontrada.", "INVALID_ACCOUNT");
            }

            if (!contaCorrente.Ativo)
            {
                throw new BusinessException("Conta corrente inativa.", "INACTIVE_ACCOUNT");
            }

            // 3. Calcula o Saldo
            var movimentos = await _movimentoRepository.GetMovimentosByContaCorrenteIdAsync(request.IdContaCorrente);

            decimal saldo = 0.00M;
            foreach (var movimento in movimentos)
            {
                if (movimento.TipoMovimento == "C")
                {
                    saldo += movimento.Valor;
                }
                else if (movimento.TipoMovimento == "D")
                {
                    saldo -= movimento.Valor;
                }
            }

            // 4. Retorna a Resposta
            return new SaldoContaCorrenteResponse
            {
                NumeroContaCorrente = contaCorrente.Numero,
                NomeTitular = contaCorrente.Nome,
                DataHoraConsulta = DateTime.Now,
                SaldoAtual = saldo
            };
        }
    }
}