[![sarmaad MyGet Build Status](https://www.myget.org/BuildSource/Badge/sarmaad?identifier=597fb48d-48b3-47df-8517-364d65d5a22e)](https://www.myget.org/)

# WebJobs.Extensions.RabbitMQ

This is an extension to Azure WebJobs adding RabbitMQ support.

<b>RabbitQueueTriggerAttribute</b>: this attribute will subscribe to the queue and triggers whenever a message arrives.
```
[RabbitQueueTrigger("queueName")]
```

<b>RabbitQueueBinderAttribute</b>: this attribute extends RabbitQueueTriggerAttribute to allow for dynamic creation of the queue and bind it to the exchange.
```
[RabbitQueueBinder("exchangeName", "routingKey", "errorExchangeName(optional)","autoDelete=false(optional)","durable=true(optional)","exclusive=false(optional)")]
```

<b>RabbitMessageAttribute</b>: this attribute allows you to publish a message to an exchange.
```
[RabbitMessage("exchangeName","routingKey","mandatory=false(optional)"]
```

Blog Post: http://www.sarmaad.com/2016/11/azure-webjobs-and-rabbitmq/
