using Helpers.Models;
namespace Vehicles.Domain.Services
{
    public interface IVehicleService
    {
        ResponseDetailsBase PingVehicle(string vehicleId);
    }
}