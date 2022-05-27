using FarmMonitor.Domain.Dtos;
using FarmMonitor.Domain.Entities;
using FarmMonitor.Domain.Interfaces;
using FarmMonitor.Domain.Profiles;
using FarmMonitor.Domain.Services;
using FarmMonitor.Infrastructure.Database;
using FarmMonitor.Infrastructure.Database.Repositories;
using FarmMonitor.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FarmMonitor.Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        private static IServiceCollection AddDbContext(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetService<IConfiguration>();

            var connectionString = configuration!.GetSection("FarmDatabaseConnectionString").Value;

            services.AddDbContextPool<FarmMonitorDbContext>(options =>
                options.UseNpgsql(connectionString,
                    sqlServerOptions => sqlServerOptions
                        .MigrationsAssembly("FarmMonitor")
                        .EnableRetryOnFailure(
                            maxRetryCount: 6,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorCodesToAdd: null)
                        .MigrationsHistoryTable("__EFMigrationsHistory", FarmMonitorDbContext.DefaultSchema)));

            return services;
        }

        private static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            services.AddSingleton<IMessagingSubscriber<EventDto<DeviceDto>>>(s =>
            {
                var configuration = s.GetService<IConfiguration>();

                var rabbitConnection = new RabbitConnection(configuration!);
                var brokerName = configuration!.GetSection("BrokerName").Value;
                var logger = s.GetService<ILogger<RabbitSubscriber<EventDto<DeviceDto>>>>();

                return new RabbitSubscriber<EventDto<DeviceDto>>(rabbitConnection, brokerName, logger!);
            });

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepository<TelemetryEntity>, TelemetryRepository>();
            services.AddScoped<IRepository<DeviceEntity>, DeviceRepository>();
            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(TelemetryProfile));
            services.AddScoped<ITelemetryService, TelemetryService>();
            services.AddScoped<IDeviceService, FarmDeviceService>();
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
