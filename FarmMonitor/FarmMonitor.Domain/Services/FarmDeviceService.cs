using AutoMapper;
using FarmMonitor.Domain.Dtos;
using FarmMonitor.Domain.Entities;
using FarmMonitor.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FarmMonitor.Domain.Services
{
    public class FarmDeviceService : IDeviceService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<DeviceEntity> _deviceRepository;
        private readonly ILogger _logger;

        public FarmDeviceService(
            IMapper mapper,
            IRepository<DeviceEntity> deviceRepository,
            ILogger<FarmDeviceService> logger)
        {
            _mapper = mapper;
            _deviceRepository = deviceRepository;
            _logger = logger;
        }

        public async Task ExecuteAsync(EventDto<DeviceDto> @event)
        {
            var entity = _mapper.Map<DeviceEntity>(@event.Event);

            switch (@event.Status)
            {
                case EventStatus.Create:
                    await _deviceRepository.AddAsync(entity);
                    break;
                case EventStatus.Update:
                    await _deviceRepository.UpdateAsync(entity);
                    break;
                case EventStatus.Delete:
                    await _deviceRepository.RemoveAsync(o => o.Id == entity.Id);
                    break;
                default:
                    break;
            }

            _logger.LogInformation($"Operation in DB done with success: {@event.Status} - Id: {@event.Event.Id}");
        }
    }
}
