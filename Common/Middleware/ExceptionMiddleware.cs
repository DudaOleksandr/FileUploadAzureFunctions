using System;
using System.Net;
using System.Threading.Tasks;
using Common.Errors;
using Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionMiddleware> _logger;

        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                var traceId = context.TraceIdentifier;
                switch (ex)
                {
                    case ValidationAggregationException validationAggregationException:
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            context.Response.ContentType = "application/json";
                            var errorResponse = new ErrorResponse(traceId);

                            foreach(var error in validationAggregationException.Exceptions)
                            {
                                errorResponse.Errors.Add(new Error(error.Message, error.Source));
                            }
                            await context.Response.WriteAsync(errorResponse.ToString());
                            break;
                        }
                    case ValidationException validationException:
                        {
                            await CreateErrorResponse((int)HttpStatusCode.BadRequest, new ErrorResponse(validationException.Message, validationException.Source, traceId), context);
                            break;
                        }
                    case NotFoundException notFoundException:
                        {
                            await CreateErrorResponse((int)HttpStatusCode.NotFound, new ErrorResponse(notFoundException.Message, notFoundException.Source, traceId), context);

                            break;
                        }
                    default:
                        {
                            if (_env.IsDevelopment())
                            {
                                throw;
                            }
                            await CreateErrorResponse((int)HttpStatusCode.InternalServerError, new ErrorResponse("Internal server Error", string.Empty, traceId), context);
                            break;
                        }
                }
            }
        }

        private async Task CreateErrorResponse(int statusCode, ErrorResponse errorResponse, HttpContext context)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(errorResponse.ToString());
        }
    }
}
