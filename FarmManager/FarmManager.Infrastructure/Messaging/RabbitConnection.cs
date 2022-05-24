using FarmManager.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace FarmManager.Infrastructure.Messaging
{
    public class RabbitConnection : IMessagingConnection
    {
        public RabbitConnection(IConfiguration configuration)
        {
            HostName = configuration.GetSection("RabbitHostName").Value;
            UserName = configuration.GetSection("RabbitUserName").Value;
            Password = configuration.GetSection("RabbitPassword").Value;
            Port = AmqpTcpEndpoint.UseDefaultPort;
        }

        public string HostName { get; private set; }

        public string UserName { get; private set; }

        public string Password { get; private set; }

        public int Port { get; }
    }
}
