using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using RabbitMQ.Client;
using WebJobs.Extensions.RabbitMQ.Binding;

namespace WebJobs.Extensions.RabbitMQ.Config
{
    internal class RabbitMqExtensionConfig : IExtensionConfigProvider
    {
        private readonly IConnection _connection;

        public RabbitMqExtensionConfig(string serverEndpoint, bool automaticRecovery = true)
        {
            var factory = new ConnectionFactory
            {
                Uri = serverEndpoint,
                AutomaticRecoveryEnabled = automaticRecovery
            };

            _connection = factory.CreateConnection();
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            // Register our extension binding providers
            context.Config.RegisterBindingExtensions(
                new RabbitQueueTriggerAttributeBindingProvider(_connection),
                new RabbitMessageAttributeBindingProvider(_connection));
        }
    }
}