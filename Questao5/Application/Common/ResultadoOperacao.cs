namespace Questao5.Application.Common
{
    public class ResultadoOperacao<T>
    {
        public bool Sucesso { get; set; }
        public T Dados { get; set; }
        public string Mensagem { get; set; }
        public string TipoErro { get; set; }

        public static ResultadoOperacao<T> SucessoResult(T data, string message = null)
        {
            return new ResultadoOperacao<T> { Sucesso = true, Dados = data, Mensagem = message };
        }

        public static ResultadoOperacao<T> ErroResult(string message, string tipoErro)
        {
            return new ResultadoOperacao<T> { Sucesso = false, Mensagem = message, TipoErro = tipoErro };
        }
    }
}
