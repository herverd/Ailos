using System;

namespace Questao5.Application.Responses
{
    public class SaldoContaCorrenteResponse
    {
        public int NumeroContaCorrente { get; set; }
        public string NomeTitular { get; set; }
        public DateTime DataHoraConsulta { get; set; }
        public decimal SaldoAtual { get; set; }
    }
}