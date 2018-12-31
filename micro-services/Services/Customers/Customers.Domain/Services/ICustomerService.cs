using Helpers.Generics;
using Customers.Domain.Models;
using Helpers.Models;
using Common.Messaging.Models;

namespace Customers.Domain.Services
{
    public interface ICustomerService
    {

        ResponseDetailsList<CustomerVehicleAggregate> GetAllVehicles(string searchTage);

        ResponseDetailsBase UpdateVehicleStatus(PingVehicleModel pingVehicle);
        
                    
    }
}