using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Net.Security;

namespace Montreal.Core.Crosscutting.Communication.RabbitMQ
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private IConnection     _connection;
        private readonly string _host;
        private readonly int    _port;
        private readonly string _user;
        private readonly string _password;
        private readonly string _virtualHost;
        private readonly bool   _useSSL;
        private readonly string _SSLServerName;
        private readonly string _SSLCertPath;       

        public RabbitMQConnection(IConfiguration configuration)
        {
            this._host = configuration.GetValue<string>("RabbitMQ:Host");
            this._port = configuration.GetValue<int>("RabbitMQ:Port");
            this._user = configuration.GetValue<string>("RabbitMQ:User");
            this._password = configuration.GetValue<string>("RabbitMQ:Password");
            this._virtualHost = configuration.GetValue<string>("RabbitMQ:VirtualHost");

            this._useSSL = configuration.GetValue<bool>("RabbitMQ:UseSSL");

            if (this._useSSL)
            {
                this._SSLServerName = configuration.GetValue<string>("RabbitMQ:SSLServerName");
                this._SSLCertPath = configuration.GetValue<string>("RabbitMQ:SSLCertPath");
            }

            this.CreateConnection();
        }

        private void CreateConnection()
        {
            ConnectionFactory factory = null;

            factory = new ConnectionFactory()
            {
                HostName = this._host,
                Port = this._port,
                UserName = this._user,
                Password = this._password,
                VirtualHost = this._virtualHost,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            if (this._useSSL)
            {
                factory.Ssl.Enabled = true;
                factory.Ssl.ServerName = this._SSLServerName;
                factory.Ssl.CertPath = this._SSLCertPath;
                factory.Ssl.AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch
                    | SslPolicyErrors.RemoteCertificateNotAvailable
                    | SslPolicyErrors.RemoteCertificateChainErrors;
            }

            this._connection = factory.CreateConnection();
        }

        public IConnection Connection => this._connection;
    }
}
