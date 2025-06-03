using Xunit;
using NSubstitute;
using Questao5.Application.Queries;
using Questao5.Domain.Interfaces;
using Questao5.Domain.Entities;
using FluentValidation;
using Questao5.Application.Exceptions;
using Questao5.Application.Responses;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;

namespace Questao5.Tests.Application
{
    public class ConsultarSaldoContaCorrenteQueryHandlerTests
    {
        private readonly IContaCorrenteRepository _mockContaCorrenteRepository;
        private readonly IMovimentoRepository _mockMovimentoRepository;
        private readonly IValidator<ConsultarSaldoContaCorrenteQuery> _mockValidator;
        private readonly ConsultarSaldoContaCorrenteQueryHandler _handler;

        public ConsultarSaldoContaCorrenteQueryHandlerTests()
        {
            _mockContaCorrenteRepository = Substitute.For<IContaCorrenteRepository>();
            _mockMovimentoRepository = Substitute.For<IMovimentoRepository>();
            _mockValidator = Substitute.For<IValidator<ConsultarSaldoContaCorrenteQuery>>();
            _handler = new ConsultarSaldoContaCorrenteQueryHandler(
                _mockContaCorrenteRepository,
                _mockMovimentoRepository,
                _mockValidator);
        }

        [Fact]
        public async Task Handle_ValidQueryWithMovimentos_ReturnsCorrectSaldo()
        {
            // Organiza
            var contaId = Guid.NewGuid();
            var query = new ConsultarSaldoContaCorrenteQuery { IdContaCorrente = contaId };

            _mockValidator.ValidateAsync(query, Arg.Any<CancellationToken>()).Returns(new FluentValidation.Results.ValidationResult());
            _mockContaCorrenteRepository.GetByIdAsync(contaId).Returns(new ContaCorrente { IdContaCorrente = contaId, Ativo = true, Numero = 123, Nome = "Test Account" });

            var movimentos = new List<Movimento>
            {
                new Movimento { TipoMovimento = "C", Valor = 200.00M },
                new Movimento { TipoMovimento = "D", Valor = 50.00M },
                new Movimento { TipoMovimento = "C", Valor = 100.00M }
            };
            _mockMovimentoRepository.GetMovimentosByContaCorrenteIdAsync(contaId).Returns(movimentos);

            // Ação
            var result = await _handler.Handle(query, CancellationToken.None);

            // Afirmação
            Assert.NotNull(result);
            Assert.Equal(123, result.NumeroContaCorrente);
            Assert.Equal("Test Account", result.NomeTitular);
            Assert.Equal(250.00M, result.SaldoAtual); // 200 - 50 + 100 = 250
        }

        [Fact]
        public async Task Handle_ValidQueryNoMovimentos_ReturnsZeroSaldo()
        {
            // Organiza
            var contaId = Guid.NewGuid();
            var query = new ConsultarSaldoContaCorrenteQuery { IdContaCorrente = contaId };

            _mockValidator.ValidateAsync(query, Arg.Any<CancellationToken>()).Returns(new FluentValidation.Results.ValidationResult());
            _mockContaCorrenteRepository.GetByIdAsync(contaId).Returns(new ContaCorrente { IdContaCorrente = contaId, Ativo = true, Numero = 123, Nome = "Test Account" });
            _mockMovimentoRepository.GetMovimentosByContaCorrenteIdAsync(contaId).Returns(new List<Movimento>());

            // Ação
            var result = await _handler.Handle(query, CancellationToken.None);

            // Afirmação
            Assert.NotNull(result);
            Assert.Equal(0.00M, result.SaldoAtual);
        }

        [Fact]
        public async Task Handle_InactiveAccount_ThrowsBusinessException()
        {
            // Organiza
            var contaId = Guid.NewGuid();
            var query = new ConsultarSaldoContaCorrenteQuery { IdContaCorrente = contaId };

            _mockValidator.ValidateAsync(query, Arg.Any<CancellationToken>()).Returns(new FluentValidation.Results.ValidationResult());
            _mockContaCorrenteRepository.GetByIdAsync(contaId).Returns(new ContaCorrente { IdContaCorrente = contaId, Ativo = false, Numero = 123, Nome = "Inactive Account" });

            // Ação e Afirmação
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Conta corrente inativa.", exception.Message);
            Assert.Equal("INACTIVE_ACCOUNT", exception.Type);
        }

        // Adicione mais testes para outros cenários:
        // - Conta inválida (não encontrada)
        // - Erros de FluentValidation
    }
}