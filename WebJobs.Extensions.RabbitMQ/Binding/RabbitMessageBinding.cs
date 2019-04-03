using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using RabbitMQ.Client;

namespace WebJobs.Extensions.RabbitMQ.Binding
{
    internal class RabbitMessageBinding : IBinding
    {
        private readonly IConnection _connection;
        private readonly string _exchange;
        private readonly bool _mandatory;
        private readonly ParameterInfo _parameter;
        private readonly string _routingKey;

        public RabbitMessageBinding(IConnection connection, string exchange, string routingKey, bool mandatory, ParameterInfo parameter)
        {
            _connection = connection;
            _exchange = exchange;
            _routingKey = routingKey;
            _mandatory = mandatory;
            _parameter = parameter;
        }

        public bool FromAttribute => true;

        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            return Task.FromResult<IValueProvider>(new RabbitMessageValueBinder(_connection, _exchange, _routingKey, _mandatory, _parameter));
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            return Task.FromResult<IValueProvider>(new RabbitMessageValueBinder(_connection, _exchange, _routingKey, _mandatory, _parameter));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor
            {
                Name = _parameter.Name,
                DisplayHints = new ParameterDisplayHints
                {
                    Description = "RabbitMessage",
                    DefaultValue = "string",
                    Prompt = "Please enter a string value"
                }
            };
        }
    }
}