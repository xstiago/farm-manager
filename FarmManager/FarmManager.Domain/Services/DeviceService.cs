using AutoMapper;
using FarmManager.Domain.Dtos;
using FarmManager.Domain.Entities;
using FarmManager.Domain.Exceptions;
using FarmManager.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FarmManager.Domain.Services
{
    public class DeviceService : IService<DeviceDto>
    {
        private readonly IRepository<DeviceEntity> _deviceRepository;
        private readonly IRepository<FarmEntity> _farmRepository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IMessagingPublisher<DeviceDto> _publisher;

        private async Task<FarmEntity> EnsureFarmExists(DeviceDto device)
        {
            var farmFound = await _farmRepository.GetSingleAsync(farm => farm.Id == device.FarmId);

            if (farmFound is null)
                throw new EntityNotFoundException($"FarmId: { device.FarmId }");

            return farmFound;
        }

        public DeviceService(
            IServiceProvider provider,
            IMapper mapper,
            IRepository<DeviceEntity> deviceRepository,
            IRepository<FarmEntity> farmRepository,
            ILogger<DeviceService> logger)
        {
            _publisher = provider.GetRequiredService<IMessagingPublisher<DeviceDto>>();
            _mapper = mapper;
            _deviceRepository = deviceRepository;
            _farmRepository = farmRepository;
            _logger = logger;
        }

        public async Task CreateAsync(DeviceDto device)
        {
            try
            {
                var farm = await EnsureFarmExists(device);

                var entity = _mapper.Map<DeviceEntity>(device);
                entity.FarmId = farm.Id;

                await _deviceRepository.AddAsync(entity);
                _publisher.Publish(new EventDto<DeviceDto>
                {
                    Event = device,
                    Status = EventStatus.Create
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error to create device {device.Id}.");
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var deviceFound = await _deviceRepository.GetSingleAsync(device => device.Id == id);

                if (deviceFound is null)
                    throw new EntityNotFoundException($"DeviceId: { id }");

                await _deviceRepository.RemoveAsync(entity => entity.Id == id);
                _publisher.Publish(new EventDto<DeviceDto>
                {
                    Event = new DeviceDto { Id = id },
                    Status = EventStatus.Delete
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error to delete farm {id}.");
                throw;
            }
        }

        public async Task<IEnumerable<DeviceDto>?> GetAsync()
        {
            try
            {
                var entities = await _deviceRepository.GetAsync(entity => entity.Id != Guid.Empty);
                return _mapper.Map<IEnumerable<DeviceDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error to delete get all farms.");
                throw;
            }
        }

        public async Task UpdateAsync(DeviceDto device)
        {
            try
            {
                var farm = await EnsureFarmExists(device);

                var entity = _mapper.Map<DeviceEntity>(device);
                entity.FarmId = farm.Id;

                await _deviceRepository.UpdateAsync(entity);
                _publisher.Publish(new EventDto<DeviceDto>
                {
                    Event = device,
                    Status = EventStatus.Update
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error to update farm {device.Id}.");
                throw;
            }
        }
    }
}
