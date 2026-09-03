// Services/Implementations/CustomerService.cs
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;
using WebApplication1.Services.Emails.EmailService;
using WebApplication1.Services.Emails.EmailService.Entities;
using WebApplication1.Services.Emails.TemplateService;
using WebApplication1.Services.Emails.TemplateService.Enitities;

namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmployeeLocationRepository _employeeLocRepositoty;
        private readonly ILocationRepository _locationRepository;
        private readonly ITransactionCodeRepository _transactionCodeRepository;

        public CustomerService(
            ICustomerRepository customerRepository,
            ILogger<CustomerService> logger,
            IUserRepository userRepository,
            IEmailTemplateService emailTemplateService,
            IEmailSenderService emailSenderService,
            ICompanyRepository companyRepository,
             IEmployeeLocationRepository employeelocRepositoty,
            ILocationRepository locationRepository,
            ITransactionCodeRepository transactionCodeRepository)
        {
            _customerRepository = customerRepository;
            _logger = logger;
            _userRepository = userRepository;
            _emailTemplateService = emailTemplateService;
            _emailSenderService = emailSenderService;
            _companyRepository = companyRepository;
            _employeeLocRepositoty = employeelocRepositoty;
            _locationRepository = locationRepository;
            _transactionCodeRepository = transactionCodeRepository;
        }

        public async Task<IEnumerable<GetCustomerDto>> GetAllCustomersAsync(Guid locationId)
        {
            try
            {
                var customers = _customerRepository.GetAllAsync(locationId);

                var returnData = customers.Select(customer => new GetCustomerDto
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Phone = customer.Phone,
                    Email = customer.Email,
                    Address = customer.Address,
                    Code = customer.Code,
                    CreditLimit = customer.CreditLimit ?? 0,
                    Status = customer.Status,
                    LocationId = customer.LocationId,
                    LocationName = customer.Location.Name,
                    StatusName = customer.Status ? "Active" : "Inactive",
                    NationalId = customer.SecondaryPhone
                });

                return returnData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customers");
                throw;
            }
        }

        public async Task<GetCustomerDto> GetCustomerByIdAsync(Guid id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);

                if (customer == null)
                    throw new Exception("Customer not found");

                var returnData = new GetCustomerDto
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Phone = customer.Phone,
                    Email = customer.Email,
                    Address = customer.Address,
                    Code = customer.Code,
                    CreditLimit = customer.CreditLimit ?? 0,
                    Status = customer.Status,
                    LocationId = customer.LocationId,
                    LocationName = customer.Location.Name,
                    StatusName = customer.Status ? "Active" : "Inactive",
                    NationalId = customer.SecondaryPhone
                };

                return returnData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer with ID {Id}", id);
                throw;
            }
        }

        public async Task<object> CreateCustomerAsync(CreateCustomerDTO createDto)
        {
            try
            {
                //if (!ModelState.IsValid)
                //    throw new Exception("Invalid model state");

                //var Code = await _transactionCodeRepository.GenerateEntityCodeAsync("CUS", createDto.LocationId);

                var currentUser = await _userRepository.GetUserByRefreshTokenAsync();
                if (currentUser == null)
                    throw new Exception("User not found");

                var location = await _locationRepository.GetByIdAsync(createDto.LocationId);
                if (location is null)
                    throw new Exception("Shop not found");

                var newCustomer = Customer.Create(
                    Guid.NewGuid(),
                    createDto.FirstName,
                    createDto.LastName,
                    createDto.Email ?? "",
                    createDto.Phone,
                    "",
                    createDto.Status,
                   location.Id,
                    createDto.Address,
                    createDto?.CreditLimit ?? 0,
                    DateTime.UtcNow,
                    Guid.Parse(currentUser.Id),
                    "",
                    createDto?.NationalIdentificationNumber ?? "");

                var customer = await _customerRepository.CreateAsync(newCustomer);

                var employeeLocations = _employeeLocRepositoty.GetAllByLocationId(location.Id) ;

                var activeManager = await employeeLocations.Where(x => x.IsManager && x.Status).FirstOrDefaultAsync();


                if (!string.IsNullOrEmpty(newCustomer.Email))
                {
                    try
                    {
                        var company = await _companyRepository.GetByIdAsync(currentUser?.CompanyId);

                        var BusinessPartnerEmail = new AllEmailsTemplateModel
                        {
                            CompanyName = company.Name,
                            AppName = "EMS",
                            ReceiverName = $"{newCustomer.FirstName} {newCustomer.LastName}",
                            ReceiverType = "Customer",
                            PrimaryPhoneNumber = customer.Phone,
                            SecondaryPhoneNumber = customer.SecondaryPhone,
                            ReceiverEmail = customer?.Email ?? "",
                            PrimaryEmail = customer?.Email ?? "",
                            SecondaryEmail = customer?.SecondaryEmail ?? "",
                            Address = customer.Address,
                            ManagerName = $"{activeManager?.Employee?.FirstName ?? ""} {activeManager?.Employee?.LastName ?? ""}",
                            ManagerPhone = activeManager?.Employee?.Phone ?? "",
                            ManagerTitle = activeManager?.Employee?.Position?.Title ?? "",
                            ManagerEmail = activeManager?.Employee?.Email ?? "",
                            SupportName = company.Name,
                            SupportEmail = company.Email,
                            SupportPhone = company.PhoneNumber,
                            PinCode = customer.Code
                        };

                        var template = await _emailTemplateService.RenderEmailTemplateAsync(BusinessPartnerEmail, "BusinessPartnerAdded");
                        var message = new EmailMessage
                        {
                            Subject = $"{company.Name} added you as a customer at {location.Name}",
                            IsHtml = true,
                            Body = template,
                            To = newCustomer.Email
                        };

                        _ = Task.Run(() => _emailSenderService.SendEmailAsync(message));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send welcome email to customer {CustomerId}", customer.Id);
                    }
                }

                return new { Id = newCustomer.Id };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                throw;
            }
        }

        public async Task UpdateCustomerAsync(UpdateCustomerDTO updateDto)
        {
            try
            {
                //if (!ModelState.IsValid)
                //    throw new Exception("Invalid model state");

                var customer = await _customerRepository.GetByIdAsync(updateDto.Id);
                if (customer == null)
                    throw new Exception("Customer not found");

                var currentUserId = await _userRepository.GetCurrentUserId();

                customer.Update(
                    updateDto?.FirstName ?? customer?.FirstName ??"",
                    updateDto?.LastName ?? customer?.LastName ?? "",
                    updateDto?.Email ?? customer?.Email ?? "",
                    updateDto?.Phone ?? customer?.Phone ?? "",
                    updateDto?.CreditLimit ?? customer?.CreditLimit ??  0,
                    updateDto?.Status ?? customer.Status,
                    customer.LocationId,
                    updateDto?.Address ?? "",
                    DateTime.UtcNow,
                    currentUserId,
                    "",
                    updateDto?.NationalIdentificationNumber ?? "");

                await _customerRepository.UpdateAsync(customer);

                await _customerRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer");
                throw;
            }
        }

        public async Task DeleteCustomerAsync(Guid id)
        {
            try
            {
                var deleted = await _customerRepository.DeleteAsync(id);

                 await _customerRepository.SaveChangesAsync();
                if (!deleted)
                    throw new Exception("Customer not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer");
                throw;
            }
        }
    }
}