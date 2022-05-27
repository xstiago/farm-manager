using AutoMapper;
using FarmManager.Domain.Dtos;
using FarmManager.Domain.Entities;
using FarmManager.Domain.Exceptions;
using FarmManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FarmManager.Domain.Services
{
    public class FarmService : IService<FarmDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<DeviceEntity> _deviceRepository;
        private readonly IRepository<FarmEntity> _farmRepository;
        private readonly ILogger _logger;

        public FarmService(
            IMapper mapper,
            IRepository<DeviceEntity> deviceRepository,
            IRepository<FarmEntity> farmRepository,
            ILogger<FarmService> logger)
        {
            _mapper = mapper;
            _deviceRepository = deviceRepository;
            _farmRepository = farmRepository;
            _logger = logger;
        }

        public async Task CreateAsync(FarmDto farm)
        {
            try
            {
                var entity = _mapper.Map<FarmEntity>(farm);
                await _farmRepository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error to create farm {farm.Id} - {farm.Name}.");
                throw;
            }
        }

        public async Task UpdateAsync(FarmDto farm)
        {
            try
            {
                var entity = _mapper.Map<FarmEntity>(farm);
                await _farmRepository.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error to update farm {farm.Id} - {farm.Name}.");
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var farmFound = await _farmRepository.GetSingleAsync(farm => farm.Id == id);

                if (farmFound is null)
                    throw new EntityNotFoundException($"FarmId: { id }");

                var deviceFound = await _deviceRepository.GetSingleAsync(device => device.Farm.Id == id);

                if (deviceFound is not null)
                    throw new EntityDependencyException($"DeviceId: {deviceFound.Id}");

                await _farmRepository.RemoveAsync(entity => entity.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error to delete farm {id}.");
                throw;
            }
        }

        public async Task<IEnumerable<FarmDto>?> GetAsync()
        {
            try
            {
                var entities = await _farmRepository.GetAsync(entity => entity.Id != Guid.Empty);
                return _mapper.Map<IEnumerable<FarmDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error to delete get all farms.");
                throw;
            }
        }
    }
}
