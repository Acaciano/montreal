using RabbitMQ.Client;

namespace Montreal.Core.Crosscutting.Communication.RabbitMQ
{
    public interface IRabbitMQConnection
    {
        IConnection Connection { get; }
    }
}
