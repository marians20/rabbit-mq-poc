using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMqAdapter;
using System;

namespace Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddRabbitMqDataAdapter(configuration)
                .BuildServiceProvider();

            var adapter = serviceProvider.GetService<IRabbitMqAdapter>();
            adapter.OnReceived("q", "rk", (msg) => Console.WriteLine(msg));

            adapter.Publish("q", "rk", "Ai scapat sapunul!", (message) => Console.WriteLine(" [x] Sent {0}", message));

        }
    }
}
