using FarmMonitor.Domain.Entities;

namespace FarmMonitor.Infrastructure.Database.Repositories
{
    public class TelemetryRepository : BaseRepository<TelemetryEntity>
    {
        public TelemetryRepository(FarmMonitorDbContext context) : base(context)
        {
        }
    }
}
