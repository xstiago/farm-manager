using FarmMonitor.Domain.Dtos;
using FarmMonitor.Domain.Entities;
using FarmMonitor.IntegrationTest.Fixtures;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace FarmMonitor.IntegrationTest.Tests.Controllers
{
    [Collection(nameof(ApiFixtureCollection))]
    public class TelemetryControllerTests
    {
        private readonly HttpClient _apiClient;
        private readonly DatabaseFixture _databaseFixture;

        public TelemetryControllerTests(
            WebApplicationFixture<Program> webApplicationFixture,
            DatabaseFixture databaseFixture)
        {
            _apiClient = webApplicationFixture.CreateClient();
            _databaseFixture = databaseFixture;
        }

        [Fact]
        public async Task Should_Return_Http_Status_200_When_To_Create_A_Telemetry()
        {
            #region Arrange

            var device = new DeviceEntity
            {
                FarmId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
            };

            var telemetry = new TelemetryDto
            {
                DeviceId = device.Id,
                Humidity = 4.0f,
                MeasurementDate = DateTimeOffset.UtcNow,
                Temperature = 20.0f
            };

            await _databaseFixture.AddAsync(device);

            #endregion Arrange

            #region Act

            var response = await _apiClient.PostAsync("api/telemetry/", JsonContent.Create(telemetry));

            #endregion Act

            #region Assert

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var entity = await _databaseFixture.SingleOrDefaultAsync<TelemetryEntity>(o => o.DeviceId == device.Id);
            Assert.NotNull(entity);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Return_Http_Status_404_When_No_Exists_A_Related_Device_To_Create_A_Telemetry()
        {
            #region Arrange

            var telemetry = new TelemetryDto
            {
                DeviceId = Guid.NewGuid(),
                Humidity = 4.0f,
                MeasurementDate = DateTimeOffset.UtcNow,
                Temperature = 20.0f
            };

            #endregion Arrange

            #region Act

            var response = await _apiClient.PostAsync("api/telemetry/", JsonContent.Create(telemetry));

            #endregion Act

            #region Assert

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            #endregion Assert
        }
    }
}
