using Common.Helpers.Generics;
using Common.Messaging.Models;
using Common.Messaging.Queues;
using Helpers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Linq;
using Vehicles.Domain.Models;
using Vehicles.Domain.Services;
using Vehicles.Infrastructure.Entities;
using Vehicles.Infrastructure.Repositories;
using Xunit;
using static Helpers.Models.CommonEnums;

namespace Vehicles.Tests.UnitTests
{
    public class VehicleServiceShould
    {
        private Mock<ILogger<VehicleService>> loggerMoq;
        private Mock<IMessageQueueHelper> messageQueueHelperMoq;
        private Mock<IVehicleRepository> vehicleRepositoryMoq;

        public VehicleServiceShould()
        {
            InitializeTests();
        }

        #region Pull Message From Service Bus
        [InlineData("Test456")]
        [InlineData("Test123")]
        [InlineData("random")]
        [Theory]
        public void ReturnSuccessForPingVehicle(string vehicleId)
        {
            //Arrange

            var vehicleService = new VehicleService(loggerMoq.Object,vehicleRepositoryMoq.Object, messageQueueHelperMoq.Object);

            //Act

            var result = vehicleService.PingVehicle(vehicleId);

            //Assert       
            Assert.Equal(ResponseStatusCode.Success, result.StatusCode);
        }

        [InlineData("Test000")]
        [InlineData("Test00")]
        [InlineData("random00")]
        [Theory]
        public void ReturnNotFoundForPingVehicle(string vehicleId)
        {
            //Arrange

            var vehicleService = new VehicleService(loggerMoq.Object, vehicleRepositoryMoq.Object, messageQueueHelperMoq.Object);

            //Act

            var result = vehicleService.PingVehicle(vehicleId);

            //Assert       
            Assert.Equal(ResponseStatusCode.NotFound, result.StatusCode);
        }


        [Fact]
        public void ReturnserverErrorForPingVehicle()
        {
            //Arrange
            messageQueueHelperMoq.Setup(mq => mq.PushMessageToQueue(It.IsAny<VehicleDto>())).Throws(new System.Exception("Custom Exception"));

            var vehicleService = new VehicleService(loggerMoq.Object, vehicleRepositoryMoq.Object, messageQueueHelperMoq.Object);

            //Act

            var result = vehicleService.PingVehicle("Test123");

            //Assert       
            Assert.Equal(ResponseStatusCode.ServerError, result.StatusCode);
        }

        #endregion

        #region Set Up
        private void InitializeTests()
        {

            messageQueueHelperMoq = new Mock<IMessageQueueHelper>();
            messageQueueHelperMoq.Setup(mq => mq.PushMessageToQueue(It.IsAny<VehicleDto>())).Returns(new ResponseDetailsBase(ResponseStatusCode.Success));

            var vehicleList = new List<Vehicle>()
            {
                new Vehicle("Test123","ABC123"),
                new Vehicle("Test456","DEF456"),
            };
            DbSet<Vehicle> myDbSet =MockHelper.GetQueryableMockDbSet(vehicleList);
            vehicleRepositoryMoq = new Mock<IVehicleRepository>();
            vehicleRepositoryMoq.Setup(vr => vr.DbSet).Returns(myDbSet) ;
            loggerMoq = new Mock<ILogger<VehicleService>>();
        }
     

        #endregion
    }
}
