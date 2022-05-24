using AutoMapper;
using FarmManager.Domain.Dtos;
using FarmManager.Domain.Entities;
using FarmManager.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FarmManager.Domain.Services
{
    public class FarmService : IFarmService
    {
        private readonly IMessagingPublisher<FarmDto> _publisher;
        private readonly IMapper _mapper;
        private readonly IRepository<FarmEntity> _repository;
        private readonly ILogger _logger;

        public FarmService(
            IServiceProvider provider,
            IMapper mapper,
            IRepository<FarmEntity> repository,
            ILogger<FarmService> logger)
        {
            _publisher = provider.GetRequiredService<IMessagingPublisher<FarmDto>>();
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        public async Task CreateAsync(FarmDto farm)
        {
            try
            {
                var entity = _mapper.Map<FarmEntity>(farm);
                await _repository.AddAsync(entity);
                _publisher.Publish(new EventDto<FarmDto>
                {
                    Event = farm,
                    Status = EventStatus.Create
                });
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
                await _repository.UpdateAsync(entity);
                _publisher.Publish(new EventDto<FarmDto>
                {
                    Event = farm,
                    Status = EventStatus.Update
                });
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
                await _repository.RemoveAsync(entity => entity.Id == id);
                _publisher.Publish(new EventDto<FarmDto>
                {
                    Event = new FarmDto(id, string.Empty),
                    Status = EventStatus.Delete
                });
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
                var entities = await _repository.GetAsync(entity => entity.Id != Guid.Empty);
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
