using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands;
using Questao5.Application.Queries;
using Questao5.Application.Responses;
using Questao5.Domain.Models;
using System.Net;
using Swashbuckle.AspNetCore;




namespace Questao5.Api.Controllers
{
    [ApiController]
    [Route("api/contacorrente")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContaCorrenteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Realiza a movimentação de uma conta corrente (crédito ou débito).
        /// </summary>
        /// <param name="idRequisicao">Identificação única da requisição (chave de idempotência).</param>
        /// <param name="command">Dados da movimentação.</param>
        /// <returns>HTTP 200 com o ID do movimento gerado, ou HTTP 400 com erro.</returns>
        [HttpPost("movimentar/{idRequisicao}")]
        [ProducesResponseType(typeof(MovimentoContaCorrenteResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        //[SwaggerOperation(Summary = "Movimenta uma conta corrente (crédito ou débito).",
        //                  Description = "Esta API permite registrar um crédito ou débito em uma conta corrente. " +
        //                                "A chave de idempotência é utilizada para garantir que requisições repetidas " +
        //                                "não resultem em operações duplicadas.")]
        public async Task<IActionResult> MovimentarContaCorrente(
            [FromHeader(Name = "X-Idempotency-Key")] Guid idRequisicao,
            [FromBody] MovimentoContaCorrenteCommand command)
        {
            command.IdRequisicao = idRequisicao;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Consulta o saldo atual de uma conta corrente.
        /// </summary>
        /// <param name="idContaCorrente">Identificação da conta corrente.</param>
        /// <returns>HTTP 200 com os detalhes do saldo, ou HTTP 400 com erro.</returns>
        [HttpGet("saldo/{idContaCorrente}")]
        [ProducesResponseType(typeof(SaldoContaCorrenteResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        //[SwaggerOperation(Summary = "Consulta o saldo de uma conta corrente.",
                          //Description = "Retorna o saldo atual de uma conta corrente específica, " +
                          //              "calculado com base em todas as movimentações registradas.")]
        public async Task<IActionResult> ConsultarSaldoContaCorrente(
            [FromRoute] Guid idContaCorrente)
        {
            var query = new ConsultarSaldoContaCorrenteQuery { IdContaCorrente = idContaCorrente };
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
