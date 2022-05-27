using AutoMapper;
using FarmManager.Domain.Dtos;
using FarmManager.Domain.Entities;
using FarmManager.Domain.Exceptions;
using FarmManager.Domain.Interfaces;
using FarmManager.Domain.Profiles;
using FarmManager.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace FarmManger.UnitTest.Services
{
    public class DeviceServiceTest
    {
        private readonly DeviceDto _device;
        private readonly DeviceDto _device2;
        private readonly Mock<IRepository<DeviceEntity>> _deviceRepositoryMock;
        private readonly FarmDto _farm;
        private readonly Mock<IRepository<FarmEntity>> _farmRepositoryMock;
        private readonly ILogger<DeviceService> _logger;
        private readonly IMapper _mapper;
        private readonly Mock<IMessagingPublisher<DeviceDto>> _messagingMock;

        private static Mock<IServiceProvider> BuildServiceProviderMock(IMessagingPublisher<DeviceDto> messaging)
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IMessagingPublisher<DeviceDto>)))
                .Returns(messaging);

            var serviceScopeMock = new Mock<IServiceScope>();
            serviceScopeMock.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);

            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            serviceScopeFactoryMock.Setup(x => x.CreateScope())
                .Returns(serviceScopeMock.Object);

            serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactoryMock.Object);

            return serviceProviderMock;
        }

        private static ILogger<DeviceService> SetupLogger()
        {
            var loggerMock = new Mock<ILogger<DeviceService>>();
            return loggerMock.Object;
        }

        private static IMapper SetupMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FarmProfile>();
                cfg.AddProfile<DeviceProfile>();
            });

            return config.CreateMapper();
        }

        private DeviceService CreateService()
        {
            var serviceProviderMock = BuildServiceProviderMock(_messagingMock.Object);

            var service = new DeviceService(
                serviceProviderMock.Object,
                _mapper,
                _deviceRepositoryMock.Object,
                _farmRepositoryMock.Object,
                _logger);

            return service;
        }

        public DeviceServiceTest()
        {
            _mapper = SetupMapper();
            _logger = SetupLogger();

            _messagingMock = new Mock<IMessagingPublisher<DeviceDto>>();
            _farmRepositoryMock = new Mock<IRepository<FarmEntity>>();
            _deviceRepositoryMock = new Mock<IRepository<DeviceEntity>>();

            _farm = new FarmDto(Guid.NewGuid(), "The Farm");

            _device = new DeviceDto
            {
                Id = Guid.NewGuid(),
                FarmId = _farm.Id
            };

            _device2 = new DeviceDto
            {
                Id = Guid.NewGuid(),
                FarmId = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Should_Create_A_New_Device_When_Receive_Valid_Payloads()
        {
            #region Arrange

            _farmRepositoryMock.Setup(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()))
                .ReturnsAsync(_mapper.Map<FarmEntity>(_farm));

            EventDto<DeviceDto>? sentEvent = null;

            _messagingMock.Setup(o => o.Publish(It.IsAny<EventDto<DeviceDto>>()))
                .Callback((EventDto<DeviceDto> eventDto) => sentEvent = eventDto);

            var service = CreateService();

            #endregion Arrange

            #region Act

            await service.CreateAsync(_device);

            #endregion Act

            #region Assert

            _deviceRepositoryMock.Verify(o => o.AddAsync(It.Is<DeviceEntity>(f => f.Id == _device.Id)), Times.Once);
            _farmRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);
            _messagingMock.Verify(o => o.Publish(It.Is<EventDto<DeviceDto>>(e => e.Event.Id == _device.Id)), Times.Once);

            Assert.NotNull(sentEvent);
            Assert.Equal(_device.Id, sentEvent!.Event.Id);
            Assert.Equal(EventStatus.Create, sentEvent!.Status);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Delete_A_New_Device_When_Receive_Valid_Payloads()
        {
            #region Arrange

            EventDto<DeviceDto>? sentEvent = null;

            _messagingMock.Setup(o => o.Publish(It.IsAny<EventDto<DeviceDto>>()))
                .Callback((EventDto<DeviceDto> eventDto) => sentEvent = eventDto);

            var deviceEntity = _mapper.Map<DeviceEntity>(_device);

            _deviceRepositoryMock.Setup(o => o.GetSingleAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()))
                .ReturnsAsync(deviceEntity);

            var service = CreateService();

            var deviceId = _device.Id;

            #endregion Arrange

            #region Act

            await service.DeleteAsync(deviceId);

            #endregion Act

            #region Assert

            _deviceRepositoryMock.Verify(o => o.RemoveAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()), Times.Once);
            _deviceRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()), Times.Once);
            _messagingMock.Verify(o => o.Publish(It.Is<EventDto<DeviceDto>>(e => e.Event.Id == deviceId)), Times.Once);

            Assert.NotNull(sentEvent);
            Assert.Equal(deviceId, sentEvent!.Event.Id);
            Assert.Equal(EventStatus.Delete, sentEvent!.Status);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Get_All_Devices_When_There_Are_In_Repository()
        {
            #region Arrange

            _deviceRepositoryMock.Setup(o => o.GetAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()))
                .ReturnsAsync(new[]
                {
                    _mapper.Map<DeviceEntity>(_device),
                    _mapper.Map<DeviceEntity>(_device2)
                });

            var service = CreateService();

            #endregion Arrange

            #region Act

            var farms = await service.GetAsync();

            #endregion Act

            #region Assert

            _deviceRepositoryMock.Verify(o => o.GetAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()), Times.Once);

            Assert.NotEmpty(farms);
            Assert.Equal(2, farms!.Count());

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Create_A_New_Device_When_No_Exists_A_Related_Farm()
        {
            #region Arrange

            var service = CreateService();

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await service.CreateAsync(_device));

            #endregion Act

            #region Assert

            _deviceRepositoryMock.Verify(o => o.AddAsync(It.Is<DeviceEntity>(f => f.Id == _device.Id)), Times.Never);
            _farmRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);
            _messagingMock.Verify(o => o.Publish(It.Is<EventDto<DeviceDto>>(e => e.Event.Id == _device.Id)), Times.Never);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Create_A_New_Device_When_Occours_An_Unexpected_Error()
        {
            #region Arrange

            _farmRepositoryMock.Setup(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()))
                .ReturnsAsync(_mapper.Map<FarmEntity>(_farm));

            _deviceRepositoryMock.Setup(o => o.AddAsync(It.IsAny<DeviceEntity>()))
                .ThrowsAsync(new TimeoutException());

            var service = CreateService();

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<TimeoutException>(async () => await service.CreateAsync(_device));

            #endregion Act

            #region Assert

            _deviceRepositoryMock.Verify(o => o.AddAsync(It.Is<DeviceEntity>(f => f.Id == _device.Id)), Times.Once);
            _farmRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);
            _messagingMock.Verify(o => o.Publish(It.Is<EventDto<DeviceDto>>(e => e.Event.Id == _device.Id)), Times.Never);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Delete_A_New_Device_When_No_Exists_The_Device()
        {
            #region Arrange

            var service = CreateService();

            var deviceId = _device.Id;

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await service.DeleteAsync(deviceId));

            #endregion Act

            #region Assert

            _deviceRepositoryMock.Verify(o => o.AddAsync(It.Is<DeviceEntity>(f => f.Id == deviceId)), Times.Never);
            _deviceRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()), Times.Once);
            _messagingMock.Verify(o => o.Publish(It.Is<EventDto<DeviceDto>>(e => e.Event.Id == deviceId)), Times.Never);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Delete_A_New_Device_When_Occours_An_Unexpected_Error()
        {
            #region Arrange

            _deviceRepositoryMock.Setup(o => o.GetSingleAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()))
                .ReturnsAsync(_mapper.Map<DeviceEntity>(_device));

            _deviceRepositoryMock.Setup(o => o.RemoveAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()))
                .ThrowsAsync(new TimeoutException());

            var service = CreateService();

            var deviceId = _device.Id;

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<TimeoutException>(async () => await service.DeleteAsync(deviceId));

            #endregion Act

            #region Assert

            _deviceRepositoryMock.Verify(o => o.RemoveAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()), Times.Once);
            _deviceRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()), Times.Once);
            _messagingMock.Verify(o => o.Publish(It.Is<EventDto<DeviceDto>>(e => e.Event.Id == deviceId)), Times.Never);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Get_All_Devices_When_Occours_An_Unexpected_Error()
        {
            #region Arrange

            _deviceRepositoryMock.Setup(o => o.GetAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()))
                .ThrowsAsync(new TimeoutException());

            var service = CreateService();

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<TimeoutException>(async () => await service.GetAsync());

            #endregion Act

            #region Assert

            _deviceRepositoryMock.Verify(o => o.GetAsync(It.IsAny<Expression<Func<DeviceEntity, bool>>>()), Times.Once);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Update_A_New_Device_When_No_Exists_A_Related_Farm()
        {
            #region Arrange

            var service = CreateService();

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await service.UpdateAsync(_device));

            #endregion Act

            #region Assert

            _deviceRepositoryMock.Verify(o => o.UpdateAsync(It.Is<DeviceEntity>(f => f.Id == _device.Id)), Times.Never);
            _farmRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);
            _messagingMock.Verify(o => o.Publish(It.Is<EventDto<DeviceDto>>(e => e.Event.Id == _device.Id)), Times.Never);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Update_A_New_Device_When_Occours_An_Unexpected_Error()
        {
            #region Arrange

            _farmRepositoryMock.Setup(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()))
                .ReturnsAsync(_mapper.Map<FarmEntity>(_farm));

            _deviceRepositoryMock.Setup(o => o.UpdateAsync(It.IsAny<DeviceEntity>()))
                .ThrowsAsync(new TimeoutException());

            var service = CreateService();

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<TimeoutException>(async () => await service.UpdateAsync(_device));

            #endregion Act

            #region Assert

            _deviceRepositoryMock.Verify(o => o.UpdateAsync(It.Is<DeviceEntity>(f => f.Id == _device.Id)), Times.Once);
            _farmRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);
            _messagingMock.Verify(o => o.Publish(It.Is<EventDto<DeviceDto>>(e => e.Event.Id == _device.Id)), Times.Never);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Update_A_New_Device_When_Receive_Valid_Payloads()
        {
            #region Arrange

            _farmRepositoryMock.Setup(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()))
                .ReturnsAsync(_mapper.Map<FarmEntity>(_farm));

            EventDto<DeviceDto>? sentEvent = null;

            _messagingMock.Setup(o => o.Publish(It.IsAny<EventDto<DeviceDto>>()))
                .Callback((EventDto<DeviceDto> eventDto) => sentEvent = eventDto);

            var service = CreateService();

            #endregion Arrange

            #region Act

            await service.UpdateAsync(_device);

            #endregion Act

            #region Assert

            _deviceRepositoryMock.Verify(o => o.UpdateAsync(It.Is<DeviceEntity>(f => f.Id == _device.Id)), Times.Once);
            _farmRepositoryMock.Verify(o => o.GetSingleAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);
            _messagingMock.Verify(o => o.Publish(It.Is<EventDto<DeviceDto>>(e => e.Event.Id == _device.Id)), Times.Once);

            Assert.NotNull(sentEvent);
            Assert.Equal(_device.Id, sentEvent!.Event.Id);
            Assert.Equal(EventStatus.Update, sentEvent!.Status);

            #endregion Assert
        }
    }
}
