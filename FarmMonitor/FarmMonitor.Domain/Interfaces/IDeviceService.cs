using FarmMonitor.Domain.Dtos;

namespace FarmMonitor.Domain.Interfaces
{
    public interface IDeviceService
    {
        Task ExecuteAsync(EventDto<DeviceDto> @event);
    }
}