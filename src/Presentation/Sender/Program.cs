using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMqAdapter;
using System;
using System.Threading;

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

            var ctSource = new CancellationTokenSource();
            var task = adapter.StartListen("logs", "xxx", (msg) => Console.WriteLine(msg), ctSource.Token);

            adapter.Publish("logs", "xxx", "Ai scapat sapunul!", (message) => Console.WriteLine(" [x] Sent {0}", message));

            while (!task.IsCanceled && !task.IsCompleted)
            {
                Thread.Sleep(0);
                if (Console.ReadKey().KeyChar > 0)
                {
                    Console.WriteLine("Cancelling");
                    ctSource.Cancel();
                }
            }



        }
    }
}
