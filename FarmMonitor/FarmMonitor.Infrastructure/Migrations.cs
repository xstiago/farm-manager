using FarmMonitor.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FarmMonitor.Infrastructure
{
    public class Migrations
    {
        protected Migrations()
        { }

        public static async Task ApplyAsync(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<FarmMonitorDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Migrations>>();

            if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
            {
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully.");
            }
            else
                logger.LogInformation("There are no pending migrations.");
        }
    }
}
