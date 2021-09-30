using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace freezebee_api.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class EncryptMiddleware
    {
        private readonly RequestDelegate _next;

        public EncryptMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            Stream originalBody = httpContext.Response.Body;

            try
            {
                using (var memStream = new MemoryStream())
                {
                    httpContext.Response.Body = memStream;

                    memStream.Position = 0;
                    string responseBody = new StreamReader(memStream).ReadToEnd();


                    memStream.Position = 0;

                    byte[] data = Encoding.UTF8.GetBytes(responseBody);//Encrypt responseBody here
                    memStream.Write(data, 0, data.Length);


                    memStream.Position = 0;

                    await memStream.CopyToAsync(originalBody);
                }

            }
            finally
            {
                httpContext.Response.Body = originalBody;
            }
            await _next(httpContext); // calling next middleware
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EncryptMiddleware>();
        }
    }
}
