// Services/ICustomerService.cs
using WebApplication1.Domain.DTO;

namespace WebApplication1.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<GetCustomerDto>> GetAllCustomersAsync(Guid locationId);
        Task<GetCustomerDto> GetCustomerByIdAsync(Guid id);
        Task<object> CreateCustomerAsync(CreateCustomerDTO createDto);
        Task UpdateCustomerAsync(UpdateCustomerDTO updateDto);
        Task DeleteCustomerAsync(Guid id);
    }
}