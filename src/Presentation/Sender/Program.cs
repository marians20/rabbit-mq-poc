using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMqAdapter;
using System;
using Events;
using RandomDataGenerator.Randomizers;
using RandomDataGenerator.FieldOptions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sender
{
    internal class Program
    {
        static Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            GenerateMessages(host);
            
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

        private static async void GenerateMessages(IHost host)
        {
            using var serviceScope = host.Services.CreateScope();
            var provider = serviceScope.ServiceProvider;

            var adapter = provider.GetRequiredService<IRabbitMqAdapter>();
            var logger = provider.GetService<ILogger<Program>>();

            var processTime = 50; //milliseconds

            var randomizerFirstName = RandomizerFactory.GetRandomizer(new FieldOptionsFirstName());
            var randomizerLastName = RandomizerFactory.GetRandomizer(new FieldOptionsLastName());

            Console.WriteLine("Start sending events.");
            Console.WriteLine("Press <Enter> to terminate.");

            var pressedKey = ConsoleKey.NoName;
            do
            {
                Thread.Sleep(processTime);
                var createUserEvent = new CreateUserEvent
                {
                    FirstName = randomizerFirstName.Generate(),
                    LastName = randomizerLastName.Generate()
                };

                adapter?.Publish(
                    createUserEvent,
                    null,
                    (message) => logger.LogInformation(" [x] Sent {0}", message.ToString()));

                if (Console.KeyAvailable)
                {
                    pressedKey = Console.ReadKey().Key;
                }
            } while (pressedKey != ConsoleKey.Enter);

            await host.StopAsync();
        }
    }
}
