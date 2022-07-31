using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Movies.Client.Handlers;
using Movies.Client.Services;
using System;
using System.Net.Http;
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

            services.AddHttpClient("MoviesClient", client =>
            {
                client.BaseAddress = new Uri("http://localhost:35087");
                client.Timeout = new TimeSpan(0, 0, 30);
                client.DefaultRequestHeaders.Clear();
            })
                .AddHttpMessageHandler(handler=>new TimeOutDelegatingHandler(TimeSpan.FromSeconds(20)))
                .AddHttpMessageHandler(handler => new RetryPolicyDelegatingHandler(2))
                .ConfigurePrimaryHttpMessageHandler(handler =>
                new HttpClientHandler()
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip
                });

            // moved defaults for typed client to separate MoviesClient class
            // one should still prefer configuring handlers pipeline in ConfigureServices
            //services.AddHttpClient<MoviesClient>(client =>
            //{
            //    client.BaseAddress = new Uri("http://localhost:35087");
            //    client.Timeout = new TimeSpan(0, 0, 30);
            //    client.DefaultRequestHeaders.Clear();
            //}).ConfigurePrimaryHttpMessageHandler(handler => new HttpClientHandler()
            //{
            //    AutomaticDecompression = System.Net.DecompressionMethods.GZip
            //});

            services.AddHttpClient<MoviesClient>()
                .ConfigurePrimaryHttpMessageHandler(handler =>
                new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip
                });

            // services.AddScoped<IIntegrationService, CRUDService>();
            // services.AddScoped<IIntegrationService, PartialUpdateService>();
            // services.AddScoped<IIntegrationService, StreamService>();
            // services.AddScoped<IIntegrationService, CancellationService>();
            // services.AddScoped<IIntegrationService, HttpClientFactoryInstanceManagementService>();
            // services.AddScoped<IIntegrationService, DealingWithErrorsAndFaultsService>();
            // services.AddScoped<IIntegrationService, HttpHandlersService>();
        }
    }
}
