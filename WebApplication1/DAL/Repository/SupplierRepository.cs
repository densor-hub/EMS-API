using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

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
    public async Task<Supplier?> GetByIdAsync(Guid id)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(e => e.Id == id);

        return supplier;
    }

    public async Task<Supplier> CreateAsync(Supplier supplier)
    {
        _context.Suppliers.Add(supplier);
        await Task.CompletedTask;
        return supplier;
    }

    public async Task<Supplier> UpdateAsync(Supplier supplier)
    {
        _context.Suppliers.Update(supplier);
        await Task.CompletedTask;
        return supplier;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var employee = await _context.Suppliers.FindAsync(id);
        if (employee == null)
            return false;

        _context.Suppliers.Remove(employee);
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Suppliers.AnyAsync(e => e.Id == id);
    }

    public async Task<Supplier?> GetByCodeAsync(string Code)
    {
        return await _context.Suppliers.Where(x => x.Code.ToUpper().Trim() == Code.ToUpper().Trim()).FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync()
    {
        // throw new NotImplementedException();
        await _context.SaveChangesAsync();
    }
}