using System;
using System.Text;
using System.Text.Json;
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

        ///<inheritdoc/>
        public void Publish(
            string message,
            string queue,
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

        ///<inheritdoc/>
        public void Publish<T>(
            T message,
            string queue,
            Action<T> afterSendCallback = null) where T : class =>
            Publish(
                JsonSerializer.Serialize(message),
                queue ?? typeof(T).Name,
                msg => afterSendCallback(JsonSerializer.Deserialize<T>(msg)));

        ///<inheritdoc/>
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

        ///<inheritdoc/>
        public Task StartListen<T>(string queue, Action<T> action, CancellationToken cancellationToken)
             where T : class =>
            StartListen(queue ?? typeof(T).Name, msg => action(JsonSerializer.Deserialize<T>(msg)), cancellationToken);
    }
}
