using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using AREML.EPOD.Data.Logging;
using Microsoft.IdentityModel.Abstractions;
using System.Net;
using System.Text.Json;
using Microsoft.ApplicationInsights;
using AREML.EPOD.Core.Dtos.Response;

namespace AREML.EPOD.Data.Filters
{
    public class CommonExceptionFilter : IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            var ai = context.HttpContext.RequestServices.GetService(typeof(TelemetryClient)) as TelemetryClient;
            var status = HttpStatusCode.InternalServerError;
            var message = string.Empty;

            var exceptionType = context.Exception.GetType();
            if (exceptionType == typeof(UnauthorizedAccessException))
            {
                message = "Unauthorized Access";
                status = HttpStatusCode.Unauthorized;
            }
            else if (exceptionType == typeof(NotImplementedException))
            {
                message = "A server error occurred.";
                status = HttpStatusCode.NotImplemented;
            }
            else if (exceptionType == typeof(ArgumentException))
            {
                message = "Values can not be empty, Fill out all the required details";
                status = HttpStatusCode.UnprocessableEntity;
            }
            else
            {
                message = context.Exception.Message;
                status = HttpStatusCode.BadRequest;
            }

            LogWriter.WriteToFile(context.Exception);

            context.ExceptionHandled = true;

            var response = context.HttpContext.Response;

            response.StatusCode = (int)status;
            response.ContentType = "application/json";

            ResponseMessage exception = new ResponseMessage();
            exception.Message = message;
            exception.Error = context.Exception.Message;
            exception.Status = (int)status;

            await response.WriteAsync(JsonSerializer.Serialize(exception));
        }
    }
}
