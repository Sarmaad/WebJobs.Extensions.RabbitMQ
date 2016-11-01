using System;

namespace WebJobs.Extensions.RabbitMQ
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RabbitMessageAttribute : Attribute
    {
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public bool Mandatory { get; set; }

        public RabbitMessageAttribute(string exchange, string routingKey, bool mandatory = false)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            Mandatory = mandatory;
        }
    }
    
}
