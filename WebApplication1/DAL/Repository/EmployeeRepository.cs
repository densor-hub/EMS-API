using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

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

        return employee;
    }

    public async Task<Employee> UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);

        return employee;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return false;

        employee.SoftDelete();
        _context.Employees.Update(employee);
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Employees.AnyAsync(e => e.Id == id);
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

    public async Task<Employee> GetByCodeAsync(string Code)
    {
        return await _context.Employees.Where(x=> x.Code.ToUpper().Trim() == Code.ToUpper().Trim()).FirstOrDefaultAsync();
    }
}