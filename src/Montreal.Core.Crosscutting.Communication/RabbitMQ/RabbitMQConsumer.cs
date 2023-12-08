using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace Montreal.Core.Crosscutting.Communication.RabbitMQ
{
    public class RabbitMQConsumer : IRabbitMQConsumer
    {
        private readonly IConnection _connection;
        private EventingBasicConsumer _consumer = null;
        private IModel _channel = null;

        public RabbitMQConsumer(IRabbitMQConnection rabbitMQConnection)
        {
            this._connection = rabbitMQConnection.Connection;
        }

        public void Consume(string queueName, Func<BasicDeliverEventArgs, IModel, Task> messageHandler)
        {

            this._channel = this._connection.CreateModel();
            this._channel.BasicQos(0, 1, false);

            this._consumer = new EventingBasicConsumer(this._channel);

            this._consumer.Received += async (sender, eve) =>
            {
                System.Diagnostics.Debug.WriteLine("## Received");
                await messageHandler(eve, this._channel);
                System.Diagnostics.Debug.WriteLine("## Received Completed");
            };

            this._channel.BasicConsume(queue: queueName, autoAck: false, consumer: this._consumer);
        }

        public void Consume(string exchangeName, string queueName, Func<BasicDeliverEventArgs, IModel, Task> messageHandler)
        {
            this._channel = this._connection.CreateModel();
            this._channel.BasicQos(0, 1, false);

            this._channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);

            this._channel.QueueDeclare(queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

            this._channel.QueueBind(queueName, exchangeName, "");

            this._consumer = new EventingBasicConsumer(this._channel);

            this._consumer.Received += async (sender, eve) =>
            {
                System.Diagnostics.Debug.WriteLine("## Received");
                await messageHandler(eve, this._channel);
                System.Diagnostics.Debug.WriteLine("## Received Completed");
            };

            this._channel.BasicConsume(queue: queueName, autoAck: false, consumer: this._consumer);
        }
    }
}
