using FarmManager.Domain.Interfaces;
using RabbitMQ.Client;

namespace FarmManager.Infrastructure.Messaging
{
    public abstract class RabbitBase : IDisposable
    {
        private static IConnection CreateConnection(ConnectionFactory connectionFactory)
        {
            return connectionFactory.CreateConnection();
        }

        private static ConnectionFactory GetConnectionFactory(IMessagingConnection messagingConnection)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = messagingConnection.HostName,
                UserName = messagingConnection.UserName,
                Password = messagingConnection.Password,
                Port = messagingConnection.Port
            };

            return connectionFactory;
        }

        protected const string Exchange = "master";
        protected readonly IConnection _connection;

        protected RabbitBase(IMessagingConnection messagingConnection)
        {
            var factory = GetConnectionFactory(messagingConnection);
            _connection = CreateConnection(factory);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _connection?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
