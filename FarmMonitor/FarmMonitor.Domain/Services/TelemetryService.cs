using AutoMapper;
using FarmMonitor.Domain.Dtos;
using FarmMonitor.Domain.Entities;
using FarmMonitor.Domain.Exceptions;
using FarmMonitor.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FarmMonitor.Domain.Services
{
    public class TelemetryService : ITelemetryService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<TelemetryEntity> _telemetryRepository;
        private readonly IRepository<DeviceEntity> _deviceRepository;
        private readonly ILogger<TelemetryService> _logger;

        public TelemetryService(
            IMapper mapper,
            IRepository<TelemetryEntity> telemetryRepository,
            IRepository<DeviceEntity> deviceRepository,
            ILogger<TelemetryService> logger)
        {
            _mapper = mapper;
            _telemetryRepository = telemetryRepository;
            _deviceRepository = deviceRepository;
            _logger = logger;
        }

        public async Task CreateAsync(TelemetryDto dto)
        {
            try
            {
                var device = await _deviceRepository.GetSingleAsync(o => o.Id == dto.DeviceId);

                if (device is null)
                    throw new EntityNotFoundException($"DeviceId: {dto.DeviceId}");

                var entity = _mapper.Map<TelemetryEntity>(dto);

                await _telemetryRepository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error to create telemetry {JsonSerializer.Serialize(dto)}.");
                throw;
            }
        }
    }
}
