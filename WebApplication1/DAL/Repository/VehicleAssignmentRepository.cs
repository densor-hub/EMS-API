using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Interfaces;

namespace WebApplication1.Infrastructure.Repositories
{
    public class VehicleAssignmentRepository : IVehicleAssignmentRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<VehicleAssignments> _assignments;

        public VehicleAssignmentRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _assignments = _context.Set<VehicleAssignments>();
        }

        public async Task<VehicleAssignments?> GetByIdAsync(Guid id)
        {
            return await _assignments
                .Include(a => a.Driver)
                .Include(a => a.Vehicle)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IQueryable<VehicleAssignments>> GetAllAsync()
        {
            var data =   _assignments
                .Include(a => a.Driver)
                .Include(a => a.Vehicle)
                    .ThenInclude( a=> a.Location)
                .OrderByDescending(a => a.AssignedtDate)
                //.Where(x=> x.Vehicle.LocationId == locationId)
                .AsNoTracking();


            return data;
        }

        public async Task AddAsync(VehicleAssignments assignment)
        {
            if (assignment == null)
                throw new ArgumentNullException(nameof(assignment));

            await _assignments.AddAsync(assignment);
        }

        public async Task UpdateAsync(VehicleAssignments assignment)
        {
            if (assignment == null)
                throw new ArgumentNullException(nameof(assignment));

            _assignments.Update(assignment);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var assignment = await GetByIdAsync(id);
            if (assignment != null)
            {
                _assignments.Remove(assignment);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _assignments.AnyAsync(a => a.Id == id);
        }

        public async Task SaveChangesAsync()
        {
          await  _context.SaveChangesAsync();
        }
    }
}