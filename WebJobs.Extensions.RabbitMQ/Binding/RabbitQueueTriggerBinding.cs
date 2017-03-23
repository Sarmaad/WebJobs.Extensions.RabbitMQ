using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using RabbitMQ.Client;
using WebJobs.Extensions.RabbitMQ.Listener;

namespace WebJobs.Extensions.RabbitMQ.Binding
{
    internal class RabbitQueueTriggerBinding : ITriggerBinding
    {
        readonly IConnection _connection;
        readonly string _queueName;
        readonly RabbitQueueBinderAttribute _queueBinderAttribute;
        readonly ParameterInfo _parameter;
        readonly IReadOnlyDictionary<string, Type> _bindingContract;

        public RabbitQueueTriggerBinding(IConnection connection, string queueName, RabbitQueueBinderAttribute queueBinderAttribute, ParameterInfo parameter)
        {
            _connection = connection;
            _queueName = queueName;
            _queueBinderAttribute = queueBinderAttribute;
            _parameter = parameter;
            _bindingContract = CreateBindingDataContract();
        }

        public IReadOnlyDictionary<string, Type> BindingDataContract
        {
            get { return _bindingContract; }
        }

        public Type TriggerValueType
        {
            get { return typeof(RabbitQueueTriggerValue); }
        }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            var triggerValue = value as RabbitQueueTriggerValue;
            IValueBinder valueBinder = new RabbitQueueValueBinder(_parameter, triggerValue);
            return Task.FromResult<ITriggerData>(new TriggerData(valueBinder, GetBindingData(triggerValue)));
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            if (_queueBinderAttribute != null)
            {
                // we need to dynamically declare and bind the queue
                
                var args = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(_queueBinderAttribute.ErrorExchange))
                {
                    args.Add("x-dead-letter-exchange", _queueBinderAttribute.ErrorExchange);
                    args.Add("x-dead-letter-routing-key", _queueName);
                }

                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(_queueName, _queueBinderAttribute.Durable, _queueBinderAttribute.Execlusive, _queueBinderAttribute.AutoDelete,args);
                    channel.QueueBind(_queueName, _queueBinderAttribute.Exchange,_queueBinderAttribute.RoutingKey);
                }
            }


            return Task.FromResult<IListener>(new RabbitQueueListener(_connection.CreateModel(),_queueName, context.Executor));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new RabbitQueueTriggerParameterDescriptor
            {
                Name = _parameter.Name,
                
            };
        }

        IReadOnlyDictionary<string, object> GetBindingData(RabbitQueueTriggerValue value)
        {
            var bindingData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            bindingData.Add("RabbitQueueTrigger", value);

            
            return bindingData;
        }

        IReadOnlyDictionary<string, Type> CreateBindingDataContract()
        {
            var contract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            contract.Add("RabbitQueueTrigger", typeof(RabbitQueueTriggerValue));

            return contract;
        }
    }
}