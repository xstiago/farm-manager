using System.Linq.Expressions;

namespace FarmMonitor.Domain.Interfaces
{
    public interface IRepository<TEntity> : IDisposable
       where TEntity : IEntity
    {
        Task AddAsync(TEntity obj);

        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter);

        Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> filter);

        Task RemoveAsync(Expression<Func<TEntity, bool>> filter);

        Task UpdateAsync(TEntity obj);
    }
}
