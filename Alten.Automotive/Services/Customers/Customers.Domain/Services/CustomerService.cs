using Common.Messaging.Models;
using Customers.Domain.Models;
using Customers.Infrastructure.Repositories;
using Helpers.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using static Helpers.Models.CommonEnums;

namespace Customers.Domain.Services
{
    public class CustomerService : ICustomerService
    {

        ///
        private readonly ICustomerRepository _customerRepository;
        private ICustomerVehicleRepository _customerVehicleRepository;
        private readonly ILogger _logger;

        public CustomerService(ILogger<CustomerService> logger, ICustomerRepository customerRepository, ICustomerVehicleRepository customerVehicleRepository)
        {
            _customerRepository = customerRepository;
            _customerVehicleRepository = customerVehicleRepository;
            _logger = logger;
        }


        public ResponseDetailsList<CustomerVehicleAggregate> GetAllVehicles(string searchTage)
        {
            try
            {
                var customerVehicleQuery = _customerVehicleRepository.DbSet.AsQueryable();
                if (!string.IsNullOrEmpty(searchTage) && searchTage != "all")
                {
                    var searchCustomerNameResult = customerVehicleQuery.Where(cv => cv.Owner.Name.ToLowerInvariant().Contains(searchTage.ToLowerInvariant()) || cv.Owner.Address.ToLowerInvariant().Contains(searchTage.ToLowerInvariant()));
                    var searchVehicleResult = customerVehicleQuery.Where(cv => cv.VehicleId.ToLowerInvariant().Contains(searchTage.ToLowerInvariant()) || cv.RegNumber.ToLowerInvariant().Contains(searchTage.ToLowerInvariant()));

                    customerVehicleQuery = searchCustomerNameResult.Union(searchVehicleResult);
                }

                var customerVehicleList = customerVehicleQuery.Select(cv => new CustomerVehicleAggregate
                {
                    Customer = new CustomerDto { Name = cv.Owner.Name, Address = $"{cv.Owner.Address}" },
                    Vehicle = new VehicleDto { VehicleId = cv.VehicleId, RegNumber = cv.RegNumber, IsConnected = cv.LastPingTime.HasValue && DateTime.UtcNow <= cv.LastPingTime.Value.AddMinutes(1) }
                }).ToList();
                if (customerVehicleList.Any())
                {
                    return new ResponseDetailsList<CustomerVehicleAggregate>(customerVehicleList);
                }

                return new ResponseDetailsList<CustomerVehicleAggregate>(ResponseStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return new ResponseDetailsList<CustomerVehicleAggregate>(ex);
            }
        }

        public ResponseDetailsBase UpdateVehicleStatus(PingVehicleModel pingVehicle)
        {
            try
            {
                if(pingVehicle== null || string.IsNullOrEmpty(pingVehicle.VehicleId))
                    return new ResponseDetailsBase(ResponseStatusCode.InvalidInputs);

                var vehicle = _customerVehicleRepository.DbSet.FirstOrDefault(v => v.VehicleId.ToLowerInvariant() == pingVehicle.VehicleId.ToLowerInvariant());
                if (vehicle == null)
                {
                    return new ResponseDetailsBase(ResponseStatusCode.NotFound);
                }

                vehicle.LastPingTime = pingVehicle.CreatedAt;
                vehicle.UpdatedOn = DateTime.UtcNow;
                _customerVehicleRepository.Update(vehicle);
                _customerVehicleRepository.Save();
                return new ResponseDetailsList<CustomerVehicleAggregate>(ResponseStatusCode.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return new ResponseDetailsBase(ex);
            }
        }
    }

}

