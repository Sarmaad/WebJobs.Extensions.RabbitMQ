using System;
using Microsoft.Azure.WebJobs;

namespace WebJobs.Extensions.RabbitMQ.Config
{
    public static class RabbitMqJobHostConfigurationExtensions
    {
        public static void UseRabbitMq(this JobHostConfiguration config, string serverEndpoint, bool autoConnectionRecovery = true)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            // Register our extension configuration provider
            config.RegisterExtensionConfigProvider(new RabbitMqExtensionConfig(serverEndpoint, autoConnectionRecovery));
        }
    }
}