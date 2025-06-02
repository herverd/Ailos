using MediatR;
using Questao5.Application.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Questao5.Application.Queries
{
    public class ConsultaSaldoQuery : IRequest<ResultadoOperacao<ConsultaSaldoResponse>>
    {
        [Required]
        public Guid IdContaCorrente { get; set; }
    }

    public class ConsultaSaldoResponse
    {
        public int NumeroContaCorrente { get; set; }
        public string NomeTitular { get; set; }
        public DateTime DataHoraConsulta { get; set; }
        public double SaldoAtual { get; set; }
    }
}
