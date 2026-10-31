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
        public async Task<ActionResult<PagedResult<GetPurchaseDto>>> GetAllPurchases(
            [FromQuery] Guid locationId,
            [FromQuery] Guid? supplierId = null,
            [FromQuery] Guid? salesPersonId = null,
            [FromQuery] int? page =1,
            [FromQuery] int? pageSize =1)
        {
            try
            {
                var result = await _purchaseService.GetAllAsync(locationId, GeneralStatus.Active, supplierId, salesPersonId, page, pageSize, 1);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("ManagerCheck")]
        public async Task<ActionResult<PagedResult<GetPurchaseDto>>> ManagerCheck(
            [FromQuery] Guid locationId,
            [FromQuery] Guid? supplierId = null,
            [FromQuery] Guid? salesPersonId = null,
            [FromQuery] int? page =1,
            [FromQuery] int? pageSize =1)
        {
            try
            {
                var result = await _purchaseService.GetAllAsync(locationId, GeneralStatus.Active, supplierId, salesPersonId, page, pageSize, 2);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("PendingRecievals")]
        public async Task<ActionResult<PagedResult<GetPurchaseDto>>> GetApproved(
           [FromQuery] Guid locationId,
           [FromQuery] Guid? supplierId = null,
           [FromQuery] Guid? salesPersonId = null,
           [FromQuery] int? page =1,
            [FromQuery] int? pageSize =1)
        {
            try
            {
                var result = await _purchaseService.GetAllAsync(locationId, GeneralStatus.Approved, supplierId, salesPersonId,  page, pageSize, 3);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("PendingPayments")]
        public async Task<ActionResult<PagedResult<GetPurchaseDto>>> PendingPayments(
           [FromQuery] Guid locationId,
           [FromQuery] Guid? supplierId = null,
           [FromQuery] Guid? salesPersonId = null,
           [FromQuery] int? page =1,
           [FromQuery] int? pageSize =1)
        {
            try
            {
                var result = await _purchaseService.GetAllAsync(locationId, GeneralStatus.Approved, supplierId, salesPersonId,  page, pageSize, 4);
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

       
    }
}