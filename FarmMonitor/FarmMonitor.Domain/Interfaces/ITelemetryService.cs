using FarmMonitor.Domain.Dtos;

namespace FarmMonitor.Domain.Interfaces
{
    public interface ITelemetryService
    {
        Task CreateAsync(TelemetryDto dto);
    }
}