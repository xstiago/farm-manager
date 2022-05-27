namespace FarmManager.Domain.Interfaces
{
    public interface IService<TDto>
    {
        Task CreateAsync(TDto dto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<TDto>?> GetAsync();
        Task UpdateAsync(TDto dto);
    }
}