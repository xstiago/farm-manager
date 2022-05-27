using FarmManager.Domain.Entities;

namespace FarmManager.Infrastructure.Database.Repositories
{
    public class DeviceRepository : BaseRepository<DeviceEntity>
    {
        public DeviceRepository(FarmManagerDbContext context) : base(context)
        {
        }
    }
}
