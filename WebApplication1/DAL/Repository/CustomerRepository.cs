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

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Customer> GetAllAsync(Guid locationId)
    {
        return _context.Customers.Where(x => x.LocationId == locationId);
    }

    public async Task<Customer> GetByIdAsync(Guid id)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(e => e.Id == id);

        if (customer == null)
            return null;

        return customer;
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return customer;
    }

    public async Task<Customer> UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();

        return customer;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
            return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Employees.AnyAsync(e => e.Id == id);
    }

    public async Task<string> GenerateCodeAsync(Guid locationId)
    {
        var locationName = _context.Locations.Where(x=> x.Id== locationId).Select(x=> x.Name).FirstOrDefault();   
        var initials = StringExtensions.GetInitials(locationName);

        var totalCount = await _context.Customers.Where(x=> x.LocationId == locationId).CountAsync();

        var number = totalCount + 1;
        return $"CUS-{initials.ToUpper().Trim()}-{number:D3}";
    }
}