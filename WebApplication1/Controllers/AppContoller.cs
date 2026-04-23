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
using WebApplication1.Domain.Repository;
using WebApplication1.Services.TokenService;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppController : ControllerBase
    {
        private readonly ILogger<AppController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        public AppController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IConfiguration configuration,
            ILogger<AppController> logger,
            IUserRepository userRepository,
        AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _logger = logger;
            _context = context;
            _userRepository = userRepository;
        }
       

            [HttpGet("Routes")]
            [Authorize]
            public async Task<IActionResult> GetProfile()
            {
                try
                {
                    var user = await _userRepository.GetUserByRefreshTokenAsync();

                if (user is null)
                {
                    return BadRequest("User not found");
                }
                    var appRoutes =  _context.ApplicationRoutes
                    .Include(x=> x.Children)
                    .Where(x=> x.ParentId == Guid.Empty || x.Parent == null).AsNoTracking();

                    var results = appRoutes.Select(x=> new GetApplicationRoutesDTO
                    {
                        ParentId = x.ParentId !=null ? (Guid)x.ParentId : Guid.Empty,
                        Id = x.Id,
                        Level = x.Level,
                        Path= x.Path,
                        Title = x.Title,
                        Children = x.Children != null ? x.Children.Select(child=> new GetApplicationRoutesDTO
                        {
                            Id = child != null ?  child.Id : Guid.Empty,
                            ParentId = child != null ? (Guid)child.ParentId : Guid.Empty,
                            Level = child != null ? child.Level : null,
                            Path = child != null ? child.Path : string.Empty,
                            Title = child != null ? child.Title : string.Empty,
                        }) : null

                    }).ToList();

                    return Ok(results);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Get profile failed");
                    return StatusCode(500, new { message = "An error occurred while fetching profile" });
                }
            }


        [HttpGet("AppUserRoutes")]
        [Authorize]
        public async Task<IActionResult> GetAppUserRoutes()
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();

                if (user is null)
                {
                    return BadRequest("User not found");
                }
                var appRoleRoutes = _context.UserRoutes
                    .Include(x=> x.PositionRoutes)
                    .Where(x=> x.UserId == user.Id).AsNoTracking();

                var appRouteIds = appRoleRoutes.Select(x=> x.PositionRoutes.AppRouteId).ToList();
                var appRoutes = _context.ApplicationRoutes.Where(x => appRouteIds.Contains(x.Id));

                var results = appRoutes.Select(x => new GetApplicationRoutesDTO
                {
                    ParentId = x.ParentId != null ? (Guid)x.ParentId : Guid.Empty,
                    Id = x.Id,
                    Level = x.Level,
                    Path = x.Path,
                    Title = x.Title,
                    Children = x.Children != null ? x.Children.Select(child => new GetApplicationRoutesDTO
                    {
                        Id = child != null ? child.Id : Guid.Empty,
                        ParentId = child != null ? (Guid)child.ParentId : Guid.Empty,
                        Level = child != null ? child.Level : null,
                        Path = child != null ? child.Path : string.Empty,
                        Title = child != null ? child.Title : string.Empty,
                    }) : null

                }).ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get profile failed");
                return StatusCode(500, new { message = "An error occurred while fetching profile" });
            }
        }


    }
}