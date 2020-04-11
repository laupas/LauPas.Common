using System;
using System.Text;
using System.Threading.Tasks;
using LauPas.Azure.Model;
using LauPas.Common;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace LauPas.Azure
{
    [Singleton]
    internal class AzureServiceBus : IAzureServiceBus
    {
        private ILogger logger;
        private readonly AzureServiceBusConfiguration config;

        public AzureServiceBus(ILoggerFactory loggerFactory, IConfigService configService)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType().Name);
            this.config = configService.Get<AzureServiceBusConfiguration>("AzureServiceBusConfiguration");

        }
        public void ListenToQueue(string queueName, Action<string> data)
        {
            var queueClient = new QueueClient(this.config.ConnectionString, queueName);
            var messageHandlerOptions = this.GetMessageHandlerOptions();

            // Register the function that processes messages.
            queueClient.RegisterMessageHandler(async (message, token) =>
            {
                data.Invoke(Encoding.UTF8.GetString(message.Body));
                await queueClient.CompleteAsync(message.SystemProperties.LockToken);

            }, messageHandlerOptions);
        }

        private MessageHandlerOptions GetMessageHandlerOptions()
        {
            return new MessageHandlerOptions(exceptionReceivedEventArgs =>
            {
                var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
                this.logger.LogError(exceptionReceivedEventArgs.Exception, $"Endpoint: {context.Endpoint} Entity Path: {context.EntityPath} Executing Action: {context.Action}");
                return Task.CompletedTask;
            })
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };
        }

        public void ListenToTopic(string topicName, string subscriptionName, Action<string> data)
        {
            var subscriptionClient = new SubscriptionClient(this.config.ConnectionString, topicName, subscriptionName);
            var messageHandlerOptions = this.GetMessageHandlerOptions();

            // Register the function that processes messages.
            subscriptionClient.RegisterMessageHandler(async (message, token) =>
            {
                data.Invoke(Encoding.UTF8.GetString(message.Body));
                await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);

            }, messageHandlerOptions);
        }

        public async Task SendToQueueAsync(string queueName, string data)
        {
            var queueClient = new QueueClient(this.config.ConnectionString, queueName);
            var message = new Message(Encoding.UTF8.GetBytes(data));
            
            await queueClient.SendAsync(message);
            await queueClient.CloseAsync();
        }

        public async Task SendToTopicAsync(string topicName, string data)
        {
            var topicClient = new TopicClient(this.config.ConnectionString, topicName);
            var message = new Message(Encoding.UTF8.GetBytes(data));
            
            await topicClient.SendAsync(message);
            await topicClient.CloseAsync();
        }
    }
}