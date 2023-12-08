using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Montreal.Core.Crosscutting.Domain.Controller.ActionFilter
{
    internal class AuditFilter : IActionFilter
    {
        private readonly string _callIdHeader;

        public AuditFilter()
        {
            this._callIdHeader = "CALL_ID";
        }

        private string GetRequestBody(ActionExecutingContext context)
        {
            string returnValue = string.Empty;

            foreach (KeyValuePair<string, object> parameter in context.ActionArguments)
            {
                if (!context.HttpContext.Request.Query.ContainsKey(parameter.Key) && !context.HttpContext.Request.RouteValues.ContainsKey(parameter.Key))
                {
                    returnValue = JsonConvert.SerializeObject(parameter.Value);
                    break;
                }
            }

            return returnValue;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string callId = Guid.NewGuid().ToString();
            string log = string.Empty;
            ILogger<AuditFilter> logger = context.HttpContext.RequestServices.GetService<ILogger<AuditFilter>>();

            log = "\r\n" +
                  "Chamada iniciada: {0}\r\n" +
                  "Data: {1}\r\n" +
                  "Rota: {2}{3}\r\n" +
                  "Payload requisicao: {4}\r\n" +
                  "\r\n";

            context.HttpContext.Request.Headers.Add(this._callIdHeader, callId);

            logger.LogInformation(string.Format(log,
                callId,
                DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                context.HttpContext.Request.Path,
                context.HttpContext.Request.QueryString.ToString(),
                this.GetRequestBody(context))
            );
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            string callId = context.HttpContext.Request.Headers[this._callIdHeader];
            string log = string.Empty;
            string result = string.Empty;
            int resultCode = 0;
            ILogger<AuditFilter> logger = context.HttpContext.RequestServices.GetService<ILogger<AuditFilter>>();

            if (context.Exception == null)
            {
                if (context.Result.GetType().IsAssignableTo(typeof(ObjectResult)))
                {
                    result = JsonConvert.SerializeObject(((ObjectResult)context.Result).Value, Formatting.Indented, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                    resultCode = ((ObjectResult)context.Result).StatusCode.Value;
                }
                else
                {
                    resultCode = context.HttpContext.Response.StatusCode;
                }

                log = "\r\n" +
                      "Chamada finalizada: {0}\r\n" +
                      "Data: {1}\r\n" +
                      "Codigo de retorno: {2}\r\n" +
                      "Payload resposta: {3}\r\n" +
                      "\r\n";

                logger.LogInformation(string.Format(log,
                    callId,
                    DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    resultCode,
                    result)
                );
            }
            else
            {
                log = "\r\n" +
                      "Chamada finalizada: {0}\r\n" +
                      "Data: {1}\r\n" +
                      "Codigo de retorno: 500\r\n" +
                      "Excecao: {2}\r\n" +
                      "\r\n";

                logger.LogInformation(string.Format(log,
                    callId,
                    DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    JsonConvert.SerializeObject(context.Exception))
                );
            }
        }
    }
}
