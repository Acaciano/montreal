namespace Montreal.Core.Crosscutting.Communication.RabbitMQ
{
    public interface IRabbitMQProducer
    {
        bool Send(string routingKey, string value);
        bool Send(string exchange, string routingKey, string value);
    }
}
