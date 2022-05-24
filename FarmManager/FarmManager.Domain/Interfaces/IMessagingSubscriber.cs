using FarmManager.Domain.Dtos;

namespace FarmManager.Domain.Interfaces
{
    public interface IMessagingSubscriber<TEvent> 
        where TEvent : class
    {
        EventDto<TEvent>? RetrieveSingleMessage();
    }
}