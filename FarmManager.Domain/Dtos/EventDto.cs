namespace FarmManager.Domain.Dtos
{
    public enum EventStatus
    {
        Create,
        Update,
        Delete
    }

    public class EventDto<TEvent>
    {
        public TEvent Event { get; set; }
        public EventStatus Status  { get; set; }
    }
}
