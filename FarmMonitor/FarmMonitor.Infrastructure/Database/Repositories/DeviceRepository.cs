using FarmMonitor.Domain.Entities;

namespace FarmMonitor.Infrastructure.Database.Repositories
{
    public class DeviceRepository : BaseRepository<DeviceEntity>
    {
        public DeviceRepository(FarmMonitorDbContext context) : base(context)
        {
        }
    }
}
