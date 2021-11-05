using System;
using System.Threading;
using System.Threading.Tasks;
using Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMqAdapter;

namespace Receiver
{
    class Program
    {
        static Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            ProcessMessages(host.Services);

            return host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .Build();

            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services.AddRabbitMqDataAdapter(configuration));
        }

        private static void ProcessMessages(IServiceProvider services)
        {
            using var serviceScope = services.CreateScope();
            var provider = serviceScope.ServiceProvider;

            var adapter = provider.GetRequiredService<IRabbitMqAdapter>();
            var logger = provider.GetService<ILogger<Program>>();

            var processTime = 100; //milliseconds

            var ctSource = new CancellationTokenSource();
            var task = adapter.StartListen<CreateUserEvent>(
                null,
                (message) =>
                {
                    Thread.Sleep(processTime);
                    logger.LogInformation(" [x] Received {0}", message.ToString());
                },
                ctSource.Token);
        }
    }
}
