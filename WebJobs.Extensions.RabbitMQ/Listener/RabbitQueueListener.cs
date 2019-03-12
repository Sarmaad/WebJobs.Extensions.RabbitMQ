using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WebJobs.Extensions.RabbitMQ.Listener
{
    internal class RabbitQueueListener : IListener
    {
        private readonly IModel _channel;
        private readonly ITriggeredFunctionExecutor _executor;
        private readonly string _queueName;
        private EventingBasicConsumer _consumer;
        private string _consumerTag;

        public RabbitQueueListener(IModel channel, string queueName, ITriggeredFunctionExecutor executor)
        {
            _channel = channel;
            _queueName = queueName;
            _executor = executor;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += async (sender, args) =>
            {
                var triggerValue = new RabbitQueueTriggerValue { MessageBytes = args.Body };
                if (args.BasicProperties != null)
                {
                    triggerValue.MessageId = args.BasicProperties.MessageId;
                    triggerValue.ApplicationId = args.BasicProperties.AppId;
                    triggerValue.ContentType = args.BasicProperties.ContentType;
                    triggerValue.CorrelationId = args.BasicProperties.CorrelationId;
                    triggerValue.Headers = args.BasicProperties.Headers;
                }

                var result = await _executor.TryExecuteAsync(new TriggeredFunctionData {TriggerValue = triggerValue}, CancellationToken.None)
                    .ConfigureAwait(false);

                if (result.Succeeded)
                    _channel.BasicAck(args.DeliveryTag, false);
                else
                    _channel.BasicNack(args.DeliveryTag, false, false);
            };

            _consumerTag = _channel.BasicConsume(_queueName, false, _consumer);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer = null;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel.Close();
            _channel.Dispose();
        }

        public void Cancel()
        {
            _channel.Abort();
        }
    }
}