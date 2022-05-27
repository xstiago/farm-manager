using FarmMonitor.Domain.Dtos;

namespace FarmMonitor.Domain.Interfaces
{
    public interface IMessagingSubscriber<TEvent> 
        where TEvent : class
    {
        EventDto<TEvent>? RetrieveSingleMessage();
        void Subscribe(Func<TEvent, Task> eventHandler);
    }
}