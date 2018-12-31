using Helpers.Models;
using Vehicles.Domain.Models;

namespace Vehicles.Domain.Services
{
    public interface IMessageQueueHelper
    {
        ResponseDetailsBase PushMessageToQueue(VehicleDto vehicle);
    }
}