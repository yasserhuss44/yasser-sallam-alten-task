using Common.Helpers.Generics;
using Common.Messaging.Models;
using Customers.Domain.Services;
using Customers.Infrastructure.Entities;
using Customers.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Helpers.Models.CommonEnums;

namespace Customers.Tests.UnitTests
{
    public class CustomerServiceShould
    {
        private Mock<ICustomerVehicleRepository> customerVehicleServiceMoq;
        private Mock<ICustomerRepository> customerServiceMoq;
        private Mock<ILogger<CustomerService>> loggerMoq;

        public CustomerServiceShould()
        {
            InitializeTests();
        }

        #region Get All Vehicles
        [InlineData("all")]
        [InlineData("")]
        [Theory]
        public void ReturnSuccessAndTwoItems(string searchTag)
        {

            var customerService = new CustomerService(loggerMoq.Object, customerServiceMoq.Object, customerVehicleServiceMoq.Object);

            //Act

            var result = customerService.GetAllVehicles(searchTag);
            //Assert       
            Assert.Equal(ResponseStatusCode.Success, result.StatusCode);
            Assert.Equal(2, result.ItemsList.Count);
        }

        [InlineData("Yasser")]
        [InlineData("Ahmed")]
        [InlineData("ABC")]
        [InlineData("DEF")]
        [InlineData("Cairo")]
        [InlineData("Assiut")]
        [Theory]
        public void ReturnSuccessAndOneItem(string searchTag)
        {
            //Arrange

            var customerService = new CustomerService(loggerMoq.Object, customerServiceMoq.Object, customerVehicleServiceMoq.Object);

            //Act

            var result = customerService.GetAllVehicles(searchTag);
            //Assert       
            Assert.Equal(ResponseStatusCode.Success, result.StatusCode);
            Assert.Single(result.ItemsList);
        }

        [Fact]
        public void ReturnNotFound()
        {
            //Arrange

            var customerService = new CustomerService(loggerMoq.Object, customerServiceMoq.Object, customerVehicleServiceMoq.Object);

            //Act
            var result = customerService.GetAllVehicles("John");

            //Assert       
            Assert.Equal(ResponseStatusCode.NotFound, result.StatusCode);
        }
        [Fact]
        public void ReturnServerError()
        {
            //Arrange

            customerVehicleServiceMoq.Setup(m => m.DbSet).Throws(new System.Exception()); ;

            var customerService = new CustomerService(loggerMoq.Object, customerServiceMoq.Object, customerVehicleServiceMoq.Object);

            //Act
            var result = customerService.GetAllVehicles("John");

            //Assert       
            Assert.Equal(ResponseStatusCode.ServerError, result.StatusCode);
        }

        #endregion

        #region Update Vehicle Status
        [Fact]
        public void ReturnSuccessForUpdateVehicleStatus()
        {
            var customerService = new CustomerService(loggerMoq.Object, customerServiceMoq.Object, customerVehicleServiceMoq.Object);

            //Act
            var result = customerService.UpdateVehicleStatus(new PingVehicleModel { VehicleId="Test123"});

            //Assert       
            Assert.Equal(ResponseStatusCode.Success, result.StatusCode);
        }
        [Fact]
        public void ReturnInvalidInputUpdateVehicleStatus()
        {
            var customerService = new CustomerService(loggerMoq.Object, customerServiceMoq.Object, customerVehicleServiceMoq.Object);

            //Act
            var nullResult = customerService.UpdateVehicleStatus(null);
            //Act
            var emptyVehicle = customerService.UpdateVehicleStatus(new PingVehicleModel { VehicleId = "" });

            //Assert       
            Assert.Equal(ResponseStatusCode.InvalidInputs, nullResult.StatusCode);
            Assert.Equal(ResponseStatusCode.InvalidInputs, emptyVehicle.StatusCode);
        }
        [Fact]
        public void ReturnNotFoundForUpdateVehicleStatus ()
        {
            //Arrange

            var customerService = new CustomerService(loggerMoq.Object, customerServiceMoq.Object, customerVehicleServiceMoq.Object);

            //Act
            var result = customerService.UpdateVehicleStatus(new PingVehicleModel { VehicleId = "14221" });

            //Assert       
            Assert.Equal(ResponseStatusCode.NotFound, result.StatusCode);
        }
        [Fact]
        public void ReturnNotFoundForUpdateVehicleStatusForEmptyList()
        {
            //Arrange
            var moqList = new List<CustomerVehicle>();
            DbSet<CustomerVehicle> myDbSet = MockHelper.GetQueryableMockDbSet(moqList);
            customerVehicleServiceMoq.Setup(m => m.DbSet).Returns(myDbSet);

            var customerService = new CustomerService(loggerMoq.Object, customerServiceMoq.Object, customerVehicleServiceMoq.Object);

            //Act
            var result = customerService.UpdateVehicleStatus(new PingVehicleModel {VehicleId="Test123" });

            //Assert       
            Assert.Equal(ResponseStatusCode.NotFound, result.StatusCode);
        }
        [Fact]
        public void ReturnServerErrorForUpdateVehicleStatus()
        {
            //Arrange
            customerVehicleServiceMoq.Setup(m => m.DbSet).Throws(new System.Exception()); ;
            var customerService = new CustomerService(loggerMoq.Object, customerServiceMoq.Object, customerVehicleServiceMoq.Object);

            //Act
            var result = customerService.UpdateVehicleStatus(new PingVehicleModel { VehicleId = "Test123" });

            //Assert       
            Assert.Equal(ResponseStatusCode.ServerError, result.StatusCode);
        }
        #endregion

        #region Set Up
        private void InitializeTests()
        {
            customerVehicleServiceMoq = new Mock<ICustomerVehicleRepository>();
            customerServiceMoq = new Mock<ICustomerRepository>();
            var moqList = new List<CustomerVehicle>() { new CustomerVehicle("Test123", "ABC123", "Yasser", "Assiut Egypt")
                ,new CustomerVehicle("Test2", "DEF123", "Ahmed", "Cairo Egypt")
            };
            DbSet<CustomerVehicle> myDbSet = MockHelper.GetQueryableMockDbSet(moqList);
            customerVehicleServiceMoq.Setup(m => m.DbSet).Returns(myDbSet);

            loggerMoq = new Mock<ILogger<CustomerService>>();

        }
     
        #endregion
        
    }


}
