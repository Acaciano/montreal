using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using Montreal.Core.Crosscutting.Communication.Kafka;
using Montreal.Core.Crosscutting.Domain.Enum;

namespace Montreal.Core.Tests.Services
{
    public class KafkaProducerTest
    {
        public IConfiguration Configuration { get; }
        public IServiceCollection Services;

        public KafkaProducerTest()
        {
            string APP_SETTINGS = "appsettings.json";

            Configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile(APP_SETTINGS, optional: true, reloadOnChange: true)
              .AddEnvironmentVariables()
              .Build();
        }

        [Test]
        public async Task ProducerTestAsync()
        {
            string communicationKafka = Configuration.GetValue<string>("Communication:Kafka");
            var producer = new KafkaProducer();
            var persistenceStatus = await producer.SendAsync(communicationKafka, "teste", Guid.NewGuid().ToString(), "teste de mensagem");

            Assert.IsTrue(persistenceStatus == PersistenceStatusEnum.Persisted);
        }
    }
}