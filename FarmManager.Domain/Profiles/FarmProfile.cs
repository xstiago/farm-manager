using AutoMapper;
using FarmManager.Domain.Dtos;
using FarmManager.Domain.Entities;

namespace FarmManager.Domain.Profiles
{
    public class FarmProfile : Profile
    {
        public FarmProfile()
        {
            CreateMap<FarmDto, FarmEntity>();
            CreateMap<FarmEntity, FarmDto>();
        }
    }
}
