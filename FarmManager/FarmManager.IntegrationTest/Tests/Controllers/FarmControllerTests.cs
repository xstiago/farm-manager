using AutoMapper;
using FarmManager.Domain.Dtos;
using FarmManager.Domain.Entities;
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
    public class FarmControllerTests
    {
        private readonly HttpClient _apiClient;
        private readonly DatabaseFixture _databaseFixture;
        private readonly IMapper _mapper;

        public FarmControllerTests(
            WebApplicationFixture<Program> webApplicationFixture,
            DatabaseFixture databaseFixture)
        {
            _apiClient = webApplicationFixture.CreateClient();
            _databaseFixture = databaseFixture;
            _mapper = webApplicationFixture.Services.GetRequiredService<IMapper>();
        }

        [Fact]
        public async Task Should_Return_Http_Status_200_And_All_Registered_Farms_When_Get_All()
        {
            #region Arrange

            var farm1 = new FarmDto(Guid.NewGuid(), "The Farm");
            var farm2 = new FarmDto(Guid.NewGuid(), "The Farm 2");

            await _databaseFixture.AddAsync(_mapper.Map<FarmEntity>(farm1));
            await _databaseFixture.AddAsync(_mapper.Map<FarmEntity>(farm2));

            #endregion Arrange

            #region Act

            var response = await _apiClient.GetAsync("api/farm/");

            #endregion Act

            #region Assert

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsAsync<FarmDto[]>(new[] { new JsonMediaTypeFormatter() });

            content.Should().ContainEquivalentOf(farm1);
            content.Should().ContainEquivalentOf(farm2);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Return_Http_Status_200_When_To_Create_A_Farm()
        {
            #region Arrange

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");

            #endregion Arrange

            #region Act

            var response = await _apiClient.PostAsync("api/farm/", JsonContent.Create(farm));

            #endregion Act

            #region Assert

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var entity = await _databaseFixture.SingleOrDefaultAsync<FarmEntity>(o => o.Id == farm.Id);

            Assert.NotNull(entity);

            Assert.Equal(farm.Id, entity!.Id);
            Assert.Equal(farm.Name, entity!.Name);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Return_Http_Status_200_When_To_Delete_A_Farm()
        {
            #region Arrange

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");

            await _databaseFixture.AddAsync(_mapper.Map<FarmEntity>(farm));

            #endregion Arrange

            #region Act

            var response = await _apiClient.DeleteAsync($"api/farm/{farm.Id}");

            #endregion Act

            #region Assert

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var entity = await _databaseFixture.SingleOrDefaultAsync<FarmEntity>(o => o.Id == farm.Id);

            Assert.Null(entity);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Return_Http_Status_200_When_To_Update_A_Farm()
        {
            #region Arrange

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");

            await _databaseFixture.AddAsync(_mapper.Map<FarmEntity>(farm));

            farm.Name = "The Updated Farm";

            #endregion Arrange

            #region Act

            var response = await _apiClient.PutAsync("api/farm/", JsonContent.Create(farm));

            #endregion Act

            #region Assert

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var entity = await _databaseFixture.SingleOrDefaultAsync<FarmEntity>(o => o.Id == farm.Id);

            Assert.NotNull(entity);
            Assert.Equal(farm.Id, entity!.Id);
            Assert.Equal(farm.Name, entity!.Name);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Return_Http_Status_404_When_Try_Delete_A_Farm_That_No_Exists()
        {
            #region Arrange

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");

            #endregion Arrange

            #region Act

            var response = await _apiClient.DeleteAsync($"api/farm/{farm.Id}");

            #endregion Act

            #region Assert

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Return_Http_Status_409_When_Try_Delete_A_Farm_And_Exists_A_Ralated_Device()
        {
            #region Arrange

            var farm = new FarmEntity { Id = Guid.NewGuid(), Name = "The Farm" };
            var device = new DeviceEntity { Id = Guid.NewGuid(), Farm = farm };
            await _databaseFixture.AddAsync(_mapper.Map<DeviceEntity>(device));

            #endregion Arrange

            #region Act

            var response = await _apiClient.DeleteAsync($"api/farm/{farm.Id}");

            #endregion Act

            #region Assert

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

            #endregion Assert
        }
    }
}
