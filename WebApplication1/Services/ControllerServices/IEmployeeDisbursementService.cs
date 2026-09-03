// Services/IEmployeeDisbursementService.cs
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs;

namespace WebApplication1.Services
{
    public interface IEmployeeDisbursementService
    {
        Task CreateAsync(CreateTransactionDto createDto);
        Task<EmployeeDisbursementResponseDto> GetByIdAsync(Guid id);
        Task<IEnumerable<EmployeeDisbursementResponseDto>> GetAllAsync(Guid locationId, Guid? employeeId, TransactionType? transactionType);
        Task<EmployeeDisbursementResponseDto> UpdateAsync(Guid id, UpdateEmployeeDisbursementDto updateDto);
        Task DeleteAsync(Guid id);
    }
}