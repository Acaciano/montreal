using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace Montreal.Core.Crosscutting.Communication.RabbitMQ
{
    public interface IRabbitMQConsumer
    {
        void Consume(string queueName, Func<BasicDeliverEventArgs, IModel, Task> messageHandler);
        void Consume(string exchangeName, string queueName, Func<BasicDeliverEventArgs, IModel, Task> messageHandler);
    }
}
