using System.Collections.Generic;

namespace WebJobs.Extensions.RabbitMQ
{
    public sealed class MessageContext
    {
        public string MessageId { get; set; }
        public string ApplicationId { get; set; }
        public string ContentType { get; set; }
        public string CorrelationId { get; set; }
        public IDictionary<string, object> Headers { get; set; }
        public object Message { get; set; }
    }

   
}
