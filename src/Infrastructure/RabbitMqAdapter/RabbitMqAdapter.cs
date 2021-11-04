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
            string queue,
            string message,
            Action<string> afterSendCallback = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queue,
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(string.Empty, queue, properties, body);

            afterSendCallback?.Invoke(message);
        }

        public Task StartListen(
            string queue,
            Action<string> action,
            CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(cancellationToken);

            return taskFactory.StartNew(() =>
            {
                using var connection = _connectionFactory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: queue,
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    action.Invoke(message);
                    ((EventingBasicConsumer)sender).Model.BasicAck(ea.DeliveryTag, false);
                };

                channel.BasicConsume(queue, false, consumer);

                while (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(1);
                }
            }, cancellationToken);
        }
    }
}
