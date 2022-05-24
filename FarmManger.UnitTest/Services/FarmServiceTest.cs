using AutoMapper;
using FarmManager.Domain.Dtos;
using FarmManager.Domain.Entities;
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
    public class FarmServiceTest
    {
        private readonly ILogger<FarmService> _logger;
        private readonly IMapper _mapper;

        private static Mock<IServiceProvider> BuildServiceProviderMock(IMessagingPublisher<FarmDto> messaging)
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IMessagingPublisher<FarmDto>)))
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

        public FarmServiceTest()
        {
            _mapper = SetupMapper();
            _logger = SetupLogger();
        }

        [Fact]
        public async Task Should_Create_A_New_Farm_When_Receive_Valid_Payloads()
        {
            #region Arrange

            EventDto<FarmDto>? sentEvent = null;

            var messagingMock = new Mock<IMessagingPublisher<FarmDto>>();
            messagingMock.Setup(o => o.Publish(It.IsAny<EventDto<FarmDto>>()))
                .Callback((EventDto<FarmDto> eventDto) => sentEvent = eventDto);

            var serviceProviderMock = BuildServiceProviderMock(messagingMock.Object);

            var repositoryMock = new Mock<IRepository<FarmEntity>>();

            var service = new FarmService(serviceProviderMock.Object, _mapper, repositoryMock.Object, _logger);

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");

            #endregion Arrange

            #region Act

            await service.CreateAsync(farm);

            #endregion Act

            #region Assert

            repositoryMock.Verify(o => o.AddAsync(It.Is<FarmEntity>(f => f.Id == farm.Id)), Times.Once);
            messagingMock.Verify(o => o.Publish(It.Is<EventDto<FarmDto>>(e => e.Event.Id == farm.Id)), Times.Once);

            Assert.NotNull(sentEvent);
            Assert.Equal(farm.Id, sentEvent!.Event.Id);
            Assert.Equal(EventStatus.Create, sentEvent!.Status);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Create_A_New_Farm_When_Occours_An_Unexpected_Error()
        {
            #region Arrange

            var messagingMock = new Mock<IMessagingPublisher<FarmDto>>();

            var serviceProviderMock = BuildServiceProviderMock(messagingMock.Object);

            var repositoryMock = new Mock<IRepository<FarmEntity>>();
            repositoryMock.Setup(o => o.AddAsync(It.IsAny<FarmEntity>()))
                .ThrowsAsync(new TimeoutException());

            var service = new FarmService(serviceProviderMock.Object, _mapper, repositoryMock.Object, _logger);

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<TimeoutException>(async () => await service.CreateAsync(farm));

            #endregion Act

            #region Assert

            repositoryMock.Verify(o => o.AddAsync(It.Is<FarmEntity>(f => f.Id == farm.Id)), Times.Once);
            messagingMock.Verify(o => o.Publish(It.Is<EventDto<FarmDto>>(e => e.Event.Id == farm.Id)), Times.Never);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Update_A_New_Farm_When_Receive_Valid_Payloads()
        {
            #region Arrange

            EventDto<FarmDto>? sentEvent = null;

            var messagingMock = new Mock<IMessagingPublisher<FarmDto>>();
            messagingMock.Setup(o => o.Publish(It.IsAny<EventDto<FarmDto>>()))
                .Callback((EventDto<FarmDto> eventDto) => sentEvent = eventDto);

            var serviceProviderMock = BuildServiceProviderMock(messagingMock.Object);

            var repositoryMock = new Mock<IRepository<FarmEntity>>();

            var service = new FarmService(serviceProviderMock.Object, _mapper, repositoryMock.Object, _logger);

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");

            #endregion Arrange

            #region Act

            await service.UpdateAsync(farm);

            #endregion Act

            #region Assert

            repositoryMock.Verify(o => o.UpdateAsync(It.Is<FarmEntity>(f => f.Id == farm.Id)), Times.Once);
            messagingMock.Verify(o => o.Publish(It.Is<EventDto<FarmDto>>(e => e.Event.Id == farm.Id)), Times.Once);

            Assert.NotNull(sentEvent);
            Assert.Equal(farm.Id, sentEvent!.Event.Id);
            Assert.Equal(EventStatus.Update, sentEvent!.Status);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Update_A_New_Farm_When_Occours_An_Unexpected_Error()
        {
            #region Arrange

            var messagingMock = new Mock<IMessagingPublisher<FarmDto>>();

            var serviceProviderMock = BuildServiceProviderMock(messagingMock.Object);

            var repositoryMock = new Mock<IRepository<FarmEntity>>();
            repositoryMock.Setup(o => o.UpdateAsync(It.IsAny<FarmEntity>()))
                .ThrowsAsync(new TimeoutException());

            var service = new FarmService(serviceProviderMock.Object, _mapper, repositoryMock.Object, _logger);

            var farm = new FarmDto(Guid.NewGuid(), "The Farm");

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<TimeoutException>(async () => await service.UpdateAsync(farm));

            #endregion Act

            #region Assert

            repositoryMock.Verify(o => o.UpdateAsync(It.Is<FarmEntity>(f => f.Id == farm.Id)), Times.Once);
            messagingMock.Verify(o => o.Publish(It.Is<EventDto<FarmDto>>(e => e.Event.Id == farm.Id)), Times.Never);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Delete_A_New_Farm_When_Receive_Valid_Payloads()
        {
            #region Arrange

            EventDto<FarmDto>? sentEvent = null;

            var messagingMock = new Mock<IMessagingPublisher<FarmDto>>();
            messagingMock.Setup(o => o.Publish(It.IsAny<EventDto<FarmDto>>()))
                .Callback((EventDto<FarmDto> eventDto) => sentEvent = eventDto);

            var serviceProviderMock = BuildServiceProviderMock(messagingMock.Object);

            var repositoryMock = new Mock<IRepository<FarmEntity>>();

            var service = new FarmService(serviceProviderMock.Object, _mapper, repositoryMock.Object, _logger);

            var farmId = Guid.NewGuid();

            #endregion Arrange

            #region Act

            await service.DeleteAsync(farmId);

            #endregion Act

            #region Assert

            repositoryMock.Verify(o => o.RemoveAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);
            messagingMock.Verify(o => o.Publish(It.Is<EventDto<FarmDto>>(e => e.Event.Id == farmId)), Times.Once);

            Assert.NotNull(sentEvent);
            Assert.Equal(farmId, sentEvent!.Event.Id);
            Assert.Equal(EventStatus.Delete, sentEvent!.Status);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Delete_A_New_Farm_When_Occours_An_Unexpected_Error()
        {
            #region Arrange

            var messagingMock = new Mock<IMessagingPublisher<FarmDto>>();

            var serviceProviderMock = BuildServiceProviderMock(messagingMock.Object);

            var repositoryMock = new Mock<IRepository<FarmEntity>>();
            repositoryMock.Setup(o => o.RemoveAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()))
                .ThrowsAsync(new TimeoutException());

            var service = new FarmService(serviceProviderMock.Object, _mapper, repositoryMock.Object, _logger);

            var farmId = Guid.NewGuid();

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<TimeoutException>(async () => await service.DeleteAsync(farmId));

            #endregion Act

            #region Assert

            repositoryMock.Verify(o => o.RemoveAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);
            messagingMock.Verify(o => o.Publish(It.Is<EventDto<FarmDto>>(e => e.Event.Id == farmId)), Times.Never);

            #endregion Assert
        }

        [Fact]
        public async Task Should_Get_All_Farms_When_There_Are_In_Repository()
        {
            #region Arrange

            var messagingMock = new Mock<IMessagingPublisher<FarmDto>>();

            var serviceProviderMock = BuildServiceProviderMock(messagingMock.Object);

            var repositoryMock = new Mock<IRepository<FarmEntity>>();
            repositoryMock.Setup(o => o.GetAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()))
                .ReturnsAsync(new[]
                {
                    new FarmEntity { Id = Guid.NewGuid(), Name = "The Farm" },
                    new FarmEntity { Id = Guid.NewGuid(), Name = "West Farm" }
                });

            var service = new FarmService(serviceProviderMock.Object, _mapper, repositoryMock.Object, _logger);

            #endregion Arrange

            #region Act

            var farms = await service.GetAsync();

            #endregion Act

            #region Assert

            repositoryMock.Verify(o => o.GetAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);

            Assert.NotEmpty(farms);
            Assert.Equal(2, farms!.Count());

            #endregion Assert
        }

        [Fact]
        public async Task Should_Not_Get_All_Farms_When_Occours_An_Unexpected_Error()
        {
            #region Arrange

            var messagingMock = new Mock<IMessagingPublisher<FarmDto>>();

            var serviceProviderMock = BuildServiceProviderMock(messagingMock.Object);

            var repositoryMock = new Mock<IRepository<FarmEntity>>();
            repositoryMock.Setup(o => o.GetAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()))
                .ThrowsAsync(new TimeoutException());

            var service = new FarmService(serviceProviderMock.Object, _mapper, repositoryMock.Object, _logger);

            #endregion Arrange

            #region Act

            await Assert.ThrowsAsync<TimeoutException>(async () => await service.GetAsync());

            #endregion Act

            #region Assert

            repositoryMock.Verify(o => o.GetAsync(It.IsAny<Expression<Func<FarmEntity, bool>>>()), Times.Once);

            #endregion Assert
        }
    }
}
