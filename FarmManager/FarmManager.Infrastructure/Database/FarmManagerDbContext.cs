using Microsoft.EntityFrameworkCore;

namespace FarmManager.Infrastructure.Database
{
    public class FarmManagerDbContext : DbContext
    {
        public const string DefaultSchema = "farm-manager";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DefaultSchema);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public FarmManagerDbContext(DbContextOptions<FarmManagerDbContext> options) : base(options) { }
    }
}
