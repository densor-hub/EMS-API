// Services/Implementations/EmployeeService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using VMS.Modules.Licenses.Core.Emails.EmailSenderService.Entities;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;
using WebApplication1.Services.Emails.EmailService;
using WebApplication1.Services.Emails.EmailService.Entities;
using WebApplication1.Services.Emails.TemplateService;
using WebApplication1.Services.Emails.TemplateService.Enitities;
using WebApplication1.Services.PasswordService;

namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IEmployeeLocationRepository _employeeLocationRepository;
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly ITransactionCodeRepository _transactionCodeRepository;
        private readonly EmailSettings _emailSettings;
        private readonly IPositionRepository _positionRepository;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            ILogger<EmployeeService> logger,
            IUserRepository userRepository,
            ILocationRepository locationRepository,
            IEmployeeLocationRepository employeeLocationRepository,
            AppDbContext appDbContext,
            UserManager<ApplicationUser> userManager,
            IPasswordGenerator passwordGenerator,
            ICompanyRepository companyRepository,
            IOptions<EmailSettings> emailSettings,
            IPositionRepository positionRepository,
            ITransactionCodeRepository transactionCodeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
            _userRepository = userRepository;
            _locationRepository = locationRepository;
            _employeeLocationRepository = employeeLocationRepository;
            _appDbContext = appDbContext;
            _userManager = userManager;
            _passwordGenerator = passwordGenerator;
            _companyRepository = companyRepository;
            _emailSettings = emailSettings.Value;
            _positionRepository = positionRepository;
            _transactionCodeRepository = transactionCodeRepository;
        }

        public async Task<IEnumerable<GetEmployeeDto>> GetAllEmployeesAsync(Guid? locationId, bool onlyActive)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync();

            var employees = _employeeRepository.GetAll();
            employees = employees.Where(x => x.CompanyId == user.CompanyId);

            if (onlyActive)
            {
                employees = employees.Where(x => x.Status != EmployeeStatus.Terminated);
            }

            if (locationId != Guid.Empty && locationId != null)
            {
                employees = employees
                    .Include(X => X.EmployeeLocations)
                    .Where(em => em.EmployeeLocations.Any(el => el.LocationId == locationId));
            }

            var returnData = employees.Select(employee => new GetEmployeeDto
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
                Locations = employee.EmployeeLocations.Where(x => x.Status == true).Select(x => new DropDownDTO
                {
                    Name = x.Location.Name,
                    Code = x.Location.Code,
                    Id = x.LocationId
                }).ToList(),
                StatusName = employee.Status.ToString()
            });

            return returnData;
        }

        public async Task<GetEmployeeDto> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                throw new Exception("Employee not found");

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
                StatusName = employee.Status.ToString()
            };

            return returnData;
        }

        public async Task<object> CreateEmployeeAsync(CreateEmployeeDto createDto, Guid createdAtLocation)
        {
            if (!Enum.IsDefined(typeof(EmployeeStatus), createDto.Status))
                throw new Exception("Submitted Status is incorrect");

            if (createDto.IsAppUser && string.IsNullOrEmpty(createDto.Email))
                throw new Exception("Email is required app users");

            var currentUser = await _userRepository.GetUserByRefreshTokenAsync();
            if (currentUser == null)
                throw new Exception("User not found");

            List<Location> validLocations = new List<Location>();
            if (createDto.Locations?.Any() == true)
            {
                validLocations = _locationRepository.ExistingLocations(createDto.Locations).ToList();
                if (!validLocations.Any())
                    throw new Exception("Invalid shops submitted");
            }

            if (currentUser?.CompanyId == null)
                throw new Exception("Unable to determine user company");

            var validPosition = await _positionRepository.GetByIdAsync(createDto.PositionId);
            if (validPosition == null || validPosition.CompanyId != currentUser.CompanyId)
                throw new Exception("Invalid position submitted");

            if (createDto.IsAppUser)
            {
                if (string.IsNullOrWhiteSpace(createDto.Email))
                    throw new Exception("Email is required for app users");

                var existingUser = await _userManager.FindByEmailAsync(createDto.Email);
                if (existingUser != null)
                    throw new Exception("Email already registered");
            }

            Guid createdEmployeeId;
            string createdEmployeeCode;

            // =========================================================
            // Phase 1: Create employee + user + locations in a transaction
            // =========================================================
            using (var transaction = await _appDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var empId = Guid.NewGuid();

                    var employee = Employee.Create(
                        empId,
                        createDto.FirstName,
                        createDto.LastName,
                        createDto.Email ?? "",
                        "",
                        createDto.PositionId,
                        createDto.Phone,
                        DateTime.SpecifyKind(createDto.HireDate, DateTimeKind.Utc),
                        createDto.Salary,
                        EmployeeStatus.Active,
                        (Guid)currentUser.CompanyId,
                        createDto.Address ?? "",
                        createDto.IsAppUser,
                        DateTime.UtcNow,
                        Guid.Parse(currentUser.Id),
                        empId.ToString()
                    );

                    await _employeeRepository.CreateAsync(employee);

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
                            Status = true,
                            CompanyId = employee.CompanyId,
                        };

                        var createUserResult = await _userManager.CreateAsync(userAccountForEmployee);

                        if (!createUserResult.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            var errors = createUserResult.Errors.Select(e => e.Description);
                            throw new Exception($"User creation failed: {string.Join(", ", errors)}");
                        }
                    }

                    if (validLocations.Any())
                    {
                        var employeeLocations = validLocations.Select(x =>
                            EmployeeLocation.Create(
                                Guid.NewGuid(),
                                x.Id,
                                employee.Id,
                                DateTime.UtcNow,
                                Guid.Parse(currentUser.Id),
                                true,
                                false
                            )
                        ).ToList();

                        await _employeeLocationRepository.AddRangeAsync(employeeLocations);
                    }

                    await _appDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    createdEmployeeId = employee.Id;
                    createdEmployeeCode = employee.Code;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error creating employee");
                    throw;
                }
            }

            // =========================================================
            // Phase 2: OUTSIDE the transaction — reload user, generate token, queue email
            // =========================================================
            if (createDto.IsAppUser)
            {
                try
                {
                    var company = await _companyRepository.GetByIdAsync((Guid)currentUser.CompanyId);

                    // Reload the user from the database so SecurityStamp is the committed one
                    var user = await _userManager.FindByEmailAsync(createDto.Email!);

                    if (user != null)
                    {
                        // Token generated against the committed user and stamp
                        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                        var encodedToken = Uri.EscapeDataString(resetToken);
                        var encodedEmail = Uri.EscapeDataString(user.Email!);

                        var resetUrl = $"{_emailSettings.AppUrl}/account/confirmation?token={encodedToken}&email={encodedEmail}";

                        var employeeAppAccessEmail = new AllEmailsTemplateModel
                        {
                            CompanyName = company.Name,
                            AppName = "EMS",
                            ReceiverName = $"{createDto.FirstName} {createDto.LastName}",
                            ReceiverRole = validPosition.Title,
                            ReceiverUserName = user.Email!,
                            AppUrl = resetUrl,
                            SupportName = company.Name,
                            SupportEmail = company.Email,
                            SupportPhone = company.PhoneNumber,
                            PinCode = createdEmployeeCode
                        };

                        var queuedEmail = new QueuedEmail
                        {
                            To = user.Email!,
                            Subject = $"Welcome to {company.Name} - Set Your Password",
                            TemplateName = "EmployeeSetPassword",
                            TemplateModelJson = JsonSerializer.Serialize(employeeAppAccessEmail),
                            TemplateModelType = typeof(AllEmailsTemplateModel).AssemblyQualifiedName,
                            ReceiverId = createdEmployeeId,
                            CreatedAt = DateTime.UtcNow,
                            Status = EmailQueueStatus.Pending
                        };

                        await _appDbContext.QueuedEmails.AddAsync(queuedEmail);
                        await _appDbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    // Employee was created successfully; email failed. Log but don't roll back.
                    _logger.LogError(ex, "Employee created but invite email could not be queued for {Email}", createDto.Email);
                }
            }

            return new EmployeeResponseDto
            {
                Id = createdEmployeeId,
                Code = createdEmployeeCode,
                Message = createDto.IsAppUser
                    ? "Employee created successfully. A password setup link has been sent to their email."
                    : "Employee created successfully."
            };
        }

        //public async Task<object> CreateEmployeeAsync(CreateEmployeeDto createDto, Guid createdAtLocation)
        //{
        //    if (!Enum.IsDefined(typeof(EmployeeStatus), createDto.Status))
        //        throw new Exception("Submitted Status is incorrect");

        //    if (createDto.IsAppUser && string.IsNullOrEmpty(createDto.Email))
        //        throw new Exception("Email is required app users");

        //    var currentUser = await _userRepository.GetUserByRefreshTokenAsync();
        //    if (currentUser == null)
        //        throw new Exception("User not found");

        //    List<Location> validLocations = new List<Location>();
        //    if (createDto.Locations?.Any() == true)
        //    {
        //        validLocations = _locationRepository.ExistingLocations(createDto.Locations).ToList();
        //        if (!validLocations.Any())
        //            throw new Exception("Invalid shops submitted");
        //    }

        //    if (currentUser?.CompanyId == null)
        //        throw new Exception("Unable to determine user company");

        //    var validPosition = await _positionRepository.GetByIdAsync(createDto.PositionId);
        //    if (validPosition == null || validPosition.CompanyId != currentUser.CompanyId)
        //        throw new Exception("Invalid position submitted");

        //    if (createDto.IsAppUser)
        //    {
        //        if (string.IsNullOrWhiteSpace(createDto.Email))
        //            throw new Exception("Email is required for app users");

        //        var existingUser = await _userManager.FindByEmailAsync(createDto.Email);
        //        if (existingUser != null)
        //            throw new Exception("Email already registered");
        //    }


        //    using var transaction = await _appDbContext.Database.BeginTransactionAsync();

        //    try
        //    {

        //        // 4. CHECK - Existing user
        //        if (createDto.IsAppUser)
        //        {
        //            if (string.IsNullOrWhiteSpace(createDto.Email))
        //                throw new Exception("Email is required for app users");

        //            var existingUser = await _userManager.FindByEmailAsync(createDto.Email);
        //            if (existingUser != null)
        //                throw new Exception("Email already registered");
        //        }

        //        // 5. CREATE - Employee
        //        var empId = Guid.NewGuid();


        //        var employee = Employee.Create(
        //           empId,
        //            createDto.FirstName,
        //            createDto.LastName,
        //            createDto.Email ?? "",
        //            "",
        //            createDto.PositionId,
        //            createDto.Phone,
        //            DateTime.SpecifyKind(createDto.HireDate, DateTimeKind.Utc),
        //            createDto.Salary,
        //            EmployeeStatus.Active,
        //            (Guid)currentUser.CompanyId,
        //            createDto.Address ?? "",
        //            createDto.IsAppUser,
        //            DateTime.UtcNow,
        //            Guid.Parse(currentUser.Id),
        //            empId.ToString()
        //        );


        //        await _employeeRepository.CreateAsync(employee);



        //        // 6. CREATE - App User if needed (without password)
        //        if (createDto.IsAppUser)
        //        {
        //            var userAccountForEmployee = new ApplicationUser
        //            {
        //                Id = employee.Id.ToString(),
        //                UserName = employee.Email,
        //                Email = employee.Email,
        //                FullName = $"{employee.FirstName} {employee.LastName}",
        //                PhoneNumber = employee.Phone,
        //                CreatedAt = DateTime.UtcNow,
        //                Status = true,
        //                CompanyId = employee.CompanyId,
        //            };

        //            // Create user without password - will use password reset flow
        //            var createUserResult = await _userManager.CreateAsync(userAccountForEmployee);

        //            if (!createUserResult.Succeeded)
        //            {
        //                await transaction.RollbackAsync();
        //                var errors = createUserResult.Errors.Select(e => e.Description);
        //                throw new Exception($"User creation failed: {string.Join(", ", errors)}");
        //            }
        //        }

        //        // 7. CREATE - Employee Locations
        //        if (validLocations.Any())
        //        {
        //            var employeeLocations = validLocations.Select(x =>
        //                EmployeeLocation.Create(
        //                    Guid.NewGuid(),
        //                    x.Id,
        //                    employee.Id,
        //                    DateTime.UtcNow,
        //                    Guid.Parse(currentUser.Id),
        //                    true,
        //                    false
        //                )
        //            ).ToList();

        //            await _employeeLocationRepository.AddRangeAsync(employeeLocations);
        //        }

        //        // 8. SEND - Password Reset Email
        //        if (createDto.IsAppUser)
        //        {
        //            var company = await _companyRepository.GetByIdAsync(currentUser.CompanyId);
        //            var user = await _userManager.FindByEmailAsync(employee.Email);

        //            if (user != null)
        //            {
        //                // Generate password reset token
        //                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        //                // URL encode the token
        //                var encodedToken = Uri.EscapeDataString(resetToken);
        //                var encodedEmail = Uri.EscapeDataString(employee.Email);

        //                // Build reset password URL
        //                var resetUrl = $"{_emailSettings.AppUrl}/account/confirmation?token={encodedToken}&email={encodedEmail}";

        //                // You can also use the frontend route
        //                // var resetUrl = $"{_emailSettings.FrontendUrl}/auth/reset-password?token={encodedToken}&email={encodedEmail}";

        //                var employeeAppAccessEmail = new AllEmailsTemplateModel
        //                {
        //                    CompanyName = company.Name,
        //                    AppName = "EMS",
        //                    ReceiverName = $"{employee.FirstName} {employee.LastName}",
        //                    ReceiverRole = validPosition.Title,
        //                    ReceiverUserName = employee.Email,
        //                    AppUrl = resetUrl,
        //                    SupportName = company.Name,
        //                    SupportEmail = company.Email,
        //                    SupportPhone = company.PhoneNumber,
        //                    PinCode = employee.Code
        //                };

        //                var queuedEmail = new QueuedEmail
        //                {
        //                    To = employee.Email,
        //                    Subject = $"Welcome to {company.Name} - Set Your Password",
        //                    TemplateName = "EmployeeSetPassword", // New template name
        //                    TemplateModelJson = JsonSerializer.Serialize(employeeAppAccessEmail),
        //                    TemplateModelType = typeof(AllEmailsTemplateModel).AssemblyQualifiedName,
        //                    ReceiverId = employee.Id,
        //                    CreatedAt = DateTime.UtcNow,
        //                    Status = EmailQueueStatus.Pending
        //                };

        //                await _appDbContext.QueuedEmails.AddAsync(queuedEmail);
        //            }
        //        }

        //        // 9. SAVE - All changes
        //        await _appDbContext.SaveChangesAsync();
        //        await transaction.CommitAsync();

        //        return new EmployeeResponseDto
        //        {
        //            Id = employee.Id,
        //            Code = employee.Code,
        //            Message = createDto.IsAppUser ?
        //                "Employee created successfully. A password setup link has been sent to their email." :
        //                "Employee created successfully."
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        _logger.LogError(ex, "Error creating employee");
        //        throw;
        //    }
        //}

        public async Task UpdateEmployeeAsync(UpdateEmployeeDto updateDto)
        {
            //if (!ModelState.IsValid)
            //    throw new Exception("Invalid model state");

            var transaction = await _appDbContext.Database.BeginTransactionAsync();


            if (!Enum.IsDefined(typeof(EmployeeStatus), updateDto.Status))
                throw new Exception("Submitted Status is incorrect");

            if (updateDto.IsAppUser && string.IsNullOrEmpty(updateDto.Email))
                throw new Exception("Email is required app users");

            var submittedLocations = updateDto.Locations.ToList();
            if (updateDto.Locations.Any())
            {
                var queriableLocations = _locationRepository.ExistingLocations(submittedLocations);
                if (!queriableLocations.Any())
                    throw new Exception("Invalid shops submitted");
            }

            var user = await _userRepository.GetUserByRefreshTokenAsync();
            if (user == null)
                throw new Exception("User not found");

            var employee = await _employeeRepository.GetByIdAsync(updateDto.Id);
            if (employee == null || employee.CompanyId != user?.CompanyId)
                throw new Exception("Employee not found");


            try
            {
                employee.Update(
                               updateDto.FirstName,
                               updateDto.LastName,
                               updateDto.Email ?? employee.Email ??"",
                               updateDto.PositionId,
                               updateDto.Phone,
                               DateTime.SpecifyKind(updateDto.HireDate, DateTimeKind.Utc),
                               updateDto.Salary,
                               updateDto.Status,
                               (Guid)user.CompanyId,
                               updateDto?.Address ?? "",
                               updateDto?.IsAppUser?? employee.IsAppUser,
                               DateTime.UtcNow,
                               Guid.Parse(user.Id));

                await _employeeLocationRepository.ManageLocationAccess(updateDto.Locations, employee.Id, Guid.Parse(user.Id));
                await _employeeRepository.UpdateAsync(employee);

                if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email.ToLower().Trim() != employee?.Email?.ToLower()?.Trim())
                {
                    user.Email = updateDto.Email;
                    await _userManager.UpdateAsync(user);
                }
                await   _appDbContext.SaveChangesAsync();

                 await transaction.CommitAsync();
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
           

        }

        public async Task DeleteEmployeeAsync(Guid id)
        {
            using var transaction = await _appDbContext.Database.BeginTransactionAsync();

            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null)
                    throw new Exception("User not found");

                var employee = await _employeeRepository.DeleteAsync(id);
                if (!employee)
                    throw new Exception("Employee not found");

                var userAccount = await _userManager.FindByIdAsync(id.ToString());
                if (userAccount != null)
                {
                    userAccount.Status = false;
                    await _userManager.UpdateAsync(userAccount);
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}