// Services/IEmployeeService.cs
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<GetEmployeeDto>> GetAllEmployeesAsync(Guid? locationId, bool onlyActive);
        Task<GetEmployeeDto> GetEmployeeByIdAsync(Guid id);
        Task<object> CreateEmployeeAsync(CreateEmployeeDto createDto, Guid createdAtLocation);
        Task UpdateEmployeeAsync(UpdateEmployeeDto updateDto);
        Task DeleteEmployeeAsync(Guid id);
    }
}