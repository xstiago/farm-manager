using FarmManager.Domain.Dtos;
using FarmManager.Domain.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FarmManager.Infrastructure.Messaging
{
    public class RabbitSubscriber<TEvent> : RabbitBase, IMessagingSubscriber<TEvent>
        where TEvent : class
    {
        private readonly string _brokerName;

        public RabbitSubscriber(IMessagingConnection messagingConnection, string brokerName)
            : base(messagingConnection)
        {
            _brokerName = brokerName;
        }

        public EventDto<TEvent>? RetrieveSingleMessage()
        {
            BasicGetResult data;

            using var channel = _connection.CreateModel();

            data = channel.BasicGet(_brokerName, true);

            if (data != null)
            {
                var body = Encoding.UTF8.GetString(data.Body.ToArray());
                return JsonSerializer.Deserialize<EventDto<TEvent>>(body);
            }

            return null;
        }
    }
}
