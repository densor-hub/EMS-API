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
        private readonly AppDbContext _context;
        private readonly EmailSettings _emailSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtOptions _jwtOptions;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            ILogger<AuthService> logger,
            IOptions<EmailSettings> emailSettings,
            IHttpContextAccessor httpContextAccessor,
            IOptions<JwtOptions> jwtOptions,
            AppDbContext context)
        {
            _userManager         = userManager;
            _signInManager       = signInManager;
            _tokenService        = tokenService;
            _logger              = logger;
            _context             = context;
            _emailSettings       = emailSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _jwtOptions          = jwtOptions.Value;
        }

        // ────────────────────────────────────────────────────────────
        // Helpers
        // ────────────────────────────────────────────────────────────

        private (string? deviceInfo, string? ip) GetClientInfo()
        {
            var http = _httpContextAccessor.HttpContext;
            return (
                http?.Request.Headers["User-Agent"].FirstOrDefault(),
                http?.Connection.RemoteIpAddress?.ToString()
            );
        }

        private CookieOptions BuildCookieOptions(DateTimeOffset? expires = null) => new()
        {
            HttpOnly = _jwtOptions.RefreshCookie.HttpOnly,
            Secure   = _jwtOptions.RefreshCookie.Secure
                        ?? (_httpContextAccessor.HttpContext?.Request.IsHttps ?? true),
            SameSite = Enum.Parse<SameSiteMode>(_jwtOptions.RefreshCookie.SameSite, ignoreCase: true),
            Path     = _jwtOptions.RefreshCookie.Path,
            Domain   = _jwtOptions.RefreshCookie.Domain,
            Expires  = expires
        };

        private void WriteRefreshCookie(string token, DateTime expiresAt)
            => _httpContextAccessor.HttpContext?.Response.Cookies.Append(
                _jwtOptions.RefreshCookie.Name, token, BuildCookieOptions(expiresAt));

        private void ClearRefreshCookie()
            => _httpContextAccessor.HttpContext?.Response.Cookies.Delete(
                _jwtOptions.RefreshCookie.Name, BuildCookieOptions());

        // ────────────────────────────────────────────────────────────
        // Register
        // ────────────────────────────────────────────────────────────

        public async Task RegisterAsync(RegisterDTO request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var companyExist = await _context.Companies
                    .Where(x => (x.Email.ToLower().Trim() == request.Company.Email.ToLower().Trim())
                             || (x.PhoneNumber.ToLower().Trim() == request.Company.Phone.ToLower().Trim()))
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
                    request.Company?.Address ?? "",
                    request.Company?.Website ?? "",
                    Guid.Empty);

                var user = new ApplicationUser
                {
                    UserName    = request.AdminInfo.Email,
                    Email       = request.AdminInfo.Email,
                    FullName    = request.AdminInfo.FullName,
                    PhoneNumber = request.AdminInfo.PhoneNumber,
                    CreatedAt   = DateTime.UtcNow,
                    Status      = true
                };

                var createUserResult = await _userManager.CreateAsync(user, request.AdminInfo.Password);
                if (!createUserResult.Succeeded)
                    throw new Exception($"User creation failed: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");

                company.UpdateCreatedBy(Guid.Parse(user.Id));

                var adminPosition = await _context.Positions
                    .Where(x => x.Id == Guid.Parse("00000000-0000-0000-0000-000000000001"))
                    .FirstOrDefaultAsync()
                    ?? throw new Exception("Default admin position not configured");

                var initials = PropertyTypeExtensions.GetInitials(company.Name);
                var id = Guid.Parse(user.Id);

                var defaultEmployeeAccount = Employee.Create(
                    id,
                    user.FullName,
                    "",
                    user.Email,
                    $"{initials}-ADMIN",
                    adminPosition.Id,
                    user.PhoneNumber ?? "",
                    DateTime.UtcNow,
                    0,
                    Domain.Enums.EmployeeStatus.Active,
                    company.Id,
                    "",
                    true,
                    DateTime.UtcNow,
                    id,
                    id.ToString());

                var positionRoutes = await _context.PositionRoutes
                    .Where(x => x.PositionId == adminPosition.Id)
                    .AsNoTracking()
                    .ToListAsync();

                var userRoutes = positionRoutes.Select(pr => UserRoutes.Create(
                    Guid.NewGuid(), pr.Id, user.Id, DateTime.UtcNow, Guid.Empty)).ToList();

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

        // ────────────────────────────────────────────────────────────
        // Login
        // ────────────────────────────────────────────────────────────

        public async Task<AuthResponseDTO> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user is null || !user.Status)
                    throw new Exception("Invalid email or password");

                var result = await _signInManager.CheckPasswordSignInAsync(
                    user, request.Password, lockoutOnFailure: true);

                if (!result.Succeeded)
                {
                    if (result.IsLockedOut)
                        throw new Exception("Account is locked out. Please try again later.");
                    throw new Exception("Invalid email or password");
                }

                var (deviceInfo, ip) = GetClientInfo();

                // Single write: GenerateAuthResponseAsync saves the refresh token.
                // Do NOT also call UpdateAsync here — that triggers ConcurrencyStamp conflicts.
                var tokens = await _tokenService.GenerateAuthResponseAsync(user, deviceInfo, ip);

                // Best-effort audit timestamp (fire-and-forget, isolated from the token save).
                user.UpdatedAt = DateTime.UtcNow;
                var stampResult = await _userManager.UpdateAsync(user);
                if (!stampResult.Succeeded)
                {
                    _logger.LogWarning("Failed to update LastLogin for {Email}: {Errors}",
                        user.Email, string.Join(", ", stampResult.Errors.Select(e => e.Description)));
                }

                var company = await _context.Companies
                    .Where(x => x.Id == user.CompanyId)
                    .Select(x => new { x.Id, x.Name })
                    .FirstOrDefaultAsync();

                var employee = await _context.Employees
                    .Include(x => x.EmployeeLocations).ThenInclude(x => x.Location)
                    .Include(x => x.Position).ThenInclude(x => x.PositionRoutes)
                        .ThenInclude(x => x.ApplicationRoutes)
                    .Where(x => x.Id == Guid.Parse(user.Id))
                    .FirstOrDefaultAsync();

                List<DropDownDTO>? allLocationsDTO = null;
                if (employee is not null && employee.Id == employee.CreatedBy)
                {
                    allLocationsDTO = await _context.Locations
                        .Where(x => x.Status && x.CompanyId == user.CompanyId)
                        .Select(x => new DropDownDTO { Code = x.Code, Id = x.Id, Name = x.Name })
                        .ToListAsync();
                }

                var locations = (allLocationsDTO?.Count > 0)
                    ? allLocationsDTO
                    : employee?.EmployeeLocations?.Where(x => x.Status)
                        .Select(x => new DropDownDTO
                        {
                            Code = x.Location.Code,
                            Id   = x.Location.Id,
                            Name = x.Location.Name
                        }).ToList();

                var routes = employee?.Position?.PositionRoutes?
                    .Select(x => new DropDownDTO { Code = x.ApplicationRoutes.Path })
                    .ToList();

                var response = new AuthResponseDTO
                {
                    Company = new IdAndNameDTO
                    {
                        Id   = company?.Id ?? Guid.Empty,
                        Name = company?.Name ?? string.Empty
                    },
                    Email               = user.Email ?? "",
                    FullName            = user.FullName,
                    Id                  = user.Id,
                    Locations           = locations,
                    Routes              = routes,
                    IsCreator           = user.Id == employee?.CreatedBy.ToString(),
                    AccessToken         = tokens.AccessToken,
                    AccessTokenExpires  = tokens.AccessTokenExpires,
                    RefreshToken        = tokens.RefreshToken,
                    RefreshTokenExpires = tokens.RefreshTokenExpires
                };

                WriteRefreshCookie(tokens.RefreshToken, tokens.RefreshTokenExpires);

                _logger.LogInformation("User logged in: {Email}", user.Email);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for {Email}", request.Email);
                throw;
            }
        }

        // ────────────────────────────────────────────────────────────
        // Refresh
        // ────────────────────────────────────────────────────────────

        public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new Exception("Refresh token not provided");

            var (deviceInfo, ip) = GetClientInfo();

            // Atomic rotate. Returns null on: not found / revoked / expired / lost race / save failed.
            var rotated = await _tokenService.TryRefreshAsync(refreshToken, deviceInfo, ip);
            if (rotated is null)
            {
                ClearRefreshCookie();
                throw new Exception("Invalid or expired refresh token");
            }

            var user = rotated.User;

            var company = await _context.Companies
                .Where(x => x.Id == user.CompanyId)
                .Select(x => new { x.Id, x.Name })
                .FirstOrDefaultAsync();

            var employee = await _context.Employees
                .Include(x => x.EmployeeLocations).ThenInclude(x => x.Location)
                .Include(x => x.Position).ThenInclude(x => x.PositionRoutes)
                    .ThenInclude(x => x.ApplicationRoutes)
                .Where(x => x.Id == Guid.Parse(user.Id))
                .FirstOrDefaultAsync();

            List<DropDownDTO>? allLocationsDTO = null;
            if (employee is not null && employee.Id == employee.CreatedBy)
            {
                allLocationsDTO = await _context.Locations
                    .Where(x => x.Status && x.CompanyId == user.CompanyId)
                    .Select(x => new DropDownDTO { Code = x.Code, Id = x.Id, Name = x.Name })
                    .ToListAsync();
            }

            var locations = (allLocationsDTO?.Count > 0)
                ? allLocationsDTO
                : employee?.EmployeeLocations?.Where(x => x.Status)
                    .Select(x => new DropDownDTO
                    {
                        Code = x.Location.Code,
                        Id   = x.Location.Id,
                        Name = x.Location.Name
                    }).ToList();

            var routes = employee?.Position?.PositionRoutes?
                .Select(x => new DropDownDTO { Code = x.ApplicationRoutes.Path })
                .ToList();

            var response = new AuthResponseDTO
            {
                Company = new IdAndNameDTO
                {
                    Id   = company?.Id ?? Guid.Empty,
                    Name = company?.Name ?? string.Empty
                },
                Email               = user.Email ?? "",
                FullName            = user.FullName,
                Id                  = user.Id,
                Locations           = locations,
                Routes              = routes,
                IsCreator           = user.Id == employee?.CreatedBy.ToString(),
                AccessToken         = rotated.NewAccessToken,
                AccessTokenExpires  = DateTime.UtcNow.AddMinutes(_jwtOptions.TokenExpirationMinutes),
                RefreshToken        = rotated.NewRefreshToken,
                RefreshTokenExpires = rotated.NewRefreshTokenExpires
            };

            WriteRefreshCookie(rotated.NewRefreshToken, rotated.NewRefreshTokenExpires);
            return response;
        }

        // ────────────────────────────────────────────────────────────
        // Logout
        // ────────────────────────────────────────────────────────────

        public async Task LogoutAsync(string userId, string accessToken, string? refreshToken)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(refreshToken))
                    await _tokenService.RevokeRefreshTokenAsync(refreshToken);

                if (!string.IsNullOrWhiteSpace(accessToken))
                    await _tokenService.RevokeAccessTokenAsync(accessToken);

                await _signInManager.SignOutAsync();

                // Clear cookie. `BuildCookieOptions` matches what we used to write it,
                // so the browser removes the same cookie entry.
                ClearRefreshCookie();

                _logger.LogInformation("User logged out: {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed for {UserId}", userId);
                throw;
            }
        }

        // ────────────────────────────────────────────────────────────
        // Change password
        // ────────────────────────────────────────────────────────────

        public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId)
                    ?? throw new Exception("User not found");

                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

                if (!result.Succeeded)
                    throw new Exception(
                        $"Password change failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                // Optional: invalidate other sessions after a password change.
                // await _tokenService.RevokeAllRefreshTokensAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password change failed for {UserId}", userId);
                throw;
            }
        }

        // ────────────────────────────────────────────────────────────
        // Profile
        // ────────────────────────────────────────────────────────────

        public async Task<object> GetProfileAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId)
                    ?? throw new Exception("User not found");

                var employee = await _context.Employees
                    .Include(x => x.EmployeeLocations).ThenInclude(x => x.Location)
                    .Include(x => x.Position).ThenInclude(x => x.PositionRoutes)
                        .ThenInclude(x => x.ApplicationRoutes)
                    .Where(x => x.Id == Guid.Parse(user.Id))
                    .FirstOrDefaultAsync();

                return new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    user.PhoneNumber,
                    user.EmailConfirmed,
                    user.TwoFactorEnabled,
                    user.CreatedAt,
                    user.Status,
                    Position = employee?.Position is not null
                        ? new DropDownDTO { Code = "", Id = employee.Position.Id, Name = employee.Position.Title }
                        : null,
                    Locations = employee?.EmployeeLocations?
                        .Select(x => new DropDownDTO
                        {
                            Code = x.Location.Code,
                            Id   = x.Location.Id,
                            Name = x.Location.Name
                        }).ToList(),
                    Routes = employee?.Position?.PositionRoutes?
                        .Select(x => new DropDownDTO
                        {
                            Code = "",
                            Id   = x.ApplicationRoutes.Id,
                            Name = x.ApplicationRoutes.Title
                        }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get profile failed for {UserId}", userId);
                throw;
            }
        }

        // ────────────────────────────────────────────────────────────
        // Delete user
        // ────────────────────────────────────────────────────────────

        public async Task DeleteUserAsync(string userId, string currentUserId)
        {
            if (currentUserId == userId)
                throw new Exception("You cannot delete your own account. Contact another administrator.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByIdAsync(userId)
                    ?? throw new Exception("User not found");

                _logger.LogInformation("Starting deletion for {UserId} ({Email})", userId, user.Email);

                // Revoke sessions BEFORE deleting the user row (no FK dependency surprises).
                await _tokenService.RevokeAllRefreshTokensAsync(userId);

                foreach (var login in await _userManager.GetLoginsAsync(user))
                    await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);

                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                    await _userManager.RemoveFromRolesAsync(user, roles);

                foreach (var claim in await _userManager.GetClaimsAsync(user))
                    await _userManager.RemoveClaimAsync(user, claim);

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                    throw new Exception(
                        $"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                await transaction.CommitAsync();
                _logger.LogInformation("User deleted: {UserId}", userId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ────────────────────────────────────────────────────────────
        // Password reset flow
        // ────────────────────────────────────────────────────────────

        public async Task<bool> ConfirmAccount(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                _logger.LogError("ConfirmAccount: user not found for {Email}", email);
                throw new Exception("Invalid or expired token. Please request a new password reset.");
            }

            var resetProvider = _userManager.Options.Tokens.PasswordResetTokenProvider;
            var valid = await _userManager.VerifyUserTokenAsync(user, resetProvider, "ResetPassword", token);

            _logger.LogInformation("ConfirmAccount: valid={Valid} for {Email}", valid, email);

            if (!valid)
                throw new Exception("Invalid or expired token. Please request a new password reset.");

            return true;
        }

        public async Task<object> ForgotPassword(ForgotPasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user is null)
                    return new { message = "If your email is registered, you will receive a password reset link." };

                var token        = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = Uri.EscapeDataString(token);
                var encodedEmail = Uri.EscapeDataString(request.Email);
                var resetUrl     = $"{_emailSettings.AppUrl}/set-password?token={encodedToken}&email={encodedEmail}";

                // ... enqueue / send email with resetUrl ...

                return new { message = "If your email is registered, you will receive a password reset link." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Forgot password failed for {Email}", request.Email);
                throw new Exception("Invalid request.");
            }
        }

        public async Task SetPassword(SetPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) ||
                    string.IsNullOrEmpty(request.Token) ||
                    string.IsNullOrEmpty(request.NewPassword))
                    throw new Exception("All fields are required");

                if (request.NewPassword.Length < 8)
                    throw new Exception("Password must be at least 8 characters long");

                var user = await _userManager.FindByEmailAsync(request.Email)
                    ?? throw new Exception("Invalid email");

                var resetResult = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
                if (!resetResult.Succeeded)
                {
                    var errors = string.Join(", ", resetResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Password setup failed for {Email}: {Errors}", request.Email, errors);
                    throw new Exception($"Password setup failed: {errors}");
                }

                // ResetPasswordAsync already confirms the email when the token was
                // issued from GeneratePasswordResetTokenAsync. No extra confirm needed.

                user.Status    = true;
                user.UpdatedAt = DateTime.UtcNow;

                var update = await _userManager.UpdateAsync(user);
                if (!update.Succeeded)
                {
                    _logger.LogWarning("Post-reset user update failed for {Email}: {Errors}",
                        request.Email, string.Join(", ", update.Errors.Select(e => e.Description)));
                }

                // Invalidate any old sessions after a password reset.
                await _tokenService.RevokeAllRefreshTokensAsync(user.Id);

                _logger.LogInformation("Password set successfully for {Email}", request.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password setup failed for {Email}", request.Email);
                throw;
            }
        }
    }
}