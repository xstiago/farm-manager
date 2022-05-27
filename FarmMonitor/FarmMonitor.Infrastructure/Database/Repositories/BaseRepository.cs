using FarmMonitor.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FarmMonitor.Infrastructure.Database.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        protected readonly FarmMonitorDbContext Context;

        protected BaseRepository(FarmMonitorDbContext context)
        {
            Context = context;
            Set = Context.Set<TEntity>();
        }

        protected DbSet<TEntity> Set { get; init; }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Context.Dispose();
        }

        public async Task AddAsync(TEntity obj)
        {
            Set.Add(obj);
            await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter) =>
            await Set.Where(filter).AsNoTracking().ToListAsync();

        public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> filter) =>
            (await GetAsync(filter)).SingleOrDefault();

        public async Task RemoveAsync(Expression<Func<TEntity, bool>> filter)
        {
            var entity = Set.SingleOrDefault(filter);

            if (entity is not null)
                Set.Remove(entity);

            await Context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity obj)
        {
            Context.Update(obj);
            await Context.SaveChangesAsync();
        }
    }
}
