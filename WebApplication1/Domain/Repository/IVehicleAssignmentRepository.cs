using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Interfaces
{
    public interface IVehicleAssignmentRepository
    {
        Task<VehicleAssignments?> GetByIdAsync(Guid id);
        Task<IQueryable<VehicleAssignments>> GetAllAsync();
        Task AddAsync(VehicleAssignments assignment);
        Task UpdateAsync(VehicleAssignments assignment);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task SaveChangesAsync();

    }
}