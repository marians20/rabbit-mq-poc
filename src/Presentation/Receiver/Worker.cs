using System.Threading;
using System.Threading.Tasks;
using Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMqAdapter;

namespace Receiver
{
    class Worker: BackgroundService
    {
        private readonly IRabbitMqAdapter _adapter;
        private readonly ILogger<Worker> _logger;
        private readonly int _processTime = 100; //milliseconds

        public Worker(IRabbitMqAdapter adapter, ILogger<Worker> logger)
        {
            _adapter = adapter;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var ctSource = new CancellationTokenSource();
            var task = _adapter.StartListen<CreateUserEvent>(
                null,
                (message) =>
                {
                    Thread.Sleep(_processTime);
                    _logger.LogInformation(" [x] Received {0}", message.ToString());
                },
                ctSource.Token);
            while (!stoppingToken.IsCancellationRequested)
            {
                Thread.Sleep(10);
            }
        }
    }
}
