using Common.Messaging.Models;
using Common.Messaging.Queues;
using Helpers.Models;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using Vehicles.Domain.Models;
using Vehicles.Domain.Services;
using Xunit;
using static Helpers.Models.CommonEnums;

namespace Vehicles.Tests.UnitTests
{
   public class MessageQueueHelperShould
    {
        private Mock<ILogger<MessageQueueHelper>> loggerMoq;
        private Mock<IMessagingQueueHandler> messaginQueueHandlerMoq;

        public MessageQueueHelperShould()
        {
            InitializeTests();
        }

        #region Pull Message From Service Bus
        [Fact]
        public void ReturnSuccessIfMessagePushgedToQueue()
        {
            //Arrange

            var customerService = new MessageQueueHelper(loggerMoq.Object ,messaginQueueHandlerMoq.Object);

            //Act

            var result = customerService.PushMessageToQueue(new VehicleDto() {VehicleId="Test123" });

            //Assert       
            Assert.Equal(ResponseStatusCode.Success,result.StatusCode);
        }

        [Fact]
        public void ReturnInvalidInpust()
        {
            //Arrange

            var customerService = new MessageQueueHelper(loggerMoq.Object, messaginQueueHandlerMoq.Object);

            //Act

            var nullResult = customerService.PushMessageToQueue(new VehicleDto() { });
            var emptyResult = customerService.PushMessageToQueue(new VehicleDto() { VehicleId=""});

            //Assert       
            Assert.Equal(ResponseStatusCode.InvalidInputs, nullResult.StatusCode);
            Assert.Equal(ResponseStatusCode.InvalidInputs, emptyResult.StatusCode);
        }

        [InlineData(ResponseStatusCode.BusinessError)]
        [InlineData(ResponseStatusCode.ServerError)]
        [InlineData(ResponseStatusCode.Success)]
        [InlineData(ResponseStatusCode.NotFound)]
        [Theory]
        public void ReturnFailedIfMessageNotPushedToMessageQueue(ResponseStatusCode responseStatusCode)
        {
            //Arrange
            messaginQueueHandlerMoq.Setup(mq => mq.PushMessage<PingVehicleModel>(It.IsAny<PingVehicleModel>(), It.IsAny<IModel>())).Returns(new ResponseDetailsBase(responseStatusCode));

            var customerService = new MessageQueueHelper(loggerMoq.Object, messaginQueueHandlerMoq.Object);

            //Act

            var result = customerService.PushMessageToQueue(new VehicleDto() {VehicleId= "Test123" });

            //Assert       
            Assert.Equal(responseStatusCode, result.StatusCode);
        }
        #endregion

        #region Set Up
        private void InitializeTests()
        { 
            messaginQueueHandlerMoq = new Mock<IMessagingQueueHandler>();
            messaginQueueHandlerMoq.Setup(mq => mq.PushMessage<PingVehicleModel>( It.IsAny<PingVehicleModel>(),  It.IsAny<IModel>())).Returns(new ResponseDetailsBase(ResponseStatusCode.Success));

            loggerMoq = new Mock<ILogger<MessageQueueHelper>>();
        }


        #endregion
    }
}
