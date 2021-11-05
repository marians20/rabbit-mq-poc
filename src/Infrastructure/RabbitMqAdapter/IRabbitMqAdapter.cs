using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqAdapter
{
    public interface IRabbitMqAdapter
    {
        /// <summary>
        /// Adds a string message to a queue
        /// </summary>
        /// <param name="message">The message to be put in queue</param>
        /// <param name="queue">The name of the queue</param>
        /// <param name="afterSendCallback"></param>
        void Publish(
            string message,
            string queue,
            Action<string> afterSendCallback = null);

        /// <summary>
        /// Adds an object of T Type to a queue<br/>
        /// If queue name is not provided (null), it will use tha name of the message object type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The object to be put in queue</param>
        /// <param name="queue">The name of the queue. If it is nul, the name of the queue will be the name of the message type</param>
        /// <param name="afterSendCallback"></param>
        void Publish<T>(
            T message,
            string queue,
            Action<T> afterSendCallback = null) where T: class;

        Task StartListen(
            string queue,
            Action<string> action,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets messages and parses them to objects of type T from a queue.<br/>
        /// If gueue name is not specified (null), the name of message type will be used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartListen<T>(
            string queue,
            Action<T> action,
            CancellationToken cancellationToken) where T: class;
    }
}