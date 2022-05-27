using AutoMapper;
using FarmManager.Domain.Dtos;
using FarmManager.Domain.Entities;

namespace FarmManager.Domain.Profiles
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
