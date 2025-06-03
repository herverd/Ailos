using FluentValidation;
using Questao5.Application.Commands;

namespace Questao5.Application.Validators
{
    public class MovimentoContaCorrenteCommandValidator : AbstractValidator<MovimentoContaCorrenteCommand>
    {
        public MovimentoContaCorrenteCommandValidator()
        {
            RuleFor(x => x.IdContaCorrente)
                .NotEmpty().WithMessage("O ID da conta corrente é obrigatório.");

            RuleFor(x => x.Valor)
                .GreaterThan(0).WithMessage("O valor do movimento deve ser positivo.")
                .WithName("Valor").WithErrorCode("INVALID_VALUE");

            RuleFor(x => x.TipoMovimento)
                .Must(x => x == 'C' || x == 'D')
                .WithMessage("O tipo de movimento deve ser 'C' (Crédito) ou 'D' (Débito).")
                .WithName("TipoMovimento").WithErrorCode("INVALID_TYPE");
        }
    }
}