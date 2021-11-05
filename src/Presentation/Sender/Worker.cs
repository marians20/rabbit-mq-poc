using System;
using System.Threading;
using System.Threading.Tasks;
using Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMqAdapter;

namespace Sender
{
    internal sealed class Worker: BackgroundService
    {
        private readonly IRabbitMqAdapter _adapter;
        private readonly ILogger<Worker> _logger;
        private readonly int _processTime = 50; //mlliseconds
        public Worker(IRabbitMqAdapter adapter, ILogger<Worker> logger)
        {
            _adapter = adapter;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Thread.Sleep(_processTime);
                _adapter?.Publish(
                    CreateUserEvent.GetRandomCreateUserEvent(),
                    null,
                    (message) => _logger.LogInformation(" [x] Sent {0}", message.ToString()));
            }
        }
    }
}
