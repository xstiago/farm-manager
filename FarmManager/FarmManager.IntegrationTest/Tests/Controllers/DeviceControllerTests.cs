using AutoMapper;
using FarmManager.Domain.Dtos;
using FarmManager.Domain.Entities;
using FarmManager.Domain.Interfaces;
using FarmManager.IntegrationTest.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace FarmManager.IntegrationTest.Tests.Controllers
{
    [Collection(nameof(ApiFixtureCollection))]
    public class DeviceControllerTests
    {
        private readonly HttpClient _apiClient;
        private readonly DatabaseFixture _databaseFixture;
        private readonly IMapper _mapper;
        private readonly IMessagingSubscriber<DeviceDto> _messagingSubscriber;

        public DeviceControllerTests(
            WebApplicationFixture<Program> webApplicationFixture,
            DatabaseFixture databaseFixture)
        {
            _apiClient = webApplicationFixture.CreateClient();
            _databaseFixture = databaseFixture;

            _messagingSubscriber = webApplicationFixture.Services.GetRequiredService<IMessagingSubscriber<DeviceDto>>();
            _mapper = webApplicationFixture.Services.GetRequiredService<IMapper>();
        }

        [Fact]
        public async Task Should_Return_Http_Status_200_And_All_Registered_Devicess_When_Get_All()
        {
            #region Arrange

            var farm1 = new FarmDto(Guid.NewGuid(), "The Farm");
            var farm2 = new FarmDto(Guid.NewGuid(), "The Farm2");
            var device1 = new DeviceDto { Id = Guid.NewGuid(), FarmId = farm1.Id };
            var device2 = new DeviceDto { Id = Guid.NewGuid(), FarmId = farm2.Id };

            await _databaseFixture.AddAsync(_mapper.Map<FarmEntity>(farm1));
            await _databaseFixture.AddAsync(_mapper.Map<FarmEntity>(farm2));
            await _databaseFixture.AddAsync(_mapper.Map<DeviceEntity>(device1));
            await _databaseFixture.AddAsync(_mapper.Map<DeviceEntity>(device2));

            #endregion Arrange

            #region Act

            var response = await _apiClient.GetAsync("api/device/");

            #endregion Act

            #region Assert

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsAsync<DeviceDto[]>(new[] { new JsonMediaTypeFormatter() });

            content.Should().ContainEquivalentOf(device1);
            content.Should().ContainEquivalentOf(device2);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Return_Http_Status_200_When_To_Create_A_Device()
        {
            #region Arrange

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");
            var device = new DeviceDto { Id = Guid.NewGuid(), FarmId = farm.Id };

            await _databaseFixture.AddAsync(_mapper.Map<FarmEntity>(farm));

            #endregion Arrange

            #region Act

            var response = await _apiClient.PostAsync("api/device/", JsonContent.Create(device));

            #endregion Act

            #region Assert

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var entity = await _databaseFixture.SingleOrDefaultAsync<DeviceEntity>(o => o.Id == device.Id);

            Assert.NotNull(entity);
            Assert.Equal(device.Id, entity!.Id);

            var message = _messagingSubscriber.RetrieveSingleMessage();

            Assert.NotNull(message);
            Assert.Equal(device.Id, message!.Event.Id);
            Assert.Equal(EventStatus.Create, message!.Status);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Return_Http_Status_200_When_To_Delete_A_Device()
        {
            #region Arrange

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");
            var device = new DeviceDto { Id = Guid.NewGuid(), FarmId = farm.Id };

            await _databaseFixture.AddAsync(_mapper.Map<FarmEntity>(farm));
            await _databaseFixture.AddAsync(_mapper.Map<DeviceEntity>(device));

            #endregion Arrange

            #region Act

            var response = await _apiClient.DeleteAsync($"api/device/{device.Id}");

            #endregion Act

            #region Assert

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var entity = await _databaseFixture.SingleOrDefaultAsync<DeviceEntity>(o => o.Id == device.Id);

            Assert.Null(entity);

            var message = _messagingSubscriber.RetrieveSingleMessage();

            Assert.Equal(device.Id, message!.Event.Id);
            Assert.Equal(EventStatus.Delete, message!.Status);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Return_Http_Status_404_When_Try_Create_A_Device_And_No_Exists_The_Farm()
        {
            #region Arrange

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");
            var device = new DeviceDto { Id = Guid.NewGuid(), FarmId = farm.Id };

            #endregion Arrange

            #region Act

            var response = await _apiClient.PostAsync("api/device/", JsonContent.Create(device));

            #endregion Act

            #region Assert

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Return_Http_Status_404_When_Try_Delete_A_Device_That_No_Exists()
        {
            #region Arrange

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");
            var device = new DeviceDto { Id = Guid.NewGuid(), FarmId = farm.Id };

            #endregion Arrange

            #region Act

            var response = await _apiClient.DeleteAsync($"api/device/{device.Id}");

            #endregion Act

            #region Assert

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Return_Http_Status_404_When_Try_Update_A_Device_And_No_Exists_The_Farm()
        {
            #region Arrange

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");
            var device = new DeviceDto { Id = Guid.NewGuid(), FarmId = farm.Id };

            #endregion Arrange

            #region Act

            var response = await _apiClient.PutAsync("api/device/", JsonContent.Create(device));

            #endregion Act

            #region Assert

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            #endregion Assert
        }
    }
}
