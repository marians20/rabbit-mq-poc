using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqAdapter
{
    internal sealed class RabbitMqAdapter : IRabbitMqAdapter
    {
        private readonly RabbitMqSettings _settings;
        private readonly IConnectionFactory _connectionFactory;
        public RabbitMqAdapter(RabbitMqSettings settings)
        {
            _settings = settings;
            _connectionFactory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                VirtualHost = _settings.VirtualHost,
                UserName = _settings.UserName,
                Password = _settings.Password
            };
        }

        public void Publish(string exchange, string routingKey, string message, Action<string> afterSendCallback = null)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: exchange,
                        routingKey: routingKey,
                        basicProperties: null,
                        body: body);
                    if (afterSendCallback != null)
                    {
                        afterSendCallback(message);
                    }
                }
            }
        }

        public Task StartListen(string exchange, string routingKey, Action<string> action, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout);
                        var queueName = channel.QueueDeclare().QueueName;
                        channel.QueueBind(queue: queueName,
                            exchange: exchange,
                            routingKey: routingKey);
                        Console.WriteLine(" [*] Waiting for logs.");
                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            action(message);
                        };
                        channel.BasicConsume(queue: queueName,
                            autoAck: true,
                            consumer: consumer);

                        while (!cancellationToken.IsCancellationRequested)
                        {
                            Thread.Sleep(0);
                            if (cancellationToken.IsCancellationRequested)
                            {
                                Console.WriteLine("Task cancelled.");
                            }
                        }
                    }
                }

            }, cancellationToken);
        }
    }
}
