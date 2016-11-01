using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using RabbitMQ.Client;

namespace WebJobs.Extensions.RabbitMQ.Binding
{
    internal class RabbitMessageAttributeBindingProvider : IBindingProvider
    {
        readonly IConnection _connection;

        public RabbitMessageAttributeBindingProvider(IConnection connection)
        {
            _connection = connection;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // Determine whether we should bind to the current parameter
            var parameter = context.Parameter;
            var attribute = parameter.GetCustomAttribute<RabbitMessageAttribute>(inherit: false);
            if (attribute == null)
            {
                return Task.FromResult<IBinding>(null);
            }

            ////// TODO: Include any other parameter types this binding supports in this check
            //IEnumerable<Type> supportedTypes = new List<Type>
            //{
            //    typeof(IEnumerable),
            //    typeof(string),
            //    typeof(object)
            //};
            //if (!ValueBinder.MatchParameterType(context.Parameter, supportedTypes))
            //{
            //    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
            //        "Can't bind RabbitMessageAttribute to type '{0}'.", parameter.ParameterType));
            //}

            return Task.FromResult<IBinding>(new RabbitMessageBinding(_connection, attribute.Exchange, attribute.RoutingKey,attribute.Mandatory, parameter));
        }

        
    }
}
