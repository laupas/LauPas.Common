using System;
using System.Threading.Tasks;

namespace LauPas.Azure
{
    /// <summary>
    /// IAzureServiceBus
    /// </summary>
    public interface IAzureServiceBus
    {
        /// <summary>
        /// Listen to a Queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="data"></param>
        void ListenToQueue(string queueName, Action<string> data);

        /// <summary>
        /// Listen to a Topic
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="data"></param>
        /// <param name="subscriptionName"></param>
        void ListenToTopic(string topicName, string subscriptionName, Action<string> data);

        /// <summary>
        /// Send data to a ServiceBus
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task SendToQueueAsync(string queueName, string data);

        /// <summary>
        /// Send data to Topic
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task SendToTopicAsync(string topicName, string data);
    }
}