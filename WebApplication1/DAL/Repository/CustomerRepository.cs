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

        return customer;
    }

    public async Task<Customer> UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);

        return customer;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
            return false;

        _context.Customers.Remove(customer);
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Customers.AnyAsync(e => e.Id == id);
    }

    

    public async Task<Customer> GetByCodeAsync(string Code)
    {
        return await _context.Customers.Where(x => x.Code.ToUpper().Trim() == Code.ToUpper().Trim()).FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}