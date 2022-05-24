using FarmManager.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FarmManager.IntegrationTest.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        private readonly string _connectionString;

        private FarmDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<FarmDbContext>().UseNpgsql(_connectionString).Options;
            var context = new FarmDbContext(options);

            context.Database.EnsureCreated();

            return context;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                using var context = CreateDbContext();
                context.Database.EnsureDeleted();
            }
        }

        public DatabaseFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.IntegrationTest.json")
                .Build();

            _connectionString = configuration["FarmDatabaseConnectionString"];
        }

        public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            using var context = CreateDbContext();

            await context.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<TEntity?> SingleOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            using var context = CreateDbContext();

            return await context.Set<TEntity>()
                .Where(filter)
                .SingleOrDefaultAsync();
        }
    }
}
