using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PaymentGateway.Domain.DTOs;
using PaymentGateway.Repository;

namespace PaymentGateway.API.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ApiKeyAuthenticatonMiddleWare
    {
        private readonly RequestDelegate _next;

        public ApiKeyAuthenticatonMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IMerchantRepository merchantRepository, IRequestInfo requestInfo)
        {
            if (httpContext.Request.Path.Value == "/" || httpContext.Request.Path.Value.Contains("swagger"))
            {
                await _next.Invoke(httpContext);
                return;
            }

            
            if (httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                string key = httpContext.Request.Headers["Authorization"];
                if(String.IsNullOrEmpty(key) || String.IsNullOrWhiteSpace(key))
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    httpContext.Response.ContentType = "text\\plain";
                    httpContext.Response.WriteAsync("Authorization Key was not provided").Wait();
                    return;
                }

                var merchant_id = await merchantRepository.GetMerchantIdBySecretKey(key);
                // Check if token is correct
                if (merchant_id != Guid.Empty)
                {
                    requestInfo.User_Id = merchant_id;
                    requestInfo.Transaction_Reference = Guid.NewGuid();
                    await _next.Invoke(httpContext);
                    return;
                }
                else
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    httpContext.Response.ContentType = "text\\plain";
                    httpContext.Response.WriteAsync("Invalid Authorization Key").Wait();
                    return;
                }
            }       

            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            httpContext.Response.ContentType = "text\\plain";
            httpContext.Response.WriteAsync("Authorization header not provided").Wait();
            return;
            

        }

        private bool IsAuthorized(string authToken)
        {
            return true;
            //return authToken.Equals(_token);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ApiKeyAuthenticatonMiddleWareExtensions
    {
        public static IApplicationBuilder UseApiKeyAuthenticatonMiddleWare(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyAuthenticatonMiddleWare>();
        }
    }

}
