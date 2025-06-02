using System;

namespace Questao5.Domain.Exceptions
{
    public class BusinessException : Exception
    {
        public string Tipo { get; }

        public BusinessException(string message, string tipo) : base(message)
        {
            Tipo = tipo;
        }
    }

}
