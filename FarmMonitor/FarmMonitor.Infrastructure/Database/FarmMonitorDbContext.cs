using Microsoft.EntityFrameworkCore;

namespace FarmMonitor.Infrastructure.Database
{
    public class FarmMonitorDbContext : DbContext
    {
        public const string DefaultSchema = "farm-monitor";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DefaultSchema);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public FarmMonitorDbContext(DbContextOptions<FarmMonitorDbContext> options) : base(options) { }
    }
}
