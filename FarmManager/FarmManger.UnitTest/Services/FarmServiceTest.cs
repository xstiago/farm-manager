using AutoMapper;
using FarmManager.Domain.Dtos;
using FarmManager.Domain.Entities;
using FarmManager.Domain.Exceptions;
using FarmManager.Domain.Interfaces;
using FarmManager.Domain.Profiles;
using FarmManager.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace FarmManger.UnitTest.Services
{
    public class FarmServiceTest
    {
        private readonly Mock<IRepository<DeviceEntity>> _deviceRepositoryMock;
        private readonly Mock<IRepository<FarmEntity>> _farmRepositoryMock;
        private readonly ILogger<FarmService> _logger;
        private readonly IMapper _mapper;

        private static ILogger<FarmService> SetupLogger()
        {
            var loggerMock = new Mock<ILogger<FarmService>>();
            return loggerMock.Object;
        }

        private static IMapper SetupMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FarmProfile>();
            });

            return config.CreateMapper();
        }

        private FarmService CreateService()
        {
            return new FarmService(_mapper, _deviceRepositoryMock.Object, _farmRepositoryMock.Object, _logger);
        }

        public FarmServiceTest()
        {
            _mapper = SetupMapper();
            _logger = SetupLogger();

            _farmRepositoryMock = new Mock<IRepository<FarmEntity>>();
            _deviceRepositoryMock = new Mock<IRepository<DeviceEntity>>();
        }

        [Fact]
        public async Task Should_Create_A_New_Farm_When_Receive_Valid_Payloads()
        {
            #region Arrange

            var service = CreateService();

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");

            #endregion Arrange

            #region Act

            await service.CreateAsync(farm);

            #endregion Act

            #region Assert

            _farmRepositoryMock.Verify(o => o.AddAsync(It.Is<FarmEntity>(f => f.Id == farm.Id)), Times.Once);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Delete_A_New_Farm_When_Receive_Valid_Payloads()
        {
            #region Arrange

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");
            var farmEntity = _mapper.Map<FarmEntity>(farm);

            _farmRepositoryMock.Setup(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()))
                .ReturnsAsync(farmEntity);

            var service = CreateService();

            var farmId = farm.Id;

            #endregion Arrange

            #region Act

            await service.DeleteAsync(farmId);

            #endregion Act

            #region Assert

            _farmRepositoryMock.Verify(o => o.RemoveAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);
            _farmRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);
            _deviceRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()), Times.Once);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Get_All_Farms_When_There_Are_In_Repository()
        {
            #region Arrange

            _farmRepositoryMock.Setup(o => o.GetAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()))
                .ReturnsAsync(new[]
                {
                    new FarmEntity { Id = Guid.NewGuid(), Name = "The Farm" },
                    new FarmEntity { Id = Guid.NewGuid(), Name = "West Farm" }
                });

            var service = CreateService();

            #endregion Arrange

            #region Act

            var farms = await service.GetAsync();

            #endregion Act

            #region Assert

            _farmRepositoryMock.Verify(o => o.GetAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);

            Assert.NotEmpty(farms);
            Assert.Equal(2, farms!.Count());

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Create_A_New_Farm_When_Occours_An_Unexpected_Error()
        {
            #region Arrange

            _farmRepositoryMock.Setup(o => o.AddAsync(It.IsAny<FarmEntity>()))
                .ThrowsAsync(new TimeoutException());

            var service = CreateService();

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<TimeoutException>(async () => await service.CreateAsync(farm));

            #endregion Act

            #region Assert

            _farmRepositoryMock.Verify(o => o.AddAsync(It.Is<FarmEntity>(f => f.Id == farm.Id)), Times.Once);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Delete_A_New_Farm_When_Exists_A_Device_Dependency_Register()
        {
            #region Arrange

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");
            var farmEntity = _mapper.Map<FarmEntity>(farm);

            _farmRepositoryMock.Setup(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()))
                .ReturnsAsync(farmEntity);

            _deviceRepositoryMock.Setup(o => o.GetSingleAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()))
                .ReturnsAsync(new DeviceEntity
                {
                    Id = Guid.NewGuid(),
                    Farm = farmEntity
                });

            var service = CreateService();

            var farmId = farm.Id;

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<EntityDependencyException>(async () => await service.DeleteAsync(farmId));

            #endregion Act

            #region Assert

            _farmRepositoryMock.Verify(o => o.RemoveAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Never);
            _farmRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);
            _deviceRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()), Times.Once);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Delete_A_New_Farm_When_Not_Exists_The_Farm()
        {
            #region Arrange

            var service = CreateService();

            var farmId = Guid.NewGuid();

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await service.DeleteAsync(farmId));

            #endregion Act

            #region Assert

            _farmRepositoryMock.Verify(o => o.RemoveAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Never);
            _farmRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);
            _deviceRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()), Times.Never);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Delete_A_New_Farm_When_Occours_An_Unexpected_Error()
        {
            #region Arrange

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");
            var farmEntity = _mapper.Map<FarmEntity>(farm);

            _farmRepositoryMock.Setup(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()))
                .ReturnsAsync(farmEntity);

            _farmRepositoryMock.Setup(o => o.RemoveAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()))
                .ThrowsAsync(new TimeoutException());

            var service = CreateService();

            var farmId = farm.Id;

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<TimeoutException>(async () => await service.DeleteAsync(farmId));

            #endregion Act

            #region Assert

            _farmRepositoryMock.Verify(o => o.RemoveAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);
            _farmRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);
            _deviceRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()), Times.Once);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Get_All_Farms_When_Occours_An_Unexpected_Error()
        {
            #region Arrange

            _farmRepositoryMock.Setup(o => o.GetAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()))
                .ThrowsAsync(new TimeoutException());

            var service = CreateService();

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<TimeoutException>(async () => await service.GetAsync());

            #endregion Act

            #region Assert

            _farmRepositoryMock.Verify(o => o.GetAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Update_A_New_Farm_When_Occours_An_Unexpected_Error()
        {
            #region Arrange

            _farmRepositoryMock.Setup(o => o.UpdateAsync(It.IsAny<FarmEntity>()))
                .ThrowsAsync(new TimeoutException());

            var service = CreateService();

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<TimeoutException>(async () => await service.UpdateAsync(farm));

            #endregion Act

            #region Assert

            _farmRepositoryMock.Verify(o => o.UpdateAsync(It.Is<FarmEntity>(f => f.Id == farm.Id)), Times.Once);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Update_A_New_Farm_When_Receive_Valid_Payloads()
        {
            #region Arrange

            var service = CreateService();

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");

            #endregion Arrange

            #region Act

            await service.UpdateAsync(farm);

            #endregion Act

            #region Assert

            _farmRepositoryMock.Verify(o => o.UpdateAsync(It.Is<FarmEntity>(f => f.Id == farm.Id)), Times.Once);
            
            #endregion Assert
        }
    }
}
