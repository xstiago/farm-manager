using FarmMonitor.Domain.Interfaces;

namespace FarmMonitor.Domain.Entities
{
    public class TelemetryEntity : IEntity
    {
        public Guid Id { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public DateTimeOffset MeasurementDate { get; set; }
        public Guid DeviceId { get; set; }
        public DeviceEntity Device { get; set; } = default!;
    }
}
