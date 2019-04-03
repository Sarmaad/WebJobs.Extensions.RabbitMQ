// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Triggers;
using RabbitMQ.Client;

namespace WebJobs.Extensions.RabbitMQ.Binding
{
    internal class RabbitQueueTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        private readonly IConnection _connection;


        public RabbitQueueTriggerAttributeBindingProvider(IConnection connection)
        {
            _connection = connection;
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var parameter = context.Parameter;
            var attribute = parameter.GetCustomAttribute<RabbitQueueTriggerAttribute>(false);
            if (attribute == null) return Task.FromResult<ITriggerBinding>(null);

            var queueBinderAttribute = parameter.GetCustomAttribute<RabbitQueueBinderAttribute>(false);

            return Task.FromResult<ITriggerBinding>(new RabbitQueueTriggerBinding(_connection, attribute.QueueName, queueBinderAttribute, context.Parameter));
        }
    }
}