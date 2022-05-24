using FarmManager.Domain.Dtos;

namespace FarmManager.Domain.Interfaces
{
    public interface IMessagingPublisher<TEvent> where TEvent : class
    {
        void Publish(EventDto<TEvent> @event);
    }
}
