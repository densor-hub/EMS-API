// Services/Implementations/LocationPaymentService.cs
using Microsoft.Extensions.Options;
using System.Text.Json;
using VMS.Modules.Licenses.Core.Emails.EmailSenderService.Entities;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;
using WebApplication1.DTOs;
using WebApplication1.Helpers;
using WebApplication1.Repositories;
using WebApplication1.Services.ControllerServices;
using WebApplication1.Services.Emails.EmailService.Entities;
using WebApplication1.Services.Emails.EmailService.Queuer;
using WebApplication1.Services.Emails.TemplateService.Enitities;
using WebApplication1.Services.TokenService;

namespace WebApplication1.Services.Implementations
{
    public class EmployeeDisbursementService : IEmployeeDisbursementService
    {
        private readonly ILocationPaymentRepository _locationPaymentRepository;
        private readonly ILogger<EmployeeDisbursementService> _logger;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionCodeRepository _transactionCodeRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IPaymentTokenService _paymentTokenService;
        private readonly IEmailQueueRepository _emailQueueRepository;
        private readonly ITransactionService _transactionService;
        private readonly AppDbContext _appDbContext;
        private readonly EmailSettings _emailSettings;

        public EmployeeDisbursementService(
            ILocationPaymentRepository locationPaymentRepository,
            ILogger<EmployeeDisbursementService> logger,
            IEmployeeRepository employeeRepository,
            ICurrencyRepository currencyRepository,
            IUserRepository userRepository,
            ITransactionCodeRepository transactionCodeRepository,
            ICompanyRepository companyRepository,
            IPaymentTokenService paymentTokenService,
            IEmailQueueRepository emailQueueRepository,
            ITransactionService transactionService,
        IOptions<EmailSettings> emailSettings,
        AppDbContext appDbContext
            )

        {
            _locationPaymentRepository = locationPaymentRepository;
            _logger = logger;
            _employeeRepository = employeeRepository;
            _currencyRepository = currencyRepository;
            _userRepository = userRepository;
            _transactionCodeRepository = transactionCodeRepository;
            _companyRepository = companyRepository;
            _paymentTokenService = paymentTokenService;
            _transactionService = transactionService;
            _appDbContext = appDbContext;
            _emailQueueRepository = emailQueueRepository;
            _emailSettings = emailSettings.Value;
        }

        public async Task CreateAsync(CreateTransactionDto createDto)
        {
            try
            {
                var contactPerson = await _employeeRepository.GetByIdAsync(createDto.BusinessPartnerId.Value);
                if (contactPerson == null)
                {
                    throw new Exception("Invalid contact person.");
                }

                if (contactPerson.GeneralStatus != GeneralStatus.Active)
                {
                    throw new Exception("The selected contact person is not active.");
                }

                var currency = await _currencyRepository.GetByCurrencyCodeAsync(createDto.CurrencyCode);

                if (currency == null) throw new Exception("Invalid currency");
                // Create deposit entity

                var user = await _userRepository.GetUserByRefreshTokenAsync();

                if (user == null) throw new Exception("User not found");

                await _transactionService.CompleteTransationProcess(createDto, null);

                //return await GetByIdAsync(locationPayment.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment");
                throw;
            }
        }

        public async Task<EmployeeDisbursementResponseDto> GetByIdAsync(Guid id)
        {
            try
            {
                var locationPayment = await _locationPaymentRepository.GetByIdAsync(id);
                if (locationPayment == null)
                    throw new Exception("Location payment not found");

                return MapToResponseDto(locationPayment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting location payment with ID {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeDisbursementResponseDto>> GetAllAsync(Guid locationId, Guid? employeeId, TransactionType? transactionType)
        {
            try
            {
                var locationPayments =  _locationPaymentRepository.GetAllAsync();
                locationPayments = locationPayments.Where(x => x.LocationId == locationId);

                if (employeeId.HasValue && employeeId != Guid.Empty)
                {
                    locationPayments = locationPayments.Where(x=> x.EmployeeId == employeeId);
                }

                if (transactionType.HasValue)
                {
                    locationPayments = locationPayments.Where(x => x.Transaction.TransactionType.ToString() == transactionType.ToString());
                }


                return locationPayments.Select(MapToResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all location payments");
                throw;
            }
        }

      

        public async Task<EmployeeDisbursementResponseDto> UpdateAsync(Guid id, UpdateEmployeeDisbursementDto updateDto)
        {
            try
            {
                var locationPayment = await _locationPaymentRepository.GetByIdAsync(id);
                if (locationPayment == null)
                    throw new Exception("Location payment not found");

                // Update using reflection since entity has private setters
                if (updateDto.ReceiverId.HasValue)
                {
                    var propertyInfo = locationPayment.GetType().GetProperty("ReceiverId");
                    propertyInfo?.SetValue(locationPayment, updateDto.ReceiverId.Value);
                }


                await _locationPaymentRepository.Update(locationPayment);
                await _locationPaymentRepository.SaveChangesAsync();

                return await GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating location payment with ID {Id}", id);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var locationPayment = await _locationPaymentRepository.GetByIdAsync(id);
                if (locationPayment == null)
                    throw new Exception("Location payment not found");

                _locationPaymentRepository.Delete(locationPayment);
                await _locationPaymentRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting location payment with ID {Id}", id);
                throw;
            }
        }

        private EmployeeDisbursementResponseDto MapToResponseDto(EmployeeDisbursement locationPayment)
        {
            return new EmployeeDisbursementResponseDto
            {
                Id = locationPayment.Id,
                LocationId = locationPayment.LocationId,
                LocationName = locationPayment.Location?.Name ?? string.Empty,
                EmployeeId = locationPayment.EmployeeId,
                EmployeeName = locationPayment.Employee != null
                    ? $"{locationPayment.Employee.FirstName} {locationPayment.Employee.LastName}"
                    : string.Empty,
                //PaymentType = locationPayment.Transaction.TransactionType.ToEnum(TransactionType),
                PaymentTypeName = locationPayment.Transaction.TransactionType.ToString()
            };
        }
    }
}