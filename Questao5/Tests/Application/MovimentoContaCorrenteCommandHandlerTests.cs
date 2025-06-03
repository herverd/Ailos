using Xunit;
using NSubstitute;
using Questao5.Application.Commands;
using Questao5.Domain.Interfaces;
using Questao5.Infrastructure.Idempotency;
using Questao5.Domain.Entities;
using FluentValidation;
using Questao5.Application.Exceptions;
using Questao5.Application.Responses;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Questao5.Tests.Application
{
    public class MovimentoContaCorrenteCommandHandlerTests
    {
        private readonly IContaCorrenteRepository _mockContaCorrenteRepository;
        private readonly IMovimentoRepository _mockMovimentoRepository;
        private readonly IIdempotenciaRepository _mockIdempotenciaRepository;
        private readonly IValidator<MovimentoContaCorrenteCommand> _mockValidator;
        private readonly MovimentoContaCorrenteCommandHandler _handler;

        public MovimentoContaCorrenteCommandHandlerTests()
        {
            _mockContaCorrenteRepository = Substitute.For<IContaCorrenteRepository>();
            _mockMovimentoRepository = Substitute.For<IMovimentoRepository>();
            _mockIdempotenciaRepository = Substitute.For<IIdempotenciaRepository>();
            _mockValidator = Substitute.For<IValidator<MovimentoContaCorrenteCommand>>();
            _handler = new MovimentoContaCorrenteCommandHandler(
                _mockContaCorrenteRepository,
                _mockMovimentoRepository,
                _mockIdempotenciaRepository,
                _mockValidator);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessResponse()
        {
            // Organiza
            var contaId = Guid.NewGuid();
            var requisicaoId = Guid.NewGuid();
            var command = new MovimentoContaCorrenteCommand
            {
                IdRequisicao = requisicaoId,
                IdContaCorrente = contaId,
                Valor = 100,
                TipoMovimento = 'C'
            };

            _mockIdempotenciaRepository.GetResultAsync(requisicaoId).Returns((string)null);
            _mockValidator.ValidateAsync(command, Arg.Any<CancellationToken>()).Returns(new FluentValidation.Results.ValidationResult());
            _mockContaCorrenteRepository.GetByIdAsync(contaId).Returns(new ContaCorrente { IdContaCorrente = contaId, Ativo = true, Numero = 123, Nome = "Test Account" });
            _mockMovimentoRepository.AddAsync(Arg.Any<Movimento>()).Returns(Task.CompletedTask);
            _mockIdempotenciaRepository.StoreResultAsync(requisicaoId, Arg.Any<string>(), Arg.Any<string>()).Returns(Task.CompletedTask);

            // Ação
            var result = await _handler.Handle(command, CancellationToken.None);

            // Afirmação
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.IdMovimento);
            await _mockMovimentoRepository.Received(1).AddAsync(Arg.Is<Movimento>(m =>
                m.IdContaCorrente == contaId &&
                m.Valor == 100 &&
                m.TipoMovimento == "C"));
            await _mockIdempotenciaRepository.Received(1).StoreResultAsync(requisicaoId, Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_InactiveAccount_ThrowsBusinessException()
        {
            // Organiza
            var contaId = Guid.NewGuid();
            var command = new MovimentoContaCorrenteCommand
            {
                IdRequisicao = Guid.NewGuid(),
                IdContaCorrente = contaId,
                Valor = 50,
                TipoMovimento = 'D'
            };

            _mockIdempotenciaRepository.GetResultAsync(command.IdRequisicao).Returns((string)null);
            _mockValidator.ValidateAsync(command, Arg.Any<CancellationToken>()).Returns(new FluentValidation.Results.ValidationResult());
            _mockContaCorrenteRepository.GetByIdAsync(contaId).Returns(new ContaCorrente { IdContaCorrente = contaId, Ativo = false, Numero = 123, Nome = "Inactive Account" });

            // Ação e Afirmação
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Conta corrente inativa.", exception.Message);
            Assert.Equal("INACTIVE_ACCOUNT", exception.Type);
        }

        // Adicione mais testes para outros cenários:
        // - Conta inválida (não encontrada)
        // - Valor negativo
        // - Tipo inválido
        // - Idempotência acionada (retorna resultado armazenado)
        // - Erros de FluentValidation
    }
}