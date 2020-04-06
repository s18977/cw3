using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            string path = @"Middlewares\requestLog.txt";

            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(httpContext.Request.Method);
                    sw.WriteLine(httpContext.Request.Path);
                    sw.WriteLine(httpContext.Request.QueryString);
                }
            }else
            {
                using(StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("\n");
                    sw.WriteLine(httpContext.Request.Method);
                    sw.WriteLine(httpContext.Request.Path);

                    var bodyReq = string.Empty;
                    using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                    {
                        bodyReq = await reader.ReadToEndAsync();
                    }

                    sw.WriteLine(bodyReq);
                    sw.WriteLine(httpContext.Request.QueryString);
                }
            }

            await _next(httpContext);
        }
    }
}
