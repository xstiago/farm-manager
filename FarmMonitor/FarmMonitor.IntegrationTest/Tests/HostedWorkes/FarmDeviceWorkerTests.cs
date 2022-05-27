using FarmMonitor.Domain.Dtos;
using FarmMonitor.Domain.Entities;
using FarmMonitor.Domain.Interfaces;
using FarmMonitor.IntegrationTest.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FarmMonitor.IntegrationTest.Tests.HostedWorkes
{
    [Collection(nameof(ApiFixtureCollection))]
    public class FarmDeviceWorkerTests
    {
        private readonly TimeSpan _messageWaiter = TimeSpan.FromSeconds(3);
        private readonly IMessagingPublisher<DeviceDto> _publisher;
        private readonly IMessagingSubscriber<EventDto<DeviceDto>> _subscriber;
        private readonly DatabaseFixture _databaseFixture;

        public FarmDeviceWorkerTests(
            WebApplicationFixture<Program> webApplicationFixture,
            DatabaseFixture databaseFixture)
        {
            _publisher = webApplicationFixture.Services.GetRequiredService<IMessagingPublisher<DeviceDto>>();
            _subscriber = webApplicationFixture.Services.GetRequiredService<IMessagingSubscriber<EventDto<DeviceDto>>>();
            _databaseFixture = databaseFixture;
        }

        [Fact]
        public async Task Should_Save_Telemetry_On_Repository_When_Receive_Create_Status()
        {
            #region Arrange

            var payload = new EventDto<DeviceDto>
            {
                Event = new DeviceDto
                {
                    Id = Guid.NewGuid(),
                    FarmId = Guid.NewGuid()
                },
                Status = EventStatus.Create
            };

            _publisher.Publish(payload);

            #endregion Arrange

            #region Act

            await Task.Delay(_messageWaiter, CancellationToken.None);

            #endregion Act

            #region Assert

            var deviceAfter = await _databaseFixture.SingleOrDefaultAsync<DeviceEntity>(x => x.Id == payload.Event.Id);
            Assert.NotNull(deviceAfter);

            Assert.Null(_subscriber.RetrieveSingleMessage());

            #endregion Assert
        }
    }
}
