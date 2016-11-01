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
        readonly IModel _channel;
        readonly string _queueName;
        readonly ITriggeredFunctionExecutor _executor;
        EventingBasicConsumer _consumer;
        
        string _consumerTag;


        public RabbitQueueListener(IModel channel, string queueName,  ITriggeredFunctionExecutor executor)
        {
            _channel = channel;
            _queueName = queueName;
            _executor = executor;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += (sender, args) =>
            {
                var result = _executor.TryExecuteAsync(
                    new TriggeredFunctionData {TriggerValue = new RabbitQueueTriggerValue {MessageBytes = args.Body}},
                    CancellationToken.None).Result;

                if (result.Succeeded)
                    _channel.BasicAck(args.DeliveryTag, false);
                else
                    _channel.BasicNack(args.DeliveryTag, false, false);

            };

            _consumerTag = _channel.BasicConsume(_queueName, false, _consumer);
            
            return Task.FromResult(true);
        }

        

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer = null;
            
            return Task.FromResult(true);
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