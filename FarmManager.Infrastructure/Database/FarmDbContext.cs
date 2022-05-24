using Microsoft.EntityFrameworkCore;

namespace FarmManager.Infrastructure.Database
{
    public class FarmDbContext : DbContext
    {
        public const string DefaultSchema = "farm-manager";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DefaultSchema);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public FarmDbContext(DbContextOptions<FarmDbContext> options) : base(options) { }
    }
}
