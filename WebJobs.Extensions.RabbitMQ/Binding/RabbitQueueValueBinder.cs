using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Microsoft.Azure.WebJobs.Extensions.Bindings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebJobs.Extensions.RabbitMQ.Binding
{
    internal class RabbitQueueValueBinder : ValueBinder
    {
        readonly RabbitQueueTriggerValue _value;

        public RabbitQueueValueBinder(ParameterInfo parameter, RabbitQueueTriggerValue value): base(parameter.ParameterType)
        {
            _value = value;
        }

        public override object GetValue()
        {
            // if its a string, just return the body of the message as string
            if (Type == typeof(string))
            {
                return Encoding.UTF8.GetString(_value.MessageBytes);
            }

            if (Type == typeof(JObject))
            {
                return JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(_value.MessageBytes));
            }

            
            // deserialize object based on the settings
            var obj = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(_value.MessageBytes),
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,

                });



            return obj;
        }
        public override string ToInvokeString()
        {
            return Encoding.UTF8.GetString(_value.MessageBytes);
        }
    }
}