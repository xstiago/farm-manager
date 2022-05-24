using FarmManager.Domain.Dtos;

namespace FarmManager.Domain.Interfaces
{
    public interface IFarmService
    {
        Task CreateAsync(FarmDto farm);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<FarmDto>?> GetAsync();
        Task UpdateAsync(FarmDto farm);
    }
}