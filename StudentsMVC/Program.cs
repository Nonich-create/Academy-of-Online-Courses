using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Students.DAL.Models;
using System;
using System.Threading.Tasks;
using Serilog.Formatting.Compact;
using Students.BLL.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace Students.MVC
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            
                .Enrich.FromLogContext()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .WriteTo.File(new RenderedCompactJsonFormatter(), "/logs/log.ndjson")
                .WriteTo.Seq("http://localhost:5341")
                .CreateBootstrapLogger(); 
            var host = CreateHostBuilder(args).Build();

            try
            {
                Log.Information("Starting up");
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
 
                    try
                    {
                        var context = services.GetRequiredService<Context>();
                        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                        var emailAdminOptions = services.GetRequiredService<IOptions<EmailAdminOptions>>();

                        await ApplicationInitializer.InitializeAsync(userManager, roleManager, emailAdminOptions, context);
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while seeding the database.");
                    }
                }
              await host.RunAsync();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureLogging(logging =>
                {
                
                    logging.ClearProviders();
                    logging.AddSerilog();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}