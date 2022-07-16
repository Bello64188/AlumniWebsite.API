using AlumniWebsite.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Services
{
    public static class Extension
    {
        //cors configurations
        public static void CorsConfiguration(this IServiceCollection services)
        {
            services.AddCors(op=>op.AddPolicy(
                "policy", policy =>
                {
                    policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-InlineCount");
                   
                }
                ));
            services.AddCors(op => op.AddPolicy(
                " specificPolicy ", policy =>
                {

                    policy.WithOrigins("http://localhsot:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-InlineCount");
                }
                
                ));
        }
        //error messages configuration
        public static void ConfigurationException( this IApplicationBuilder builder)
        {

            builder.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextfeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextfeature!=null)
                    {
                        Log.Error($"Somthing went wrong in the {contextfeature.Error}");
                        await context.Response.WriteAsync(new Response
                        {
                            StatusCode = context.Response.StatusCode,
                            Messages = "Internal Server Error.Please try Again later."
                        }.ToString());
                    }
                });
            });
        }
        
    }
}
