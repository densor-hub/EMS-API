using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;
using WebApplication1.Services.Emails.EmailService;
using WebApplication1.Services.Emails.EmailService.Entities;
using WebApplication1.Services.Emails.TemplateService;
using WebApplication1.Services.Emails.TemplateService.Enitities;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomersController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ICompanyRepository _companyRepository;
        private readonly ILocationManagementsRepository _locationManagementsRepository;
        private readonly ILocationRepository _locationRepository;
        public CustomersController(
            ICustomerRepository customerRepository,
            ILogger<CustomersController> logger,
            IUserRepository userRepository,
             IEmailTemplateService emailTemplateService,
             IEmailSenderService emailSenderService,
             ICompanyRepository companyRepository,
              ILocationManagementsRepository locationManagementsRepository,
              ILocationRepository locationRepository
            )
        {
            _customerRepository = customerRepository;
            _logger = logger;
            _userRepository = userRepository;
            _emailTemplateService = emailTemplateService;
            _companyRepository = companyRepository;
            _emailSenderService = emailSenderService;
            _locationManagementsRepository = locationManagementsRepository;
            _locationRepository = locationRepository;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetEmployeeDto>>> GetAllEmployees([FromQuery] Guid locationId)
        {
            try
            {
                var customers =  _customerRepository.GetAllAsync(locationId);

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
                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customer");
                return StatusCode(500, "An error occurred while retrieving customer");
            }
        }

        // GET: api/employees/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GetCustomerDto>> GetEmployeeById(Guid id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);

                if (customer == null)
                    return NotFound($"Customer not found");

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
                   NationalId =  customer.SecondaryPhone
                };
                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer");
                return StatusCode(500, "An error occurred while retrieving the customer");
            }
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult> CreateEmployee([FromBody] CreateCustomerDTO createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var Code = await  _customerRepository.GenerateCodeAsync(createDto.LocationId);

                var currentUser = await _userRepository.GetUserByRefreshTokenAsync();
                var location = await _locationRepository.GetByIdAsync(createDto.LocationId);

                if (location is null) return BadRequest("Shop not found");
                var newCustomer = Customer.Create(Guid.NewGuid(), createDto.FirstName, createDto.LastName, createDto.Email ?? "", createDto.Phone, Code,
                    createDto.Status, createDto.LocationId, createDto.Address, createDto?.CreditLimit??0, DateTime.UtcNow, Guid.Parse(currentUser.Id), "", createDto?.NationalIdentificationNumber??"");

                var customer = await _customerRepository.CreateAsync(newCustomer);

                var managers = _locationManagementsRepository.GetAllByLocationId(newCustomer.LocationId);
                var activeManager = managers.Where(x=> x.Status == true).FirstOrDefault();

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
                            ManagerName = $"{activeManager?.Manager?.FirstName ?? ""} {activeManager?.Manager?.LastName ?? ""}",
                            ManagerPhone = activeManager?.Manager?.Phone ?? "",
                            ManagerTitle = activeManager?.Manager?.Position?.Title ?? "",
                            ManagerEmail = activeManager?.Manager?.Email ?? "",
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
                        // Log email error but don't fail the request
                        _logger.LogError(ex, "Failed to send welcome email to cusromer {CustomerId}", customer.Id);
                    }
                }

                return StatusCode(201, new {Id = newCustomer.Id});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return StatusCode(500, "An error occurred while creating the customer");
            }
        }

        // PUT: api/employees/{id}
        [HttpPut]
        public async Task<ActionResult> UpdateEmployee([FromBody] UpdateCustomerDTO createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var customer = await _customerRepository.GetByIdAsync(createDto.Id) ;
                if (customer == null) return NotFound($"Customer not found");

                var currentUserId = await _userRepository.GetCurrentUserId();

                customer.Update(createDto?.FirstName??"", createDto?.LastName??"", createDto?.Email ?? "", createDto?.Phone??"",
                 createDto?.CreditLimit??0, createDto?.Status??customer.Status, customer.LocationId, createDto?.Address ?? "",
                 DateTime.UtcNow, currentUserId, "", createDto?.NationalIdentificationNumber ?? "");

                await _customerRepository.UpdateAsync(customer);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer");
                return StatusCode(500, "An error occurred while updating the customer");
            }
        }

        // DELETE: api/employees/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                var deleted = await _customerRepository.DeleteAsync(id);
                if (!deleted)
                    return NotFound($"Customer = not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer");
                return StatusCode(500, "An error occurred while deleting the customer");
            }
        }

    }
}
