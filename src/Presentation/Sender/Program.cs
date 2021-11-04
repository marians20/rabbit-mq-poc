using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMqAdapter;
using System;
using Events;
using RandomDataGenerator.Randomizers;
using RandomDataGenerator.FieldOptions;
using System.Threading;

namespace Sender
{
    internal class Program
    {
        static void Main()
        {
            var adapter = GetRabbitMqAdapter();

            var routingKey = "CreateUserEvent";
            var processTime = 50; //milliseconds

            var randomizerFirstName = RandomizerFactory.GetRandomizer(new FieldOptionsFirstName());
            var randomizerLastName = RandomizerFactory.GetRandomizer(new FieldOptionsLastName());

            Console.WriteLine("Start sending events.");
            Console.WriteLine("Press <Enter> to terminate.");

            ConsoleKey pressedKey = ConsoleKey.NoName;
            do
            {
                Thread.Sleep(processTime);
                var createUserEvent = new CreateUserEvent
                {
                    FirstName = randomizerFirstName.Generate(),
                    LastName = randomizerLastName.Generate()
                };

                adapter?.Publish(
                    routingKey,
                    createUserEvent.ToString(),
                    (message) => Console.WriteLine(" [x] Sent {0}", message));

                if(Console.KeyAvailable)
                {
                    pressedKey = Console.ReadKey().Key;
                }
            } while (pressedKey != ConsoleKey.Enter);
        }

        private static IRabbitMqAdapter GetRabbitMqAdapter()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddRabbitMqDataAdapter(configuration)
                .BuildServiceProvider();

            var adapter = serviceProvider.GetService<IRabbitMqAdapter>();
            return adapter;
        }
    }
}
