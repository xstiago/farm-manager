using FarmMonitor.Domain.Dtos;

namespace FarmMonitor.Domain.Interfaces
{
    public interface IMessagingPublisher<TEvent> where TEvent : class
    {
        void Publish(EventDto<TEvent> @event);
    }
}
