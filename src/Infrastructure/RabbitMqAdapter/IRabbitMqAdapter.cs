using System;

namespace RabbitMqAdapter
{
    public interface IRabbitMqAdapter
    {
        void Publish(string queue, string routingKey, string message, Action<string> afterSendCallback = null);
        void OnReceived(string queue, string routingKey, Action<string> action);
    }
}