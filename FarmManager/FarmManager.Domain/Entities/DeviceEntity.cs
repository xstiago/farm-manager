using FarmManager.Domain.Interfaces;

namespace FarmManager.Domain.Entities
{
    public class DeviceEntity : IEntity
    {
        public Guid Id { get; set; }
        public Guid FarmId { get; set; }
        public FarmEntity Farm { get; set; } = default!;
    }
}
