using FarmMonitor.Domain.Interfaces;

namespace FarmMonitor.Domain.Entities
{
    public class DeviceEntity : IEntity
    {
        public Guid Id { get; set; }
        public Guid FarmId { get; set; }
    }
}
