using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Helpers;
using WebApplication1.Services.TokenService;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IConfiguration configuration,
            ILogger<AuthController> logger,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Check if company already exists
                var companyExist = await _context.Companies
                    .Where(x => ( x.Email.ToLower().Trim() == request.Company.Email.ToLower().Trim())
                                || x.PhoneNumber.ToLower().Trim() == request.Company.Phone.ToLower().Trim()
                                ).FirstOrDefaultAsync();

                if (companyExist != null)
                {
                    return BadRequest(new { message = "Company already registered" });
                }

                // 2. Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(request.AdminInfo.Email.Trim());
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Email already registered" });
                }

                // 3. Create the user FIRST (to get the ID)
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
                    return BadRequest(new { message = "User creation failed", errors });
                }

                // 4. Create the company WITH the user's ID as CreatedBy
                var company = Company.Create(
                    Guid.NewGuid(),
                    request.Company.Name,
                    request.Company.Phone,
                    request.Company.Email,
                    true,
                    DateTime.UtcNow,
                    request.Company?.TIN??"",
                    request.Company?.RegistrationNumber??"",
                    request?.Company?.Address ?? "",
                    request?.Company?.Website ?? "",
                    Guid.Parse(user.Id) // Use the user's ID as CreatedBy
                );

                var adminPosition = await _context.Positions.Where(x => x.Id == Guid.Parse("00000000-0000-0000-0000-000000000001")).FirstOrDefaultAsync();

                var initials = StringExtensions.GetInitials(company.Name);
                var defaultEmployeeAccount = Employee.Create(Guid.Parse(user.Id), user.FullName, "", user.Email, $"{initials}-ADMIN", adminPosition.Id, user?.PhoneNumber ?? "", DateTime.UtcNow, 0, Domain.Enums.EmployeeStatus.Active,
                    company.Id, "", true, DateTime.UtcNow, Guid.Parse(user.Id));


                var positionRoutes = await _context.PositionRoutes.Where(x=> x.PositionId == adminPosition.Id).AsNoTracking().ToListAsync();

                var employeeLocations = new List<UserRoutes>();
                
                foreach (var positionRoute in positionRoutes)
                {

                    var userRoute = UserRoutes.Create(
                        Guid.NewGuid(),
                       positionRoute.Id,
                        user.Id,  // string userId - matches your entity
                        DateTime.UtcNow,
                        Guid.Empty
                    );
                    employeeLocations.Add(userRoute);
                }


                await _context.Companies.AddAsync(company);
                // 5. Link user to company
                user.CompanyId = company.Id;
                await _userManager.UpdateAsync(user);

                await _context.Employees.AddAsync(defaultEmployeeAccount);

                await _context.UserRoutes.AddRangeAsync(employeeLocations);

               
                await _context.SaveChangesAsync(); // Save company to get its ID

                await transaction.CommitAsync();

                _logger.LogInformation("User registered: {Email}", user.Email);

                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Registration failed for {Email}", request.AdminInfo.Email);
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
            public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginRequest request)
            {
                try
                {

                    var user = await _userManager.FindByEmailAsync(request.Email);
                    if (user == null || user?.Status == false)
                    {
                        return Unauthorized(new { message = "Invalid email or password" });
                    }

                    // Check password
                    var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password,
                        lockoutOnFailure: true);

                    if (!result.Succeeded)
                    {
                        if (result.IsLockedOut)
                        {
                            return Unauthorized(new { message = "Account is locked out. Please try again later." });
                        }

                        return Unauthorized(new { message = "Invalid email or password" });
                    }

                    var company=  await   _context.Companies.Where(x=> x.Id == user.CompanyId).Select(x=> new { x.Id, x.Name}).FirstOrDefaultAsync();

                var employee = await _context.Employees
                  .Include(x => x.EmployeeLocations)
                      .ThenInclude(x => x.Location)
                  .Include(x => x.Position)
                      .ThenInclude(x => x.PositionRoutes)
                          .ThenInclude(x => x.ApplicationRoutes) // Include ApplicationRoutes
                  .Where(x => x.Id == Guid.Parse(user.Id))
                  .FirstOrDefaultAsync();

                var allLocationsDTO = new List<DropDownDTO>();
                
                if (employee?.Id ==  employee?.CreatedBy)
                {
                    var locations = _context.Locations.Where(x => x.Status == true && x.CompanyId == user.CompanyId);
                    
                    allLocationsDTO = await locations.Select(x=> new DropDownDTO
                    {
                        Code = x.Code,
                        Id = x.Id,
                        Name = x.Name
                    }).ToListAsync();
                }

                
                // Update last login
                user.UpdatedAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    // Generate tokens
                    var tokens = await _tokenService.GenerateAuthResponseAsync(user);


                // Set refresh token as HttpOnly cookie (more secure)
                Response.Cookies.Append("refresh_token", tokens.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // required for SameSite=None
                    SameSite = SameSiteMode.None, //  THIS FIXES CHROME
                    Expires = tokens.RefreshTokenExpires,
                    Path = "/"
                });

                Response.Headers["X-Refresh-Token"] = tokens.RefreshToken;

                var authResponse = new AuthResponseDTO
                {
                    Company = new IdAndNameDTO
                    {
                        Id = company == null ? Guid.Empty : company.Id,
                        Name = company == null ? string.Empty :  company.Name
                    },
                    Email = user.Email??"",
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
                    }
                    ).ToList() : null,
                    IsCreator = user.Id == employee?.CreatedBy.ToString()
                };


                _logger.LogInformation("User logged in: {Email}", user.Email);
                    return Ok(authResponse);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Login failed for {Email}", request.Email);
                    return StatusCode(500, new { message = "An error occurred during login" });
                }
            }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> RefreshToken()
        {
            // 1. Get refresh token ONLY from httpOnly cookie
            var refreshToken = Request.Cookies["refresh_token"]; //refreh-token
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "Refresh token not found" });
            }

            // 2. Find user by refresh token
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            var test = GetPrincipalFromExpiredToken(refreshToken);

            if (user == null || user.Status == false)
            {
                return Unauthorized(new { message = "Invalid refresh token or user inactive" });
            }

            // 3. Validate refresh token
            if (!await _tokenService.ValidateRefreshTokenAsync(user, refreshToken))
            {
                return Unauthorized(new { message = "Refresh token expired" });
            }

            // 4. Generate new tokens
            var tokens = await _tokenService.GenerateAuthResponseAsync(user);

            // 5. Set new refresh token as httpOnly cookie
            Response.Cookies.Append("refresh_token", tokens.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // required for SameSite=None
                SameSite = SameSiteMode.None, //  THIS FIXES CHROME
                Expires = tokens.RefreshTokenExpires,
                Path = "/"
            });

            var company = await _context.Companies.Where(x => x.Id == user.CompanyId).Select(x => new { x.Id, x.Name }).FirstOrDefaultAsync();
            // 6. Return only what frontend needs

            var employee = await _context.Employees
                   .Include(x => x.EmployeeLocations)
                       .ThenInclude(x => x.Location)
                   .Include(x => x.Position)
                       .ThenInclude(x => x.PositionRoutes)
                           .ThenInclude(x => x.ApplicationRoutes) // Include ApplicationRoutes
                   .Where(x => x.Id == Guid.Parse(user.Id))
                   .FirstOrDefaultAsync();

          
            return Ok( new AuthResponseDTO
            {
                Company = new IdAndNameDTO
                {
                    Id = company == null ? Guid.Empty : company.Id,
                    Name = company == null ? string.Empty : company.Name
                },
                Email = user.Email ?? "",
                FullName = user.FullName,
                Id = user.Id,
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
                }).ToList() : null,
                IsCreator = user.Id == employee?.CreatedBy.ToString()
            });
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var accessToken = HttpContext.Request.Headers["Authorization"]
                    .FirstOrDefault()?.Replace("Bearer ", "");

                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        // Revoke refresh token
                        await _tokenService.RevokeRefreshTokenAsync(user);

                        // Blacklist current access token
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            await _tokenService.RevokeAccessTokenAsync(accessToken);
                        }

                        // Update security stamp to invalidate all sessions
                        await _userManager.UpdateSecurityStampAsync(user);

                        _logger.LogInformation("User logged out: {UserId}", userId);
                    }
                }

                // Sign out from cookie auth
                await _signInManager.SignOutAsync();

                // Clear cookies
                Response.Cookies.Delete("access_token");
                Response.Cookies.Delete("refresh_token");

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }


        [HttpPost("change-password")]
            [Authorize]
            public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await _userManager.FindByIdAsync(userId!);

                    if (user == null)
                    {
                        return BadRequest(new { message = "User not found" });
                    }

                    var result = await _userManager.ChangePasswordAsync(
                        user, request.CurrentPassword, request.NewPassword);

                    if (!result.Succeeded)
                    {
                        var errors = result.Errors.Select(e => e.Description);
                        return BadRequest(new { message = "Password change failed", errors });
                    }

                    return Ok(new { message = "Password changed successfully" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Password change failed");
                    return StatusCode(500, new { message = "An error occurred during password change" });
                }
            }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId!);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var employee = await _context.Employees
                    .Include(x => x.EmployeeLocations)
                        .ThenInclude(x => x.Location)
                    .Include(x => x.Position)
                        .ThenInclude(x => x.PositionRoutes)
                            .ThenInclude(x => x.ApplicationRoutes) // Include ApplicationRoutes
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

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get profile failed");
                return StatusCode(500, new { message = "An error occurred while fetching profile" });
            }
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]!)),
                    ValidateLifetime = false // We want to validate expired tokens
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                try
                {
                    var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                    if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                        !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                        return null;

                    return principal;
                }
                catch
                {
                    return null;
                }
            }


        [HttpDelete]
        [Authorize]
        //[Authorize(Roles = "Admin")] // Restrict to admin users only
        public async Task<IActionResult> DeleteUser(string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Don't allow deleting yourself
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId == userId)
                {
                    return BadRequest(new { message = "You cannot delete your own account. Contact another administrator." });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                _logger.LogInformation("Starting deletion process for user: {UserId}, {Email}", userId, user.Email);

                // 1. Revoke all tokens
                //await _tokenService.RevokeAllUserTokensAsync(userId);

                // 2. Remove from any SignalR groups if applicable
                // await _notificationHub.RemoveUserFromAllGroups(userId);

                // 3. Delete related data based on user role/type
                if (await _userManager.IsInRoleAsync(user, "Employee"))
                {
                   // await DeleteEmployeeRelatedData(userId);
                }
                else if (await _userManager.IsInRoleAsync(user, "Customer"))
                {
                  //  await DeleteCustomerRelatedData(userId);
                }
                // Add other role-specific cleanup as needed

                // 4. Delete user logins and tokens
                var logins = await _userManager.GetLoginsAsync(user);
                foreach (var login in logins)
                {
                    await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                }

                // 5. Remove from roles
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, roles);
                }

                // 6. Delete user claims
                var claims = await _userManager.GetClaimsAsync(user);
                foreach (var claim in claims)
                {
                    await _userManager.RemoveClaimAsync(user, claim);
                }

                // 7. Finally delete the user
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to delete user: {errors}");
                }

                // 8. Commit transaction
                await transaction.CommitAsync();

                // 9. Invalidate any active sessions
                await _tokenService.RevokeRefreshTokenAsync(user);
                //await _tokenService.RevokeAccessTokenAsync();

                _logger.LogInformation("User successfully deleted: {UserId}, {Email}", userId, user.Email);

                return Ok(new
                {
                    message = "User deleted successfully",
                    deletedUserId = userId,
                    deletedEmail = user.Email
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting user: {UserId}", userId);
                return StatusCode(500, new
                {
                    message = "An error occurred while deleting the user",
                    error = ex.Message
                });
            }
        }

    }
}