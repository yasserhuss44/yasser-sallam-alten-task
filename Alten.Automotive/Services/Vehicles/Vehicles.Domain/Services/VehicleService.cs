using Helpers.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Vehicles.Domain.Models;
using Vehicles.Infrastructure.Repositories;
using static Helpers.Models.CommonEnums;

namespace Vehicles.Domain.Services
{
    public class VehicleService : IVehicleService
    {
        private IVehicleRepository _VehicleRepository;
        private readonly IMessageQueueHelper _messageQueueHelper;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(ILogger<VehicleService> logger,IVehicleRepository VehicleRepository, IMessageQueueHelper messageQueueHelper)
        {
            _VehicleRepository = VehicleRepository;
            _messageQueueHelper = messageQueueHelper;
            _logger = logger;
        }

       
        public ResponseDetailsBase PingVehicle(string vehicleId)
        {
            try
            {
                if (string.IsNullOrEmpty(vehicleId))
                    return new ResponseDetailsBase(ResponseStatusCode.InvalidInputs);
                VehicleDto vehicle;
                if (vehicleId == "random")
                {
                    vehicle = _VehicleRepository.DbSet.OrderBy(_ => Guid.NewGuid()).Select(v => new VehicleDto() { RegNumber = v.RegNumber, VehicleId = v.VehicleId }).Take(1).FirstOrDefault();
                }
                else
                    vehicle = _VehicleRepository.DbSet.Select(v => new VehicleDto() { RegNumber = v.RegNumber, VehicleId = v.VehicleId }).FirstOrDefault(v => v.VehicleId == vehicleId);
                if (vehicle == null)
                    return new ResponseDetailsBase(ResponseStatusCode.NotFound);
                return _messageQueueHelper.PushMessageToQueue(vehicle);

            }
            catch (Exception ex)
            {
                return new ResponseDetailsBase(ex);
            }
        }
                        
    }
}
