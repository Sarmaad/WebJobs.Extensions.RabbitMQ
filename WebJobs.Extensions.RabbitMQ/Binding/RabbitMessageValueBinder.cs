using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.Bindings;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace WebJobs.Extensions.RabbitMQ.Binding
{
    internal class RabbitMessageValueBinder : ValueBinder
    {
        readonly IConnection _connection;
        readonly string _exchange;
        readonly string _routingKey;
        readonly bool _mandatory;
        readonly ParameterInfo _parameter;


        public RabbitMessageValueBinder(IConnection connection, string exchange,string routingKey,bool mandatory, ParameterInfo parameter) : base(parameter.ParameterType)
        {
            _connection = connection;
            _exchange = exchange;
            _routingKey = routingKey;
            _mandatory = mandatory;
            _parameter = parameter;
        }

        public override object GetValue()
        {
            if (Type.IsAbstract || Type.IsInterface || _parameter.IsOut) return null;

            return (Type)Activator.CreateInstance(Type);
        }

        public override string ToInvokeString()
        {
            return _parameter.Name;
        }

        public override Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            if (value == null || !_parameter.IsOut) return Task.FromResult(0);

            using (var channel = _connection.CreateModel())
            {
                channel.BasicReturn += (sender, args) =>
                {
                    if (_mandatory)
                    {
                        if (args.ReplyCode == 312)
                            throw new Exception("No Queue found to accept this message");
                    }
                    
                };

                if (value is string)
                    PublishMessage(channel, (string) value);

                else if (value is IEnumerable)
                    foreach (var message in (IEnumerable)value)
                        PublishMessage(channel, JsonMessage(message));

                else
                    PublishMessage(channel, JsonMessage(value));
            }
            
            return Task.FromResult(true);
        }

        string JsonMessage(object value)
        {
            return JsonConvert.SerializeObject(value,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,

                    });
        }

        void PublishMessage(IModel channel, string message)
        {
            channel.BasicPublish(_exchange, _routingKey,_mandatory, null, Encoding.UTF8.GetBytes(message));

        }
    }
}