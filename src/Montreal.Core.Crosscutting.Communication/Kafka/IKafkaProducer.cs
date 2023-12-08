using System.Threading.Tasks;
using Montreal.Core.Crosscutting.Domain.Enum;

namespace Montreal.Core.Crosscutting.Communication.Kafka
{
    public interface IKafkaProducer
    {
        Task<PersistenceStatusEnum> SendAsync(string bootstrapServers, string topic, string key, string value);
    }
}
