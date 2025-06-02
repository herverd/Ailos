using MediatR;
using Questao5.Application.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Questao5.Application.Commands
{
    public class MovimentacaoContaCommand : IRequest<ResultadoOperacao<MovimentacaoResponse>>
    {
        [Required]
        public Guid ChaveIdempotencia { get; set; }

        [Required]
        public Guid IdContaCorrente { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser positivo.")]
        public double Valor { get; set; }

        [Required]
        [RegularExpression("^[CD]$", ErrorMessage = "O tipo de movimento deve ser 'C' (Crédito) ou 'D' (Débito).")]
        public char TipoMovimento { get; set; }
    }

    public class MovimentacaoResponse
    {
        public Guid IdMovimento { get; set; }
    }
}
