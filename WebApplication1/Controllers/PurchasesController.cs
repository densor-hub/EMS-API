// Controllers/PurchaseController.cs
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;
using WebApplication1.DTOs;
using WebApplication1.Services.ControllerServices;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionService _transactionService;

        public PurchasesController(IPurchaseService purchaseService, IUserRepository userRepository, ITransactionService transactionService)
        {
            _purchaseService = purchaseService;
            _userRepository = userRepository;
            _transactionService = transactionService;
        }


        [HttpGet("Requests")]
        public async Task<ActionResult<IEnumerable<GetPurchaseDto>>> GetAllPurchases(
            [FromQuery] Guid locationId,
            [FromQuery] Guid? supplierId = null,
            [FromQuery] Guid? salesPersonId = null)
        {
            try
            {
                var result = await _purchaseService.GetAllAsync(locationId, GeneralStatus.Initiated, supplierId, salesPersonId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("Approved")]
        public async Task<ActionResult<IEnumerable<GetPurchaseDto>>> GetApproved(
           [FromQuery] Guid locationId,
           [FromQuery] Guid? supplierId = null,
           [FromQuery] Guid? salesPersonId = null)
        {
            try
            {
                var result = await _purchaseService.GetAllAsync(locationId, GeneralStatus.Approved, supplierId, salesPersonId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        [HttpPost]
        public async Task<ActionResult<Guid>> CreatePurchase([FromBody] CreateTransactionDto createDto)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null)
                    return Unauthorized(new { message = "User not found" });

                var userId = Guid.Parse(user.Id);
                
                //stamp the type to purchase
                createDto.TransactionType = TransactionType.PURC;
                await _transactionService.CompleteTransationProcess(createDto, TransactionResultsType.Creation);

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult<GetPurchaseDto>> UpdatePurchase([FromBody] UpdatePurchaseDto updateDto)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null)
                    return Unauthorized(new { message = "User not found" });

                var userId = Guid.Parse(user.Id);
                await _purchaseService.ManagerCheck( updateDto);

                //if (result == null)
                //    return NotFound(new { message = "Purchase not found" });

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeletePurchase(Guid id, [FromBody] string reason)
        //{
        //    try
        //    {
        //        var user = await _userRepository.GetUserByRefreshTokenAsync();
        //        if (user == null)
        //            return Unauthorized(new { message = "User not found" });

        //        var userId = Guid.Parse(user.Id);
        //        var result = await _purchaseService.DeleteAsync(id, userId, reason);

        //        if (!result)
        //            return NotFound(new { message = "Purchase not found" });

        //        return Ok(new { message = "Purchase deleted successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = ex.Message });
        //    }
        //}

        //[HttpPost("cancel")]
        //public async Task<ActionResult<PurchaseCancellationDto>> CancelPurchase([FromBody] CreatePurchaseCancellationDto createDto)
        //{
        //    try
        //    {
        //        var user = await _userRepository.GetUserByRefreshTokenAsync();
        //        if (user == null)
        //            return Unauthorized(new { message = "User not found" });

        //        var userId = Guid.Parse(user.Id);
        //        var result = await _purchaseService.CancelAsync(createDto, userId);

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = ex.Message });
        //    }
        //}
    }
}