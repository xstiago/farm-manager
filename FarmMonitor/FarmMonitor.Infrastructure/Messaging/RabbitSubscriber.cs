using FarmMonitor.Domain.Dtos;
using FarmMonitor.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FarmMonitor.Infrastructure.Messaging
{
    public class RabbitSubscriber<TEvent> : RabbitBase, IMessagingSubscriber<TEvent>
        where TEvent : class
    {
        private readonly string _brokerName;
        private readonly ILogger<RabbitSubscriber<TEvent>> _logger;

        public RabbitSubscriber(IMessagingConnection messagingConnection, string brokerName, ILogger<RabbitSubscriber<TEvent>> logger)
            : base(messagingConnection)
        {
            _brokerName = brokerName;
            _logger = logger;
        }

        public EventDto<TEvent>? RetrieveSingleMessage()
        {
            BasicGetResult data;

            using var channel = _connection.CreateModel();

            channel.ExchangeDeclare(exchange: Exchange, type: "direct");
            channel.QueueDeclare(queue: _brokerName, true, false, false, null);
            data = channel.BasicGet(_brokerName, true);

            if (data != null)
            {
                var body = Encoding.UTF8.GetString(data.Body.ToArray());
                return JsonSerializer.Deserialize<EventDto<TEvent>>(body);
            }

            return null;
        }

        public void Subscribe(Func<TEvent, Task> eventHandler)
        {
            var channel = _connection.CreateModel();

            var routingKey = $"{_brokerName}_key";

            channel.ExchangeDeclare(exchange: Exchange, type: "direct");
            channel.QueueDeclare(queue: _brokerName, true, false, false, null);
            channel.QueueBind(queue: _brokerName, Exchange, routingKey);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, eventArgs) =>
            {
                try
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    eventHandler(JsonSerializer.Deserialize<TEvent>(message)!).GetAwaiter().GetResult();
                    channel.BasicAck(eventArgs.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Unable to process event: {_brokerName}");
                    channel.BasicNack(eventArgs.DeliveryTag, false, false);
                }
            };

            _logger.LogInformation("Start comsume messages.");

            channel.BasicConsume(queue: _brokerName, autoAck: false, consumer);
        }
    }
}
