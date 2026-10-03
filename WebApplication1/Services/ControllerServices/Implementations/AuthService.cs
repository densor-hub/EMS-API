// Services/Implementations/AuthService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Helpers;
using WebApplication1.Services.Emails.EmailService.Entities;
using WebApplication1.Services.TokenService;

namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly EmailSettings _emailSettings;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IConfiguration configuration,
            ILogger<AuthService> logger,
             IOptions<EmailSettings> emailSettings,
        AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _logger = logger;
            _context = context;
            _emailSettings = emailSettings.Value;
            
        }

        public async Task RegisterAsync(RegisterDTO request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var companyExist = await _context.Companies
                    .Where(x => (x.Email.ToLower().Trim() == request.Company.Email.ToLower().Trim())
                                || x.PhoneNumber.ToLower().Trim() == request.Company.Phone.ToLower().Trim())
                    .FirstOrDefaultAsync();

                if (companyExist != null)
                    throw new Exception("Company already registered");

                var existingUser = await _userManager.FindByEmailAsync(request.AdminInfo.Email.Trim());
                if (existingUser != null)
                    throw new Exception("Email already registered");

                var company = Company.Create(
                   Guid.NewGuid(),
                   request.Company.Name,
                   request.Company.Phone,
                   request.Company.Email,
                   true,
                   DateTime.UtcNow,
                   request.Company?.TIN ?? "",
                   request.Company?.RegistrationNumber ?? "",
                   request?.Company?.Address ?? "",
                   request?.Company?.Website ?? "",
                   Guid.Empty
               );

                var user = new ApplicationUser
                {
                    UserName = request.AdminInfo.Email,
                    Email = request.AdminInfo.Email,
                    FullName = request.AdminInfo.FullName,
                    PhoneNumber = request.AdminInfo.PhoneNumber,
                    CreatedAt = DateTime.UtcNow,
                    Status = true
                };

                var createUserResult = await _userManager.CreateAsync(user, request.AdminInfo.Password);

                if (!createUserResult.Succeeded)
                {
                    var errors = createUserResult.Errors.Select(e => e.Description);
                    throw new Exception($"User creation failed: {string.Join(", ", errors)}");
                }

                company.UpdateCreatedBy(Guid.Parse(user.Id));
               

                var adminPosition = await _context.Positions
                    .Where(x => x.Id == Guid.Parse("00000000-0000-0000-0000-000000000001"))
                    .FirstOrDefaultAsync();

                var initials = PropertyTypeExtensions.GetInitials(company.Name);
                var id = Guid.Parse(user.Id);
                var defaultEmployeeAccount = Employee.Create(
                    id,
                    user.FullName,
                    "",
                    user.Email,
                    $"{initials}-ADMIN",
                    adminPosition.Id,
                    user?.PhoneNumber ?? "",
                    DateTime.UtcNow,
                    0,
                    Domain.Enums.EmployeeStatus.Active,
                    company.Id,
                    "",
                    true,
                    DateTime.UtcNow,
                    id,
                    id.ToString()
                    );

                var positionRoutes = await _context.PositionRoutes
                    .Where(x => x.PositionId == adminPosition.Id)
                    .AsNoTracking()
                    .ToListAsync();

                var userRoutes = new List<UserRoutes>();

                foreach (var positionRoute in positionRoutes)
                {
                    var userRoute = UserRoutes.Create(
                        Guid.NewGuid(),
                        positionRoute.Id,
                        user.Id,
                        DateTime.UtcNow,
                        Guid.Empty
                    );
                    userRoutes.Add(userRoute);
                }

                await _context.Companies.AddAsync(company);
                user.CompanyId = company.Id;
                await _userManager.UpdateAsync(user);

                await _context.Employees.AddAsync(defaultEmployeeAccount);
                await _context.UserRoutes.AddRangeAsync(userRoutes);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("User registered: {Email}", user.Email);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null || user?.Status == false)
                    throw new Exception("Invalid email or password");

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

                if (!result.Succeeded)
                {
                    if (result.IsLockedOut)
                        throw new Exception("Account is locked out. Please try again later.");

                    throw new Exception("Invalid email or password");
                }

                var company = await _context.Companies
                    .Where(x => x.Id == user.CompanyId)
                    .Select(x => new { x.Id, x.Name })
                    .FirstOrDefaultAsync();

                var employee = await _context.Employees
                    .Include(x => x.EmployeeLocations)
                        .ThenInclude(x => x.Location)
                    .Include(x => x.Position)
                        .ThenInclude(x => x.PositionRoutes)
                            .ThenInclude(x => x.ApplicationRoutes)
                    .Where(x => x.Id == Guid.Parse(user.Id))
                    .FirstOrDefaultAsync();

                var allLocationsDTO = new List<DropDownDTO>();

                if (employee?.Id == employee?.CreatedBy)
                {
                    var locations = _context.Locations.Where(x => x.Status == true && x.CompanyId == user.CompanyId);
                    allLocationsDTO = await locations.Select(x => new DropDownDTO
                    {
                        Code = x.Code,
                        Id = x.Id,
                        Name = x.Name
                    }).ToListAsync();
                }

                user.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                var tokens = await _tokenService.GenerateAuthResponseAsync(user);

                var authResponse = new AuthResponseDTO
                {
                    Company = new IdAndNameDTO
                    {
                        Id = company == null ? Guid.Empty : company.Id,
                        Name = company == null ? string.Empty : company.Name
                    },
                    Email = user.Email ?? "",
                    FullName = user.FullName,
                    Id = user.Id,
                    Locations = allLocationsDTO.Count > 0 ? allLocationsDTO : employee?.EmployeeLocations != null ? employee.EmployeeLocations.Select(x => new DropDownDTO
                    {
                        Code = x.Location.Code,
                        Id = x.Location.Id,
                        Name = x.Location.Name
                    }).ToList() : null,
                    Routes = employee?.Position?.PositionRoutes != null ? employee.Position.PositionRoutes.Select(x => new DropDownDTO
                    {
                        Code = "",
                        Id = x.ApplicationRoutes.Id,
                        Name = x.ApplicationRoutes.Title
                    }).ToList() : null,
                    IsCreator = user.Id == employee?.CreatedBy.ToString(),
                    RefreshToken = tokens.RefreshToken,
                    RefreshTokenExpires = tokens.RefreshTokenExpires
                };

                _logger.LogInformation("User logged in: {Email}", user.Email);
                return authResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for {Email}", request.Email);
                throw;
            }
        }

        public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new Exception("Refresh token not found");

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.Status == false)
                throw new Exception("Invalid refresh token or user inactive");

            if (!await _tokenService.ValidateRefreshTokenAsync(user, refreshToken))
                throw new Exception("Refresh token expired");

            var tokens = await _tokenService.GenerateAuthResponseAsync(user);

            var company = await _context.Companies
                .Include(x=> x.Locations)
                .Where(x => x.Id == user.CompanyId)
                .Select(x => new { x.Id, x.Name, x.AdminId })
                .FirstOrDefaultAsync();

            var employee = await _context.Employees
                .Include(x=> x.Company)
                    .ThenInclude(x=> x.Locations)
                .Include(x => x.EmployeeLocations)
                    .ThenInclude(x => x.Location)
                .Include(x => x.Position)
                    .ThenInclude(x => x.PositionRoutes)
                        .ThenInclude(x => x.ApplicationRoutes)
                .Where(x => x.Id == Guid.Parse(user.Id))
                .FirstOrDefaultAsync();

            var returnData = new AuthResponseDTO
            {
                
                Company = new IdAndNameDTO
                {
                    Id = employee.Company == null ? Guid.Empty : company.Id,
                    Name = employee.Company == null ? string.Empty : company.Name
                },
                Email = user.Email ?? "",
                FullName = user.FullName,
                Id = user.Id,
                Locations = employee.Id == company.AdminId ? employee.Company.Locations.Select(x => new DropDownDTO
                {
                    Code = x.Code,
                    Id = x.Id,
                    Name = x.Name
                }).ToList() : employee?.EmployeeLocations != null ? employee.EmployeeLocations.Select(x => new DropDownDTO
                        {
                            Code = x.Location.Code,
                            Id = x.Location.Id,
                            Name = x.Location.Name
                        }).ToList() : null,
                Routes = employee?.Position?.PositionRoutes != null ? employee.Position.PositionRoutes.Select(x => new DropDownDTO
                {
                    Code = "",
                    Id = x.ApplicationRoutes.Id,
                    Name = x.ApplicationRoutes.Title
                }).ToList() : null,
                IsCreator = user.Id == employee?.CreatedBy.ToString(),
                RefreshToken = tokens.RefreshToken,
                RefreshTokenExpires = tokens.RefreshTokenExpires
            };

            return returnData;
        }

        public async Task LogoutAsync(string userId, string accessToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        await _tokenService.RevokeRefreshTokenAsync(user);

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            await _tokenService.RevokeAccessTokenAsync(accessToken);
                        }

                        await _userManager.UpdateSecurityStampAsync(user);
                        _logger.LogInformation("User logged out: {UserId}", userId);
                    }
                }

                await _signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                throw;
            }
        }

        public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                    throw new Exception("User not found");

                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    throw new Exception($"Password change failed: {string.Join(", ", errors)}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password change failed");
                throw;
            }
        }

        public async Task<object> GetProfileAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                    throw new Exception("User not found");

                var employee = await _context.Employees
                    .Include(x => x.EmployeeLocations)
                        .ThenInclude(x => x.Location)
                    .Include(x => x.Position)
                        .ThenInclude(x => x.PositionRoutes)
                            .ThenInclude(x => x.ApplicationRoutes)
                    .Where(x => x.Id == Guid.Parse(user.Id))
                    .FirstOrDefaultAsync();

                var profile = new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    user.PhoneNumber,
                    user.EmailConfirmed,
                    user.TwoFactorEnabled,
                    user.CreatedAt,
                    user.Status,
                    Position = employee?.Position != null ? new DropDownDTO
                    {
                        Code = "",
                        Id = employee.Position.Id,
                        Name = employee.Position.Title,
                    } : null,
                    Locations = employee?.EmployeeLocations != null ? employee.EmployeeLocations.Select(x => new DropDownDTO
                    {
                        Code = x.Location.Code,
                        Id = x.Location.Id,
                        Name = x.Location.Name
                    }).ToList() : null,
                    Routes = employee?.Position?.PositionRoutes != null ? employee.Position.PositionRoutes.Select(x => new DropDownDTO
                    {
                        Code = "",
                        Id = x.ApplicationRoutes.Id,
                        Name = x.ApplicationRoutes.Title
                    }).ToList() : null
                };

                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get profile failed");
                throw;
            }
        }

        public async Task DeleteUserAsync(string userId, string currentUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (currentUserId == userId)
                    throw new Exception("You cannot delete your own account. Contact another administrator.");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new Exception("User not found");

                _logger.LogInformation("Starting deletion process for user: {UserId}, {Email}", userId, user.Email);

                var logins = await _userManager.GetLoginsAsync(user);
                foreach (var login in logins)
                {
                    await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                }

                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, roles);
                }

                var claims = await _userManager.GetClaimsAsync(user);
                foreach (var claim in claims)
                {
                    await _userManager.RemoveClaimAsync(user, claim);
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to delete user: {errors}");
                }

                await transaction.CommitAsync();

                await _tokenService.RevokeRefreshTokenAsync(user);

                _logger.LogInformation("User successfully deleted: {UserId}, {Email}", userId, user.Email);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
       

        public async Task<bool> ConfirmAccount(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogError("ConfirmAccount: user not found for {Email}", email);
                throw new Exception("Invalid or expired token. Please request a new password reset.");
            }

            var resetProvider = _userManager.Options.Tokens.PasswordResetTokenProvider;
           
            var validWithReset = await _userManager.VerifyUserTokenAsync(
                user, resetProvider, "ResetPassword", token);

            _logger.LogInformation("ConfirmAccount: validWithReset = {Valid}", validWithReset);

            if (!validWithReset)
                throw new Exception("Invalid or expired token. Please request a new password reset.");

            return true;
        }

        public async Task<object> ForgotPassword(ForgotPasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null) return new { message = "If your email is registered, you will receive a password reset link." };

                // Generate password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = Uri.EscapeDataString(token);
                var encodedEmail = Uri.EscapeDataString(request.Email);

                // Build reset URL
                var resetUrl = $"{_emailSettings.AppUrl}/set-password?token={encodedToken}&email={encodedEmail}";

                // Queue or send email
                // ... send email with resetUrl

                return new { message = "If your email is registered, you will receive a password reset link." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Forgot password failed for {Email}", request.Email);
                throw new Exception($"Invalid request.");
            }
        }

        public async Task SetPassword(SetPasswordRequest request)
        {
            try
            {
                // Validate request
                if (string.IsNullOrEmpty(request.Email) ||
                    string.IsNullOrEmpty(request.Token) ||
                    string.IsNullOrEmpty(request.NewPassword))
                {
                   throw  new Exception( "All fields are required" );
                }

                // Validate password strength
                if (request.NewPassword.Length < 8) throw new Exception("Password must be at least 8 characters long" );

                // Find user
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null) throw new Exception("Invalid email" );

                // Reset password
                var result = await  _userManager.ResetPasswordAsync(
                    user,
                    request.Token,
                    request.NewPassword
                );

                
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Password setup failed for {Email}: {Errors}", request.Email, string.Join(", ", errors));
                    throw new Exception("Password setup failed");
                }

                // Confirm email if not already confirmed
                if (!user.EmailConfirmed)
                {
                    var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmResult = await _userManager.ConfirmEmailAsync(user, confirmToken);

                    if (!confirmResult.Succeeded)
                    {
                        _logger.LogWarning("Email confirmation failed for {Email}", request.Email);
                        // Continue anyway since password was set
                    }
                }

                // Update user status
                user.Status = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Password set successfully for {Email}", request.Email);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password setup failed for {Email}", request.Email);
                throw new Exception("An error occurred during password setup");
            }
        }
    }
}