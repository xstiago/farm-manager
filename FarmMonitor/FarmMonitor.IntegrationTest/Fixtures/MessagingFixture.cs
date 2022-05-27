using FarmMonitor.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;

namespace FarmMonitor.IntegrationTest.Fixtures
{
    public class MessagingFixture : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IConfiguration _configuration;

        protected virtual void Dispose(bool dispose)
        {
            if (dispose && _connection != null)
            {
                DeleteRabbitBroker(_configuration);
                _connection.Dispose();
            }
        }

        public MessagingFixture()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.IntegrationTest.json")
                .Build();

            var rabbitConnection = new RabbitConnection(_configuration);

            var connectionFactory = new ConnectionFactory
            {
                HostName = rabbitConnection.HostName,
                UserName = rabbitConnection.UserName,
                Password = rabbitConnection.Password
            };

            _connection = connectionFactory.CreateConnection();
        }

        public void DeleteRabbitBroker(IConfiguration configuration)
        {
            var brokerName = configuration.GetSection("BrokerName").Value;

            var model = _connection.CreateModel();
            model.ExchangeDelete(exchange: "master");
            model.QueueDelete(brokerName);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
