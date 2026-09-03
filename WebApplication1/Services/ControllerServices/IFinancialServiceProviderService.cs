// Services/IBankService.cs
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs;

namespace WebApplication1.Services.ControllerServices
{
    public interface IFinancialServiceProviderService
    {
        Task<FinancialServiceProviderResponseDto> CreateBankAsync(CreateFinancialServiceProviderDto createDto);
        Task<FinancialServiceProviderResponseDto> UpdateBankAsync(Guid bankId, FinancialServiceProviderUpdateDto updateDto);
        Task DeleteBankAsync(Guid bankId);
        Task<IEnumerable<FinancialServiceProviderDropdownDto>> GetBanksForDropdownAsync(Guid locationId, GeneralStatus? status);
        Task MakeDepositAsync(CreateTransactionDto depositDto);
        Task<IEnumerable<FinancialServiceProviderContactPersonResponseDto>> GetAllContactPersonsAsync(Guid bankId, string? filter, GeneralStatus? status);
    }
}