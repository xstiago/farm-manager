using AutoMapper;
using FarmMonitor.Domain.Dtos;
using FarmMonitor.Domain.Entities;

namespace FarmMonitor.Domain.Profiles
{
    public class TelemetryProfile : Profile
    {
        public TelemetryProfile()
        {
            CreateMap<TelemetryDto, TelemetryEntity>();
            CreateMap<TelemetryEntity, TelemetryDto>();
        }
    }
}
