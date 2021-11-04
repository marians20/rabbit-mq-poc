using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqAdapter
{
    public interface IRabbitMqAdapter
    {
        void Publish(
            string queue,
            string message,
            Action<string> afterSendCallback = null);
        
        Task StartListen(
            string queue,
            Action<string> action,
            CancellationToken cancellationToken);
    }
}