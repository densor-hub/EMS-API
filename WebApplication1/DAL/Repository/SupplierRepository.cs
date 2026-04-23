using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;
using WebApplication1.Helpers;

namespace WebApplication1.DAL.Repository;

public class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext _context;

    public SupplierRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Supplier> GetAllAsync(Guid compnayId)
    {
        return _context.Suppliers.Where(x => x.CompanyId == compnayId);
    }

    public IQueryable<Supplier> GetAllByLocationAsync(Guid locationId)
    {
        return _context.Suppliers.Where(x => x.SupplierLocations.Any(sl=> sl.LocationId ==  locationId));
    }
    public async Task<Supplier> GetByIdAsync(Guid id)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(e => e.Id == id);

        if (supplier == null)
            return null;

        return supplier;
    }

    public async Task<Supplier> CreateAsync(Supplier supplier)
    {
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return supplier;
    }

    public async Task<Supplier> UpdateAsync(Supplier supplier)
    {
        _context.Suppliers.Update(supplier);
        await _context.SaveChangesAsync();

        return supplier;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var employee = await _context.Suppliers.FindAsync(id);
        if (employee == null)
            return false;

        _context.Suppliers.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Suppliers.AnyAsync(e => e.Id == id);
    }

    public async Task<string> GenerateCodeAsync(Guid comapnyId)
    {
        var locationName = _context.Companies.Where(x=> x.Id== comapnyId).Select(x=> x.Name).FirstOrDefault();   
        var initials = StringExtensions.GetInitials(locationName);

        var totalCount = await _context.Suppliers.Where(x=> x.SupplierLocations.Any(sl => sl.Location.CompanyId == comapnyId)).CountAsync();

        var number = totalCount + 1;
        return $"SUP-{initials.ToUpper().Trim()}-{number:D3}";
    }
}