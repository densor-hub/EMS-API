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

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Employee> GetAllByCompanyId(Guid companyId)
    {
        return  _context.Employees.Where(x=> x.CompanyId == companyId)
            .Include(e => e.Position);
    }

    public async Task<Employee> GetByIdAsync(Guid id)
    {
        var employee = await _context.Employees
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
            return null;

        return employee;
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return employee;
    }

    public async Task<Employee> UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();

        return employee;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return false;

        employee.SoftDelete();
        _context.Employees.Update(employee);
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

        var totalCount = await _context.Employees.Where(x=> x.EmployeeLocations.Any(el=> el.LocationId == locationId)).CountAsync();

        var number = totalCount + 1;
        return $"EMP-{initials.ToUpper().Trim()}-{number:D3}";
    }

    public async Task<Employee?> GetByIdAndLocationAsync(Guid id, Guid locationId)
    {
        return await _context.Employees.Where(e => e.Id == id && e.EmployeeLocations.Any(el=> el.LocationId == locationId)).FirstOrDefaultAsync();
    }

    public IQueryable<Employee> GetAll()
    {
        return  _context.Employees;
    }

    public IQueryable<Employee> GetAllByLocationId(Guid locationId)
    {
        return  _context.Employees.Where(e => e.EmployeeLocations.Any(el => el.LocationId == locationId));
    }
}