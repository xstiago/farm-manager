using FarmMonitor.Domain.Dtos;
using FarmMonitor.Domain.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FarmMonitor.Infrastructure.Messaging
{
    public class RabbitPublisher<TEvent> : RabbitBase, IMessagingPublisher<TEvent>
        where TEvent : class
    {
        private readonly string _brokerName;

        public RabbitPublisher(IMessagingConnection messagingConnection, string brokerName)
            : base(messagingConnection)
        {
            _brokerName = brokerName;
        }

        public void Publish(EventDto<TEvent> @event)
        {
            using var channel = _connection.CreateModel();

            var routingKey = $"{_brokerName}_key";

            channel.ExchangeDeclare(exchange: Exchange, type: "direct");
            channel.QueueDeclare(queue: _brokerName, true, false, false, null);
            channel.QueueBind(queue: _brokerName, Exchange, routingKey);

            var message = JsonSerializer.Serialize(@event);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: Exchange,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }
    }
}
