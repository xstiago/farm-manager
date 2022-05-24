using FarmManager.Domain.Entities;

namespace FarmManager.Infrastructure.Database.Repositories
{
    public class FarmRepository : BaseRepository<FarmEntity>
    {
        public FarmRepository(FarmDbContext context) : base(context)
        {
        }
    }
}
