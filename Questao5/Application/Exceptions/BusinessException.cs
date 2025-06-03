using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Questao5.Domain.Models;

namespace Questao5.Application.Exceptions
{
    public class BusinessException : Exception
    {
        public string Type { get; }

        public BusinessException(string message, string type) : base(message)
        {
            Type = type;
        }
    }

    public class BusinessExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is BusinessException businessException)
            {
                context.Result = new BadRequestObjectResult(new ErrorResponse
                {
                    Message = businessException.Message,
                    Type = businessException.Type
                });
                context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                context.ExceptionHandled = true;
            }
        }
    }
}