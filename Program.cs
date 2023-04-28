using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlumniWebsite.API.Data;
using AlumniWebsite.API.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace AlumniWebsite.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(path: "C:\\Users\\MMSS\\source\\repos\\SchAlumniWebsite\\AlumniWebsite.API\\Logs\\log-.txt",
                                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}[{Level:u3}]{Message:lj}{NewLine}{Exception}",
                                rollingInterval: RollingInterval.Day,
                                restrictedToMinimumLevel: LogEventLevel.Information

                                ).CreateLogger();
            try
            {
                Log.Information("Application Have Started!!");
                var host = CreateHostBuilder(args).Build();
                using var scope = host.Services.CreateScope();
                var service = scope.ServiceProvider;
                try
                {
                    var context = service.GetRequiredService<AppDbContext>();
                    var userManager = service.GetRequiredService<UserManager<Member>>();
                    var roleManager = service.GetRequiredService<RoleManager<MemberRole>>();
                    //await context.Database.MigrateAsync();
                    //await MemberSeed.SeedMembersAsync(userManager, roleManager);
                }
                catch (Exception ex)
                {

                    Log.Error(ex, "An error occurred during migration.");
                }
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application Failed to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }


        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
