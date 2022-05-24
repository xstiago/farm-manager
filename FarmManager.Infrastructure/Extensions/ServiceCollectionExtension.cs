using FarmManager.Domain.Dtos;
using FarmManager.Domain.Entities;
using FarmManager.Domain.Interfaces;
using FarmManager.Domain.Profiles;
using FarmManager.Domain.Services;
using FarmManager.Infrastructure.Database;
using FarmManager.Infrastructure.Database.Repositories;
using FarmManager.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FarmManager.Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        private static IServiceCollection AddDbContext(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetService<IConfiguration>();

            var connectionString = configuration!.GetSection("FarmDatabaseConnectionString").Value;

            services.AddDbContextPool<FarmDbContext>(options =>
                options.UseNpgsql(connectionString,
                    sqlServerOptions => sqlServerOptions
                        .MigrationsAssembly("FarmManager")
                        .EnableRetryOnFailure(
                            maxRetryCount: 6,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorCodesToAdd: null)
                        .MigrationsHistoryTable("__EFMigrationsHistory", FarmDbContext.DefaultSchema)));

            return services;
        }

        private static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            services.AddSingleton<IMessagingPublisher<FarmDto>>(s =>
            {
                var configuration = s.GetService<IConfiguration>();

                var rabbitConnection = new RabbitConnection(configuration!);
                var brokerName = configuration!.GetSection("BrokerName").Value;

                return new RabbitPublisher<FarmDto>(rabbitConnection, brokerName);
            });

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepository<FarmEntity>, FarmRepository>();
            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(FarmProfile));
            services.AddScoped<IFarmService, FarmService>();
            return services;
        }

        public static IServiceCollection AddInfra(this IServiceCollection services)
        {
            services.AddDbContext()
                .AddMessaging()
                .AddRepositories()
                .AddServices();

            return services;
        }
    }
}
