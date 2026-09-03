using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Interfaces;
using WebApplication1.Domain.Repository;

namespace WebApplication1.Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Vehicle> _vehicles;
        private readonly IUserRepository _userRepository;

        public VehicleRepository(AppDbContext context, IUserRepository userRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _vehicles = _context.Set<Vehicle>();
            _userRepository = userRepository;
        }

        public async Task<Vehicle?> GetByIdAsync(Guid id)
        {
            return await _vehicles
                .Include(x=> x.Location)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IQueryable<Vehicle>> GetByLocationIdAsync(Guid locationId, string? typeOfLocation)
        {
            var data = _vehicles
                .Include(x=> x.Location)
               // .Where(v => v.LocationId == locationId)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(typeOfLocation))
            {
                if (typeOfLocation.ToLower().Trim() == "company") return data.Where(x=> x.Location.CompanyId == locationId);

            }

            return data.Where(x => x.LocationId  == locationId);
        }

       

        public async Task<IQueryable<Vehicle>> GetAllAsync()
        {
            return  _vehicles
                .Include(x=> x.Location)
                .OrderBy(v => v.VehicleNumber)
                .AsNoTracking();
        }

        public async Task AddAsync(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            await _vehicles.AddAsync(vehicle);
        }

        public async Task UpdateAsync(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            _vehicles.Update(vehicle);
        }

        public async Task DeleteAsync(Guid id)
        {
            var vehicle = await GetByIdAsync(id);
            if (vehicle != null)
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                vehicle.BaseSoftDelete(Guid.Parse(user.Id));

                _context.Vehicles.Update(vehicle);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _vehicles.AnyAsync(v => v.Id == id);
        }

        public async Task<bool> VehicleNumberExistsAsync(string vehicleNumber)
        {
            return await _vehicles.AnyAsync(v => v.VehicleNumber.ToLower().Trim() == vehicleNumber.ToLower().Trim());
        }
    }
}