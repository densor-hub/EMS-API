// Controllers/AuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using WebApplication1.Domain.DTO;
using WebApplication1.Services.ControllerServices;
using WebApplication1.Services.TokenService;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly TokenCookieHelper _cookieHelper;
        private readonly ILogger<AuthController> _logger;
        private readonly JwtOptions _jwtOptions;
        public AuthController(
            IAuthService authService,
            TokenCookieHelper cookieHelper,
            ILogger<AuthController> logger,
            IOptions<JwtOptions> jwtOptions)
        {
            _authService  = authService;
            _cookieHelper = cookieHelper;
            _logger       = logger;
            _jwtOptions = jwtOptions.Value;
        }

        // ---------------------------------------------------------------------
        // Register
        // ---------------------------------------------------------------------
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO request)
        {
            try
            {
                await _authService.RegisterAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for {Email}", request.AdminInfo.Email);
                return BadRequest(new { message = ex.Message });
            }
        }

        // ---------------------------------------------------------------------
        // Login
        // ---------------------------------------------------------------------
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);

                // Cookies (browser transport)
                _cookieHelper.SetRefreshToken(Response, result.RefreshToken);
                // Uncomment if you also issue an access-token cookie:
                // _cookieHelper.SetAccessToken(Response, result.AccessToken);

                // Mobile transport — only if this request looks like a mobile client
                if (IsMobileClient())
                    _cookieHelper.ExposeRefreshTokenToMobile(Response, result.RefreshToken);

                // Never return the refresh token in the body to the browser
                result.RefreshTokenExpires = DateTime.MinValue;
                result.RefreshToken        = string.Empty;

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for {Email}", request.Email);
                return Unauthorized(new { message = ex.Message });
            }
        }

        // ---------------------------------------------------------------------
        // Refresh
        // ---------------------------------------------------------------------
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> RefreshToken()
        {
            try
            {
                var refreshToken = _cookieHelper.GetRefreshToken(Request);
                if (string.IsNullOrEmpty(refreshToken))
                    return Unauthorized(new { message = "Missing refresh token" });

                var result = await _authService.RefreshTokenAsync(refreshToken);

                // Rotate: set the new cookie
                _cookieHelper.SetRefreshToken(Response, result.RefreshToken);
                // _cookieHelper.SetAccessToken(Response, result.AccessToken);

                if (IsMobileClient())
                    _cookieHelper.ExposeRefreshTokenToMobile(Response, result.RefreshToken);

                result.RefreshTokenExpires = DateTime.MinValue;
                result.RefreshToken        = string.Empty;

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // ---------------------------------------------------------------------
        // Logout
        // ---------------------------------------------------------------------
       [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var access = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var refresh = Request.Cookies[_jwtOptions.RefreshCookie.Name]
                        ?? Request.Headers[_jwtOptions.RefreshHeaderName].FirstOrDefault();

            await _authService.LogoutAsync(userId, access, refresh);
            return Ok(new { message = "Logged out" });
        }
        // ---------------------------------------------------------------------
        // Profile
        // ---------------------------------------------------------------------
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _authService.GetProfileAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get profile failed");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ---------------------------------------------------------------------
        // Delete user
        // ---------------------------------------------------------------------
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _authService.DeleteUserAsync(userId, currentUserId);

                return Ok(new
                {
                    message       = "User deleted successfully",
                    deletedUserId = userId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ---------------------------------------------------------------------
        // Confirm account
        // ---------------------------------------------------------------------
        [AllowAnonymous]
        [HttpGet("Account/Confirm")]
        public async Task<IActionResult> ConfirmAccount(
            [FromQuery] string token,
            [FromQuery] string email)
        {
            // _logger.LogInformation("Token length: {Len}", token?.Length ?? 0);
            // _logger.LogInformation("Token raw: {Token}", token);
            // _logger.LogInformation("Email: {Email}", email);

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return BadRequest(new { message = "Invalid or missing parameters" });

            try
            {
                var valid = await _authService.ConfirmAccount(token, email);

                if (!valid)
                    return BadRequest(new { message = "Invalid or missing parameters" }); // ← was missing `return`

                return Ok(new
                {
                    email,
                    token,
                    message = "Please set your new password"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading password setup page");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        // ---------------------------------------------------------------------
        // Set password
        // ---------------------------------------------------------------------
        [HttpPost("Set-Password")]
        [AllowAnonymous]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request)
        {
            try
            {
                await _authService.SetPassword(request);

                return Ok(new
                {
                    success = true,
                    message = "Please login to continue"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password setup failed for {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred during password setup" });
            }
        }

        // ---------------------------------------------------------------------
        // Forgot password
        // ---------------------------------------------------------------------
        [HttpPost("Forgot-Password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                await _authService.ForgotPassword(request);

                return Ok(new
                {
                    message = "If your email is registered, you will receive a password reset link."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Forgot password failed for {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        // ---------------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------------
        private bool IsMobileClient()
        {
            // Recommended: extract to a shared extension so middleware can use it too.
            var ua = Request.Headers.UserAgent.ToString();
            return ua.Contains("MobileApp", StringComparison.OrdinalIgnoreCase)
                || Request.Headers.ContainsKey(CookieNames.ClientTypeHeaderName);
        }
    }
}