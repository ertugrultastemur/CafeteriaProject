using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public class ExceptionMiddleware
    {
        private RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(httpContext, e);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception e)
        {
            httpContext.Response.ContentType = "application/json";
            int statusCode;
            string message;

            // Exception türüne göre durum kodu belirleme
            if (e is ValidationException)
            {
                statusCode = (int)HttpStatusCode.BadRequest; // 400 Bad Request
                message = e.Message;
            }
            else if (e is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized; // 401 Unauthorized
                message = e.Message;
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError; // 500 Internal Server Error
                message = "Internal Server Error";
            }

            httpContext.Response.StatusCode = statusCode;

            return httpContext.Response.WriteAsync(new ErrorDetails
            {
                StatusCode = statusCode,
                Message = message
            }.ToString());
        }
    }
}
