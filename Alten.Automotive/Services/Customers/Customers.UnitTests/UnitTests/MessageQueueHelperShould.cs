using Common.Messaging.Models;
using Common.Messaging.Queues;
using Customers.Domain.Services;
using Helpers.Models;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using Xunit;
using static Helpers.Models.CommonEnums;

namespace Customers.Tests.UnitTests
{
    public class MessageQueueHelperShould
    {

        private Mock<ICustomerService> customerServiceMoq;
        private Mock<ILogger<MessageQueueHelper>> loggerMoq;
        private Mock<IMessagingQueueHandler> messaginQueueHandlerMoq;

        public MessageQueueHelperShould()
        {
            InitializeTests();
        }

        #region Pull Message From Service Bus
        [Fact]
        public void ReturnTrueIfMessagePulledFromQueue()
        {
            //Arrange

            var customerService = new MessageQueueHelper(loggerMoq.Object, customerServiceMoq.Object, messaginQueueHandlerMoq.Object);

            //Act

            var result = customerService.PullMessageFromServiceBus(null);
       
            //Assert       
            Assert.True(result);
        }

        [Fact]
        public void ReturnFalseIfMessageNotPulledFromQueue()
        {
            //Arrange
            messaginQueueHandlerMoq.Setup(mq => mq.PullMessage<PingVehicleModel>(It.IsAny<IModel>())).Returns(new ResponseDetails<PingVehicleModel, ulong>(ResponseStatusCode.NotFound));

            var customerService = new MessageQueueHelper(loggerMoq.Object, customerServiceMoq.Object, messaginQueueHandlerMoq.Object);

            //Act

            var result = customerService.PullMessageFromServiceBus(null);

            //Assert       
            Assert.False(result);
        }

        [Fact]
        public void ReturnFalseIfMessagePulledFromQueueButNotUpdateInDb()
        {
            //Arrange
            customerServiceMoq.Setup(cs => cs.UpdateVehicleStatus(It.IsAny<PingVehicleModel>())).Returns(new ResponseDetailsBase(ResponseStatusCode.NotFound));

            var customerService = new MessageQueueHelper(loggerMoq.Object, customerServiceMoq.Object, messaginQueueHandlerMoq.Object);

            //Act

            var result = customerService.PullMessageFromServiceBus(null);

            //Assert       
            Assert.False(result);
        }
        #endregion

        #region Set Up
        private void InitializeTests()
        {
            customerServiceMoq = new Mock<ICustomerService>();
            customerServiceMoq.Setup(cs => cs.UpdateVehicleStatus(It.IsAny<PingVehicleModel>())).Returns(new ResponseDetailsBase(ResponseStatusCode.Success));

            messaginQueueHandlerMoq = new Mock<IMessagingQueueHandler>();
            messaginQueueHandlerMoq.Setup(mq => mq.PullMessage<PingVehicleModel>(It.IsAny<IModel>())).Returns(new ResponseDetails<PingVehicleModel, ulong>(ResponseStatusCode.Success));

            loggerMoq = new Mock<ILogger<MessageQueueHelper>>();
        }
     

        #endregion
    }
}
