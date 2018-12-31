using Common.Messaging.Models;
using Common.Messaging.Queues;
using Helpers.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vehicles.Domain.Models;
using Vehicles.Domain.Services;
using static Helpers.Models.CommonEnums;
namespace Vehicles.Domain.Services
{
    public class MessageQueueHelper: IMessageQueueHelper
    {
        private readonly ILogger _logger;
        private readonly IMessagingQueueHandler _messaginQueueHandler;
        public MessageQueueHelper(ILogger<MessageQueueHelper> logger,  IMessagingQueueHandler messaginQueueHandler)
        {
            _logger = logger;
            _messaginQueueHandler = messaginQueueHandler;
        }
        public ResponseDetailsBase PushMessageToQueue(VehicleDto vehicle)
        {
            using (var channel = _messaginQueueHandler.CreateConnection())
            {
                _logger.LogInformation("Push Message To Service Bus is starting");
                if (vehicle==null || string.IsNullOrEmpty(vehicle.VehicleId))
                {
                    _logger.LogInformation("Cannot Retrieve Random Vehicle");
                    return new ResponseDetailsBase(ResponseStatusCode.InvalidInputs);
                }
                var pingComm = new PingVehicleModel()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    VehicleId =vehicle.VehicleId
                };
                var pushMessageResponse = _messaginQueueHandler.PushMessage(pingComm, channel);
                if (pushMessageResponse.StatusCode != ResponseStatusCode.Success)
                {
                    _logger.LogInformation("Failed To Update Vehicle Status");
                }
                _logger.LogInformation("Vehicle Status Updated Successfully");
                return pushMessageResponse ;                
            }
        }
    }
}
