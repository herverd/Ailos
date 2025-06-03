using FluentValidation;
using Questao5.Application.Queries;

namespace Questao5.Application.Validators
{
    public class ConsultarSaldoContaCorrenteQueryValidator : AbstractValidator<ConsultarSaldoContaCorrenteQuery>
    {
        public ConsultarSaldoContaCorrenteQueryValidator()
        {
            RuleFor(x => x.IdContaCorrente)
                .NotEmpty().WithMessage("O ID da conta corrente é obrigatório.");
        }
    }
}