using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class CreateFinancialServiceProviderDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public Guid LocationId { get; set; }
        public GeneralStatus Status { get; set; }
        public FinancialServiceProviderType Type { get; set; }
        public List<CreateFinancialServiceProviderContactPersonDto> ContactPersons { get; set; } = new List<CreateFinancialServiceProviderContactPersonDto>();
    }

    public class CreateFinancialServiceProviderContactPersonDto
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        public string Location { get; set; }
    }

    public class UpdateFinancialServiceProviderContactPersonDto
    {
        public Guid Id { get; set; }
        [Required]
        public string FullName { get; set; }
        public GeneralStatus Status { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
    }

    public class FinancialServiceProviderUpdateDto
    {
        public string Name { get; set; }
        public GeneralStatus? Status { get; set; }
        public string Address { get; set; }
        public List<UpdateFinancialServiceProviderContactPersonDto> ContactPersons { get; set; } = new List<UpdateFinancialServiceProviderContactPersonDto>();
        public FinancialServiceProviderType? Type { get; set; } = null;
    }
    public class FinancialServiceProviderResponseDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public List<FinancialServiceProviderContactPersonResponseDto> ContactPersons { get; set; }
    }

    public class FinancialServiceProviderDropdownDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class FinancialServiceProviderContactPersonResponseDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public Guid FinancialServiceProviderId { get; set; }
        public string Status { get; set; }
    }

    public class FinancialServiceProviderContactPersonListDto
    {
        public List<CreateFinancialServiceProviderContactPersonDto> ContactPersonsToAdd { get; set; } = new List<CreateFinancialServiceProviderContactPersonDto>();
        public List<Guid> ContactPersonsToRemove { get; set; } = new List<Guid>();
        public List<FinancialServiceProviderContactPersonUpdateItemDto> ContactPersonsToUpdate { get; set; } = new List<FinancialServiceProviderContactPersonUpdateItemDto>();
    }

    public class FinancialServiceProviderContactPersonUpdateItemDto
    {
        public Guid Id { get; set; }
        public UpdateFinancialServiceProviderContactPersonDto UpdateData { get; set; }
    }

    public class CreateFinancialServiceProviderDepositDto
    {
        public Guid FinancialServiceProviderId { get; set; }
        public Guid ContactPersonId { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public DateTime DepositDate { get; set; }
        public string CurrencyCode { get; set; }    
    }

    public class DepositResponseDto
    {
        public Guid DepositId { get; set; }
        public Guid FinancialServiceProviderId { get; set; }
        public string FinancialServiceProviderName { get; set; }
        public Guid ContactPersonId { get; set; }
        public string ContactPersonName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime DepositDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TransactionNumber { get; set; }
        public string Status { get; set; }
    }



}
