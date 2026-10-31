using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StyleCop;
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
    [Route("[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        public SystemController(
            IUserRepository userRepository,
        AppDbContext context)
        {
            _context = context;
            _userRepository = userRepository;
        }
       

            [HttpPut("Settings")]
            [Authorize]
            public async Task<IActionResult> UpdateSettings ([FromBody] UpdateSettingsDTO createdDto)
            {
                try
                {
                    var user = await _userRepository.GetUserByRefreshTokenAsync();

                if (user is null)
                {
                    return BadRequest("User not found");
                }

                if (createdDto.MinimumSalePaymentPercentage > 100) throw new Exception("Minimum Sale Payment Percentage cannot be more than 100%");
                if (createdDto.MinimumSalePaymentPercentage < 1) throw new Exception("Minimum Sale Payment Percentage must be greather than 1%");

                var settings = await _context.SystemControls
                .Include(x=> x.Location)
                .Where(x=> x.LocationId == createdDto.LocationId).FirstOrDefaultAsync();

                if (settings is not null)
                {
                    settings.Update(createdDto.SaleToDeliveryMode, createdDto.RecordingOldTransactions, createdDto.MinimumSalePaymentPercentage, createdDto.PosPrinterSize, createdDto.DefaultPageSize, createdDto.Currency ?? "GHS", user.Id);
                    _context.SystemControls.Update(settings);
                }
                else
                {
                   var  newSettings = SystemControl.Create(Guid.NewGuid(), createdDto.LocationId, createdDto.SaleToDeliveryMode, createdDto.RecordingOldTransactions, createdDto.MinimumSalePaymentPercentage, createdDto.PosPrinterSize, createdDto.DefaultPageSize, createdDto.Currency ?? "GHS", user.Id);
                   await _context.SystemControls.AddAsync(newSettings);
                }

                await _context.SaveChangesAsync();
                 return Ok();
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Get profile failed");
                return StatusCode(500, new { message = ex.Message ?? "An error occurred while fetching profile" });
            }
        }


        [HttpGet("Settings/{locationId:Guid}")]
        [Authorize]
        public async Task<ActionResult<UpdateSettingsDTO>> GetAppUserRoutes([FromRoute] Guid locationId)
        {
            try
            {
                // var user = await _userRepository.GetUserByRefreshTokenAsync();

                // if (user is null)
                // {
                //     return BadRequest("User not found");
                // }
                  var settings = await _context.SystemControls
              //  .Include(x=> x.Location)
                .Where(x=> x.LocationId == locationId).FirstOrDefaultAsync();

                if (settings is null) settings = await _context.SystemControls.Where(x=> x.LocationId == null).FirstOrDefaultAsync();
                
                var results = new UpdateSettingsDTO
                {
                    Currency= settings.Currency,
                    DefaultPageSize = settings.DefaultPageSize,
                    LocationId = (Guid)(settings.LocationId !=null ? settings.LocationId : Guid.Empty),
                    MinimumSalePaymentPercentage = settings.MinimumSalePaymentPercentage,
                    PosPrinterSize = settings.PosPrinterSize,
                    RecordingOldTransactions = settings.RecordingOldTransactions,
                    SaleToDeliveryMode = settings.SaleToDeliveryMode
                };

                return Ok(results);
            }
            catch (Exception ex)
            {
             //   _logger.LogError(ex, "Get profile failed");
                return StatusCode(500, new { message = ex.Message ?? "An error occurred while fetching profile" });
            }
        }


    }
}