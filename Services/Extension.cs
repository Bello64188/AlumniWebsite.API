using AlumniWebsite.API.Configurations.Filter;
using AlumniWebsite.API.Data;
using AlumniWebsite.API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Services
{
    public static class Extension
    {
        //cors configurations
        public static void CorsConfiguration(this IServiceCollection services)
        {
            services.AddCors(op => op.AddPolicy(
                "policy", policy =>
                {
                    policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-InlineCount", "X-Pagination");

                }
                ));
            services.AddCors(op => op.AddPolicy(
                " specificPolicy ", policy =>
                {

                    policy.WithOrigins("http://localhsot:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("X-InlineCount", "X-Pagination");
                }

                ));
        }
        //error messages configuration
        public static void ConfigurationException(this IApplicationBuilder builder)
        {

            builder.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextfeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextfeature != null)
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
        public static void AddPagination(this HttpResponse response,
            int currentPage, int itemPerPage, int totalPage, int totalItem)
        {

            var paginationHeaders = new PaginationHeader(currentPage, itemPerPage, totalItem, totalPage);
            var camlCaseFormatter = new JsonSerializerSettings();
            camlCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationHeaders, camlCaseFormatter));
            response.Headers.Add("access-control-expose-headers", "X-Pagination");
        }
        public static int CalculateAge(this DateTime theAge)
        {

            var age = DateTime.Today.Year - theAge.Year;
            if (theAge.AddYears(age) > DateTime.Today)
            {
                age--;
            }
            return age;
        }

        public static void IdentityConfiguration(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<Member>(option => option.User.RequireUniqueEmail = true);
            // builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder.AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
        }

        public static void jwtConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var key = Environment.GetEnvironmentVariable("KEYAPI");
            var jwtSetting = configuration.GetSection("Jwt");
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSetting?.GetSection("Issuer").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))


                };
            });
        }
    }
}
