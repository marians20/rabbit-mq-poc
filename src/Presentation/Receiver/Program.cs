using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMqAdapter;

namespace Receiver
{
    class Program
    {
        static void Main()
        {
            var adapter = GetRabbitMqAdapter();

            var routingKey = "CreateUserEvent";
            var processTime = 100; //milliseconds

            var ctSource = new CancellationTokenSource();
            var task = adapter?.StartListen(
                routingKey,
                (message) => ProcessMessage(message, processTime),
                ctSource.Token);

            Console.WriteLine("Press <Enter> to terminate...");

            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            WaitForEnter(task, ctSource);
        }

        private static void ProcessMessage(string message, int processTime)
        {
            Thread.Sleep(processTime);
            Console.WriteLine(" [x] Received {0}", message);
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

        private static void WaitForEnter(Task task, CancellationTokenSource ctSource)
        {
            while (task is {IsCanceled: false, IsCompleted: false})
            {
                Thread.Sleep(1);
                if (!Console.KeyAvailable)
                {
                    continue;
                }

                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    ctSource.Cancel();
                }
            }
        }
    }
}
