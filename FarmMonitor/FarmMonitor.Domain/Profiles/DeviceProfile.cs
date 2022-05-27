using AutoMapper;
using FarmMonitor.Domain.Dtos;
using FarmMonitor.Domain.Entities;

namespace FarmMonitor.Domain.Profiles
{
    public class DeviceProfile : Profile
    {
        public DeviceProfile()
        {
            CreateMap<DeviceDto, DeviceEntity>();
            CreateMap<DeviceEntity, DeviceDto>();
        }
    }
}
