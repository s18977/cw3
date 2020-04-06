using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var bodyStream = string.Empty;
            //using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
            await _next(httpContext);
        }
    }
}
