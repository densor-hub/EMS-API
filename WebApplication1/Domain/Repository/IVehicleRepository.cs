using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Interfaces
{
    public interface IVehicleRepository
    {
        Task<Vehicle> GetByIdAsync(Guid id);
        Task<IQueryable<Vehicle>> GetByLocationIdAsync(Guid locationId, string? typeOfLocation);
        Task<IQueryable<Vehicle>> GetAllAsync();
        Task AddAsync(Vehicle vehicle);
        Task UpdateAsync(Vehicle vehicle);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        // Additional queries
        Task<bool> VehicleNumberExistsAsync(string vehicleNumber);
    }
}