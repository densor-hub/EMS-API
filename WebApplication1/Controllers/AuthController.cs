// Controllers/AuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Domain.DTO;
using WebApplication1.Services;
using WebApplication1.Services.Emails.EmailService.Entities;
using static QRCoder.PayloadGenerator;
using ZXing.Aztec.Internal;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

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

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);

                Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = result.RefreshTokenExpires,
                    Path = "/"
                });

                Response.Headers["X-Refresh-Token"] = result.RefreshToken;

                result.RefreshTokenExpires = DateTime.MinValue;
                result.RefreshToken = string.Empty;

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for {Email}", request.Email);
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["refresh_token"];
                var result = await _authService.RefreshTokenAsync(refreshToken);

                Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = result.RefreshTokenExpires,
                    Path = "/"
                });

                result.RefreshTokenExpires = DateTime.MinValue;
                result.RefreshToken = string.Empty;

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
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

                await _authService.LogoutAsync(userId, accessToken);

                Response.Cookies.Delete("access_token");
                Response.Cookies.Delete("refresh_token");

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                return StatusCode(500, new { message = ex.Message });
            }
        }

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
                    message = "User deleted successfully",
                    deletedUserId = userId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("Account/Confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmAccount([FromQuery] string token, [FromQuery] string email)
        {

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return BadRequest(new { message = "Invalid or missing parameters" });
            try
            {
                var valid = await _authService.ConfirmAccount(token, email);

                if (!valid) BadRequest(new { message = "Invalid or missing parameters" });
                // Return the set password page (or JSON response for SPA)
                return Ok(new
                    {
                        email = email,
                        token = token,
                        message = "Please set your new password"
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading password setup page");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [HttpPost("Set-Password")]
        [AllowAnonymous]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request)
        {
            try
            {
                // Validate request
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

       
        [HttpPost("Forgot-Password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                await _authService.ForgotPassword(request);

                return Ok(new { message = "If your email is registered, you will receive a password reset link." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Forgot password failed for {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred" });
            }
        }
    }

}
