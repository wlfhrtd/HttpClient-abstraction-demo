using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Movies.Client.Services;
using System;
using System.Threading.Tasks;


namespace Movies.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ServiceCollection serviceCollection = new();

            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            try
            {
                await serviceProvider.GetService<IIntegrationService>().Run();
            }
            catch (Exception e)
            {
                var logger = serviceProvider.GetService<ILogger<Program>>();
                logger.LogError(e, "Exception occurred while running IntegrationService.");
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            // services.AddScoped<IIntegrationService, CRUDService>();
            // services.AddScoped<IIntegrationService, PartialUpdateService>();
            services.AddScoped<IIntegrationService, StreamService>();
        }
    }
}
