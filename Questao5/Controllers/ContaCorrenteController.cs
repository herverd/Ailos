using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands;
using Questao5.Application.Common;
using Questao5.Application.Queries;
using Questao5.Domain.Exceptions;
using System;
using System.Threading.Tasks;

namespace Questao5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContaCorrenteController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("movimentacao")]
        [ProducesResponseType(typeof(MovimentacaoResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Movimentar([FromBody] MovimentacaoContaCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);

                if (result.Sucesso)
                {
                    return Ok(result.Dados);
                }

                return BadRequest(new ErrorResponse(result.Mensagem, result.TipoErro));
            }
            catch (BusinessException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, ex.Tipo));
            }
            catch (Exception ex)
            {
            
                return StatusCode(500, new ErrorResponse("Ocorreu um erro interno no servidor.", "INTERNAL_ERROR"));
            }
        }


        [HttpGet("{idContaCorrente}/saldo")]
        [ProducesResponseType(typeof(ConsultaSaldoResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> ConsultarSaldo(Guid idContaCorrente)
        {
            try
            {
                var query = new ConsultaSaldoQuery { IdContaCorrente = idContaCorrente };
                var result = await _mediator.Send(query);

                if (result.Sucesso)
                {
                    return Ok(result.Dados);
                }
                return BadRequest(new ErrorResponse(result.Mensagem, result.TipoErro));
            }
            catch (BusinessException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, ex.Tipo));
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new ErrorResponse("Ocorreu um erro interno no servidor.", "INTERNAL_ERROR"));
            }
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
        public string Type { get; set; }

        public ErrorResponse(string message, string type)
        {
            Message = message;
            Type = type;
        }
    }
}
