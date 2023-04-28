using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlumniWebsite.API.Configurations.CloudConfiguration;
using AlumniWebsite.API.Configurations.Filter;
using AlumniWebsite.API.Data;
using AlumniWebsite.API.ImplementInterface;
using AlumniWebsite.API.Interface;
using AlumniWebsite.API.Model;
using AlumniWebsite.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AlumniWebsite.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthManager, AuthManager>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IEmailService, EmailService>();
            services.Configure<Cloud>(Configuration.GetSection("Cloud"));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailConfiguration"));
            services.AddIdetityRolePolicy();
            // services.AddIdentity<Member, MemberRole>().AddEntityFrameworkStores<AppDbContext>();
            services.AddDbContext<AppDbContext>(option => option.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.jwtConfiguration(Configuration);
            services.AddAuthorization();
            services.AddAuthentication();
            services.CorsConfiguration();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AlumniWebsite.API", Version = "v1" });
            });
            services.AddControllers().AddNewtonsoftJson(option =>
            {
                option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            });
            services.AddTransient<LogMemberActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AlumniWebsite.API v1"));
            }

            app.UseHttpsRedirection();
            app.UseCors("policy");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
