namespace FarmMonitor.Domain.Dtos
{
    public enum EventStatus
    {
        Create,
        Update,
        Delete
    }

    public class EventDto<TEvent>
    {
        public TEvent Event { get; set; } = default(TEvent)!;
        public EventStatus Status  { get; set; }
    }
}
