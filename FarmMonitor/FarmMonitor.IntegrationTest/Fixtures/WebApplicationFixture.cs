using FarmMonitor.Domain.Dtos;
using FarmMonitor.Domain.Interfaces;
using FarmMonitor.Infrastructure.Messaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FarmMonitor.IntegrationTest.Fixtures
{
    public class WebApplicationFixture<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                builder.ConfigureAppConfiguration(config =>
                {
                    var integrationConfig = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.IntegrationTest.json")
                        .Build();

                    config.AddConfiguration(integrationConfig);
                });

                services.AddSingleton<IMessagingPublisher<DeviceDto>>(s =>
                {
                    var configuration = s.GetService<IConfiguration>();

                    var rabbitConnection = new RabbitConnection(configuration!);
                    var brokerName = configuration!.GetSection("BrokerName").Value;

                    return new RabbitPublisher<DeviceDto>(rabbitConnection, brokerName);
                });
            });
        }
    }
}
