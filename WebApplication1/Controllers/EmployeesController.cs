using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using VMS.Modules.Licenses.Core.Emails.EmailSenderService.Entities;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;
using WebApplication1.Services.Emails.EmailService;
using WebApplication1.Services.Emails.EmailService.Entities;
using WebApplication1.Services.Emails.EmailService.Queuer;
using WebApplication1.Services.Emails.TemplateService;
using WebApplication1.Services.Emails.TemplateService.Enitities;
using WebApplication1.Services.PasswordService;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeesController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IEmployeeLocationRepository _employeeLocationRepository;
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IEmailQueueRepository _emailQueueRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ILocationManagementsRepository _locationManagementsRepository;
        private readonly EmailSettings _emailSettings;
        private readonly IPositionRepository _positionRepository;

        public EmployeesController(
            IEmployeeRepository employeeRepository,
            ILogger<EmployeesController> logger,
            IUserRepository userRepository,
            ILocationRepository locationRepository,
            IEmployeeLocationRepository employeeLocationRepository,
             AppDbContext appDbContext,
            UserManager<ApplicationUser> userManager,
            IPasswordGenerator passwordGenerator,
            IEmailTemplateService emailTemplateService,
             IEmailSenderService emailSenderService,
             ICompanyRepository companyRepository,
             ILocationManagementsRepository locationManagementsRepository,
             IOptions<EmailSettings> emailSettings,
             IPositionRepository positionRepository
            )
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
            _userRepository = userRepository;
            _locationRepository = locationRepository;
            _employeeLocationRepository = employeeLocationRepository;
            _appDbContext = appDbContext;
            _userManager = userManager;
            _passwordGenerator = passwordGenerator;
            _emailTemplateService = emailTemplateService;
            _companyRepository = companyRepository;
            _emailSenderService = emailSenderService;
            _locationManagementsRepository = locationManagementsRepository;
            _emailSettings = emailSettings.Value;
            _positionRepository = positionRepository;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetEmployeeDto>>> GetAllEmployees([FromQuery] Guid? locationId, [FromQuery] bool onlyActive = true)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();

                var employees =  _employeeRepository.GetAll();
                employees = employees.Where(x => x.CompanyId == user.CompanyId);

                if (onlyActive ) { employees = employees.Where(x => x.Status != EmployeeStatus.Terminated);  }
                if (locationId != Guid.Empty && locationId != null)
                {
                    employees = employees
                        .Include(X=> X.EmployeeLocations)
                        .Where(em => em.EmployeeLocations.Any(el => el.LocationId == locationId));
                }
              

                var returnData =   employees.Select(employee => new GetEmployeeDto
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Phone = employee.Phone,
                    Email = employee.Email,
                    Address = employee.Address,
                    IsAppUser = employee.IsAppUser,
                    Code = employee.Code,
                    RoleId = employee.PositionId,
                    PositionName = employee.Position.Title,
                    HireDate = employee.HireDate,
                    Salary = employee.Salary,
                    Status = employee.Status,
                    Locations = employee.EmployeeLocations.Where(x=> x.Status == true).Select(x => new DropDownDTO
                    {
                        Name = x.Location.Name,
                        Code = x.Location.Code,
                        Id = x.LocationId
                    }).ToList(),
                    StatusName = employee.Status.ToString()

                    //LocationId = employee.LocationId,
                    //LocationName = employee.Location.Name,
                });
                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all employees");
                return StatusCode(500, "An error occurred while retrieving employees");
            }
        }

        // GET: api/employees/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GetEmployeeDto>> GetEmployeeById(Guid id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);

                if (employee == null)
                    return NotFound($"Employee not found");

                var returnData = new GetEmployeeDto
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Phone = employee.Phone,
                    Email = employee.Email,
                    Address = employee.Address,
                    IsAppUser = employee.IsAppUser,
                    Code = employee.Code,
                    RoleId = employee.PositionId,
                    PositionName = employee.Position.Title,
                    HireDate = employee.HireDate,
                    Salary = employee.Salary,
                    Status = employee.Status,
                    //LocationId = employee.LocationId,
                    //LocationName = employee.Location.Name,
                    StatusName = employee.Status.ToString()
                };
                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the employee");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateEmployee([FromBody] CreateEmployeeDto createDto)
        {
            // Validate ModelState first
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                // Validate Status enum
                if (!Enum.IsDefined(typeof(EmployeeStatus), createDto.Status))
                    return BadRequest("Submitted Status is incorrect");

                if (createDto.IsAppUser && string.IsNullOrEmpty(createDto.Email)) { return BadRequest("Email is required app users"); }

                // Get current user
                var currentUser = await _userRepository.GetUserByRefreshTokenAsync();

                    if (currentUser == null) return Unauthorized();
                // Validate locations if provided
                List<Location> validLocations = new List<Location>();
                if (createDto.Locations?.Any() == true)
                {
                    validLocations = _locationRepository.ValidateLocations(createDto.Locations).ToList();
                    if (!validLocations.Any())
                        return NotFound(new { error = $"Invalid shops submitted" });
                }

               
                if (currentUser?.CompanyId == null)
                    return Unauthorized(new { message = "Unable to determine user company" });

                var validPosition = await _positionRepository.GetByIdAsync(createDto.PositionId);
                if (validPosition == null || validPosition.CompanyId != currentUser.CompanyId) return NotFound(new { error = $"Invalid position submitted" });
                // Check for existing user email if employee is app user
                if (createDto.IsAppUser)
                {
                    if (string.IsNullOrWhiteSpace(createDto.Email))
                        return BadRequest(new { message = "Email is required for app users" });

                    var existingUser = await _userManager.FindByEmailAsync(createDto.Email);
                    if (existingUser != null)
                        return Conflict(new { message = "Email already registered" });
                }

               

                // Generate employee code
                var employeeCode = await _employeeRepository.GenerateCodeAsync(createDto.Locations?.FirstOrDefault() ?? Guid.Empty);

                // Create employee
                var newEmployee = Employee.Create(
                    Guid.NewGuid(),
                    createDto.FirstName,
                    createDto.LastName,
                    createDto.Email ?? "",
                    employeeCode,
                    createDto.PositionId,
                    createDto.Phone,
                    DateTime.SpecifyKind(createDto.HireDate, DateTimeKind.Utc),
                    createDto.Salary,
                    EmployeeStatus.Active,
                    (Guid)currentUser.CompanyId,
                    createDto.Address,
                    createDto.IsAppUser,
                    DateTime.UtcNow,
                    Guid.Parse(currentUser.Id)
                );

                var employee = await _employeeRepository.CreateAsync(newEmployee);
                string autoGeneratedPassword = string.Empty;

                // Create app user if needed
                if (createDto.IsAppUser)
                {
                    var userAccountForEmployee = new ApplicationUser
                    {
                        Id = employee.Id.ToString(),
                        UserName = employee.Email,
                        Email = employee.Email,
                        FullName = $"{employee.FirstName} {employee.LastName}",
                        PhoneNumber = employee.Phone,
                        CreatedAt = DateTime.UtcNow,
                        Status = true ,
                        CompanyId = employee.CompanyId,
                    };

                    autoGeneratedPassword = _passwordGenerator.GeneratePassword(8, true, true, true, true, false); // Increased to 8 chars for security
                    var createUserResult = await _userManager.CreateAsync(userAccountForEmployee, autoGeneratedPassword);

                    if (!createUserResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        var errors = createUserResult.Errors.Select(e => e.Description);
                        return BadRequest(new { message = "User creation failed", errors });
                    }

                }

                // Add employee locations
                if (validLocations?.Any() == true)
                {
                    var employeeLocations = validLocations.Select(x =>
                        EmployeeLocation.Create(
                            Guid.NewGuid(),
                            x.Id,
                            employee.Id,
                            DateTime.UtcNow,
                            Guid.Parse(currentUser.Id),
                            true
                        )
                    ).ToList();

                    await _employeeLocationRepository.AddRangeAsync(employeeLocations);
                }

                if (createDto.IsAppUser && !string.IsNullOrWhiteSpace(autoGeneratedPassword))
                {

                    try
                    {
                        var company = await _companyRepository.GetByIdAsync(currentUser.CompanyId.Value);

                        var employeeAppAccessEmail = new AllEmailsTemplateModel
                        {
                            CompanyName = company.Name,
                            AppName = "EMS",
                            ReceiverName = $"{employee.FirstName} {employee.LastName}",
                            ReceiverRole = validPosition.Title,
                            ReceiverUserName = employee.Email,
                            TemporaryPassword = autoGeneratedPassword,
                            LoginUrl = _emailSettings.AppUrl,
                            MinPasswordLength = "8",
                            SupportName = company.Name,
                            SupportEmail = company.Email,
                            SupportPhone = company.PhoneNumber,
                            PinCode = employee.Code

                        };

                        var queuedEmail = new QueuedEmail
                        {
                            To = employee.Email,
                            Subject = $"App Access from {company.Name}",
                            TemplateName = "EmpolyeeAppAccess",
                            TemplateModelJson = JsonSerializer.Serialize(employeeAppAccessEmail),
                            TemplateModelType = typeof(AllEmailsTemplateModel).AssemblyQualifiedName,
                            EmployeeId = employee.Id,
                            CreatedAt = DateTime.Now,
                            CustomerId = Guid.Empty,
                            Status = EmailQueueStatus.Pending
                        };

                        await _appDbContext.QueuedEmails.AddAsync(queuedEmail);

                    }

                    catch (Exception ex)
                    {
                        // Log email error but don't fail the request
                        _logger.LogError(ex, "Failed to send welcome email to employee {EmployeeId}", employee.Id);
                    }
                }


                await _appDbContext.SaveChangesAsync();
                // Commit transaction
                await transaction.CommitAsync();

                // Send email only if employee is app user
                

                return CreatedAtAction(
                    nameof(GetEmployeeById),
                    new { id = employee.Id },
                    new
                    {
                        employee.Id,
                        employee.Code,
                        Message = createDto.IsAppUser ?
                            "Employee created successfully. Welcome email will be sent." :
                            "Employee created successfully."
                    }
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating employee for email: {Email}", createDto.Email);
                return StatusCode(500, new { message = "An error occurred while creating the employee", error = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateEmployee([FromBody] UpdateEmployeeDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!Enum.IsDefined(typeof(EmployeeStatus), createDto.Status)) return BadRequest("Submitted Status is incorrect");

                if (createDto.IsAppUser && string.IsNullOrEmpty(createDto.Email)) { return BadRequest("Email is required app users"); }
                if (createDto.Locations.Any())
                {
                    // Check if location exists
                    var queriableLocations = _locationRepository.ValidateLocations(createDto.Locations);

                    if (!queriableLocations.Any()) { return NotFound(new { error = $"Invalid shops submitted" }); }
                }

                var user = await _userRepository.GetUserByRefreshTokenAsync();

                var employee = await _employeeRepository.GetByIdAsync(createDto.Id);

                if (employee == null || employee.CompanyId != user?.CompanyId) return NotFound($"Employee not found");

               
                employee.Update(
                    createDto.FirstName, 
                    createDto.LastName,
                    createDto.Email ?? employee.Email,
                    createDto.PositionId, 
                    createDto.Phone,
                    DateTime.SpecifyKind(createDto.HireDate, DateTimeKind.Utc),
                    createDto.Salary, 
                    createDto.Status, 
                    (Guid) user.CompanyId , 
                    createDto?.Address??"", 
                    createDto.IsAppUser,
                    DateTime.UtcNow, 
                    Guid.Parse(user.Id));

                await _employeeLocationRepository.ManageLocationAccess(createDto.Locations, employee.Id, Guid.Parse(user.Id));

                await _employeeRepository.UpdateAsync(employee);

                if (!string.IsNullOrEmpty(createDto.Email) && createDto.Email.ToLower().Trim() != employee?.Email?.ToLower()?.Trim())
                {
                    user.Email = createDto.Email;
                    await   _userManager.UpdateAsync(user);
                }

              
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee");
                return StatusCode(500, "An error occurred while updating the employee");
            }
        }

        // DELETE: api/employees/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {

                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null) return Unauthorized();

                var deleted = await _employeeRepository.DeleteAsync(id);
                if (!deleted)   return NotFound($"Employee not found");

                var userAccount = await _userManager.FindByIdAsync(id.ToString());
                userAccount.Status = false;
                await _userManager.UpdateAsync(userAccount);

                await _appDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting employee ");
                return StatusCode(500, "An error occurred while deleting the employee");
            }
        }

    }
}
