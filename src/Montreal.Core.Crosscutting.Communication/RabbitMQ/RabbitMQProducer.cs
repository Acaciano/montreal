using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Montreal.Core.Crosscutting.Communication.RabbitMQ
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQProducer> _logger;

        public RabbitMQProducer(IRabbitMQConnection rabbitMQConnection, ILogger<RabbitMQProducer> logger)
        {
            this._connection = rabbitMQConnection.Connection;
            this._channel = this._connection.CreateModel();

            this._logger = logger;
        }

        public bool Send(string routingKey, string value)
        {
            return this.Send(string.Empty, routingKey, value);
        }

        public bool Send(string exchange,  string routingKey, string value)
        {
            
            byte[] body = null;

            try
            {
                body = Encoding.UTF8.GetBytes(value);

                this._channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: null, body: body);
                    
                return true;
            }
            catch (Exception ex)
            {
                this._logger.LogError("RABBIT PRODUCER: " + ex.Message + "\r\n\r\n" + ex.ToString());
                return false;
            }
        }
    }
}
