using Confluent.Kafka;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Montreal.Core.Crosscutting.Domain.Enum;

namespace Montreal.Core.Crosscutting.Communication.Kafka
{
    public class KafkaProducer : IKafkaProducer
    {
        public async Task<PersistenceStatusEnum> SendAsync(string bootstrapServers, string topic, string key, string value)
        {
            var logger = new LoggerConfiguration().CreateLogger();

            try
            {
                int messageMaxBytes = 1024 * 1024 * 40;
                var config = new ProducerConfig
                {
                    BootstrapServers = bootstrapServers,
                    MessageMaxBytes = messageMaxBytes,
                };

                using (var producer = new ProducerBuilder<string, string>(config).Build())
                {
                    var message = new Message<string, string> { Key = key , Value = value };

                    var result = await producer.ProduceAsync(topic, message);

                    logger.Information("Concluído o envio de mensagem");

                    return result.Status switch
                    {
                        PersistenceStatus.NotPersisted => PersistenceStatusEnum.NotPersisted,
                        PersistenceStatus.PossiblyPersisted => PersistenceStatusEnum.PossiblyPersisted,
                        PersistenceStatus.Persisted => PersistenceStatusEnum.Persisted,
                        _ => PersistenceStatusEnum.NotPersisted,
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Exceção: {ex.GetType().FullName} | " + $"Mensagem: {ex.Message}");
                return PersistenceStatusEnum.NotPersisted;
            }
        }
    }
}
