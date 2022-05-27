namespace FarmMonitor.Domain.Dtos
{
    public class TelemetryDto
    {
        public Guid Id { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public DateTimeOffset MeasurementDate { get; set; }
        public Guid DeviceId { get; set; }
    }
}
