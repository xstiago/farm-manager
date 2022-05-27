using FarmManager.Domain.Entities;

namespace FarmManager.Infrastructure.Database.Repositories
{
    public class FarmRepository : BaseRepository<FarmEntity>
    {
        public FarmRepository(FarmManagerDbContext context) : base(context)
        {
        }
    }
}
