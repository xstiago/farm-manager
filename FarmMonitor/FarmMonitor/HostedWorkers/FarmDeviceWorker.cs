using FarmMonitor.Domain.Dtos;
using FarmMonitor.Domain.Interfaces;

namespace FarmMonitor.HostedWorkers
{
    public class FarmDeviceWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public FarmDeviceWorker(
            IServiceProvider serviceProvider,
            IMessagingSubscriber<EventDto<DeviceDto>> messagingSubscriber)
        {
            _serviceProvider = serviceProvider;
            messagingSubscriber.Subscribe(EventHandlerAsync);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;

        private async Task EventHandlerAsync(EventDto<DeviceDto> payload)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetService<IDeviceService>();
            await service!.ExecuteAsync(payload);
        }
    }
}
