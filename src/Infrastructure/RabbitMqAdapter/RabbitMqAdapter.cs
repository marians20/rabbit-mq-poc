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
        private readonly IConnectionFactory _connectionFactory;
        public RabbitMqAdapter(RabbitMqSettings settings)
        {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = settings.HostName,
                VirtualHost = settings.VirtualHost,
                UserName = settings.UserName,
                Password = settings.Password
            };
        }

        public void Publish(
            string exchange,
            string routingKey,
            string message,
            Action<string> afterSendCallback = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange, ExchangeType.Fanout);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange, routingKey, null, body);

            afterSendCallback?.Invoke(message);
        }

        public Task StartListen(
            string exchange,
            string routingKey,
            Action<string> action,
            CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(cancellationToken);

            return taskFactory.StartNew(() =>
            {
                using var connection = _connectionFactory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.ExchangeDeclare(exchange, ExchangeType.Fanout);
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName, exchange, routingKey);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (_, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    action.Invoke(message);
                };
                channel.BasicConsume(queueName, true, consumer);

                while (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(0);
                }
            }, cancellationToken);
        }
    }
}
