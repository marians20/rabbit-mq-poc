using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqAdapter
{
    public interface IRabbitMqAdapter
    {
        void Publish(
            string exchange,
            string routingKey,
            string message,
            Action<string> afterSendCallback = null);
        
        Task StartListen(
            string queue,
            string routingKey,
            Action<string> action,
            CancellationToken cancellationToken);
    }
}