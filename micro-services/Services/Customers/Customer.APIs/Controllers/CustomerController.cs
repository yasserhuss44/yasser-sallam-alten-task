using Customers.Domain.Models;
using Customers.Domain.Services;
using Helpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace Customers.APIs.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private ICustomerService _customerService;
        private IMessageQueueHelper _messageQueueHelper;

        public CustomerController(ICustomerService customerService, IMessageQueueHelper messageQueueHelper )
        {
            _customerService = customerService;
            _messageQueueHelper = messageQueueHelper;
        }


        [HttpGet("GetAllVehicles/{searchTage}", Name = "GetAllVehicles")]
        public ActionResult<ResponseDetailsList<CustomerVehicleAggregate>> GetAllVehicles(string searchTage)
        {
            return _customerService.GetAllVehicles(searchTage);
        }


        [HttpGet("CheckForWaitingMessages", Name = "CheckForWaitingMessages")]
        public void CheckForWaitingMessages()
        {
             _messageQueueHelper.PullMessageFromServiceBus(null);
        }
    }
}
