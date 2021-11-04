using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMqAdapter;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Events;

namespace Sender
{
    internal class Program
    {
        static void Main()
        {
            var adapter = GetRabbitMqAdapter();

            var exchange = "logs";
            var routingKey = "xxx";
            var createUserEvent = new CreateUserEvent
            {
                FirstName = "John",
                LastName = "Doe"
            };

            adapter?.Publish(
                exchange,
                routingKey,
                createUserEvent.ToString(),
                (message) => Console.WriteLine(" [x] Sent {0}", message));
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
