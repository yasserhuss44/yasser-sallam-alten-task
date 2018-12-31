using static Helpers.Models.CommonEnums;
using Common.Messaging.Models;
using Common.Messaging.Queues;
using Microsoft.Extensions.Logging;

namespace Customers.Domain.Services
{
    public class MessageQueueHelper : IMessageQueueHelper
    {
        private readonly IMessagingQueueHandler _messaginQueueHandler;
        private readonly ILogger _logger;
        private readonly ICustomerService _customerService;

        public MessageQueueHelper(ILogger<MessageQueueHelper> logger,  ICustomerService customerService, IMessagingQueueHandler messaginQueueHandler)
        {
            this._logger = logger;
            this._customerService = customerService;
            this._messaginQueueHandler = messaginQueueHandler;
        }
        public bool PullMessageFromServiceBus(object state)
        {
            using (var channel = _messaginQueueHandler.CreateConnection())
            {
                var pullResponse = _messaginQueueHandler.PullMessage<PingVehicleModel>(channel);
                if (pullResponse == null || pullResponse.StatusCode != ResponseStatusCode.Success)
                {
                   
                    return false;
                }
                var pingVehicleResponse = _customerService.UpdateVehicleStatus(pullResponse.DetailsObject);
                if (pingVehicleResponse.StatusCode != ResponseStatusCode.Success)
                {
                    _logger.LogInformation("Failed To Update Vehicle");
                    return false;
                }
                _messaginQueueHandler.AckNowledge(pullResponse.SecondDetailsObject, channel);
                _logger.LogInformation("Pull Message From Service Bus Is Completed");
                return true;
            }
        }
    }
}
