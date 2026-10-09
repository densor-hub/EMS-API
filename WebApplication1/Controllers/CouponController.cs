using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.DAL.Repository;
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
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;
        private readonly ILogger<CouponController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ILocationRepository _locationRepository;
        public CouponController(
            ICouponRepository couponRepository,
            ILogger<CouponController> logger,
            IUserRepository userRepository,
             ILocationRepository locationRepository
            )
        {
            _couponRepository = couponRepository;
            _logger = logger;
            _userRepository = userRepository;
            _locationRepository = locationRepository;
        }

        // GET: api/employees
        [HttpGet("{locationId:Guid}")]
        public async Task<ActionResult<IEnumerable<GetCouponDTO>>> GetAllCoupons([FromRoute] Guid locationId, [FromQuery] bool? status = null)
        {
            try
            {
                var coupons =  _couponRepository.GetAllAsync(locationId);

                if (status != null) coupons = coupons.Where(x=> x.Used == status);

                var returnData = coupons.Select(coupon => new GetCouponDTO
                {
                    Id = coupon.Id,
                    Amount = coupon.Amount,
                    Code = coupon.Code,
                    ExpiryDate = coupon.ExpiryDate,
                    Used = coupon.Used,
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
        [HttpGet("{code}")]
        public async Task<ActionResult<GetCouponDTO>> GetByCode([FromRoute] string code, [FromQuery] Guid locationId)
        {
            try
            {
                var coupon = await _couponRepository.GetByCodeAsync(code, locationId);

                if (coupon == null)
                    return NotFound($"Coupon not found");

                var returnData = new GetCouponDTO
                {
                    Id = coupon.Id,
                    Amount = coupon.Amount,
                    Code = coupon.Code,
                    ExpiryDate = coupon.ExpiryDate,
                    Used = coupon.Used,
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
        public async Task<ActionResult> CreateCoupon([FromBody] CreateCouponDTO createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var currentUser = await _userRepository.GetUserByRefreshTokenAsync();
                var location = await _locationRepository.GetByIdAsync(createDto.LocationId);

                if (location is null) return BadRequest("Shop not found");

                if (createDto?.CouponAmounts?.Count > 0)
                {
                    var newCoupons = new List<Coupon>();

                    foreach (var item in createDto.CouponAmounts)
                    {
                        var code = await _couponRepository.GenerateCodeAsync();

                        var coupon = Coupon.Create(
                            Guid.NewGuid(),
                            code,
                            item.Amount,
                            item.ExpiryDate.HasValue,
                            item.ExpiryDate,
                            Guid.Parse(currentUser.Id),
                            location.Id
                        );

                        newCoupons.Add(coupon);
                    }

                    await _couponRepository.CreateRangeAsync(newCoupons);
                    await _couponRepository.SaveChangesAsync();
                }

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating coupon");
                return StatusCode(500, "An error occurred while creating the coupons");
            }
        }
        // PUT: api/employees/{id}
        [HttpPut]
        public async Task<ActionResult> UpdateEmployee([FromBody] UpdateCouponDTO createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var coupon = await _couponRepository.GetByIdAsync(createDto.Id) ;
                if (coupon == null) return NotFound($"Customer not found");

                var currentUserId = await _userRepository.GetCurrentUserId();

                coupon.Update(createDto.ExpiryDate, createDto.Amount, createDto.Status);

                await _couponRepository.UpdateAsync(coupon);
                return Ok(coupon);
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
                var deleted = await _couponRepository.DeleteAsync(id);
                if (!deleted)
                    return NotFound($"Coupon not found");

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
