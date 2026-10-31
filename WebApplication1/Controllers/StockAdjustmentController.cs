// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using WebApplication1.Controllers;
// using WebApplication1.Domain.DTO;
// using WebApplication1.Domain.Enums;
// using WebApplication1.Domain.Repository;
// using WebApplication1.DTOs;
// using WebApplication1.Services.ControllerServices;

// namespace WebApplication1.Controllers
// {
//     [ApiController]
//     [Route("[controller]")]
//     [Authorize]
//     public class StockAdjustmentController : ControllerBase
//     {
//         private readonly IStockAdjustmentService _service;
//         private readonly ITransactionService _transactionService;
//         private readonly IUserRepository _userRepository;

//         public StockAdjustmentController(IStockAdjustmentService service, ITransactionService transactionService, IUserRepository userRepository)
//         {
//             _service = service;
//             _transactionService = transactionService;
//             _userRepository = userRepository;
//         }

        
//         [HttpGet("Requests")]
//         public async Task<ActionResult<PagedResult<GetPurchaseDto>>> GetAllPurchases(
//             [FromQuery] Guid locationId,
//             [FromQuery] Guid? supplierId = null,
//             [FromQuery] Guid? salesPersonId = null,
//             [FromQuery] int? page =1,
//             [FromQuery] int? pageSize =1)
//         {
//             try
//             {
//                 var result = await _service.GetAllAsync(locationId, GeneralStatus.Active, supplierId, salesPersonId, page, pageSize);
//                 return Ok(result);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, new { message = ex.Message });
//             }
//         }


//         [HttpGet("Approved")]
//         public async Task<ActionResult<PagedResult<GetPurchaseDto>>> GetApproved(
//            [FromQuery] Guid locationId,
//            [FromQuery] Guid? supplierId = null,
//            [FromQuery] Guid? salesPersonId = null,
//            [FromQuery] int? page =1,
//             [FromQuery] int? pageSize =1)
//         {
//             try
//             {
//                 var result = await _service.GetAllAsync(locationId, GeneralStatus.Approved, supplierId, salesPersonId,  page, pageSize);
//                 return Ok(result);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, new { message = ex.Message });
//             }
//         }



//         [HttpPost]
//         public async Task<ActionResult<Guid>> CreatePurchase([FromBody] CreateTransactionDto createDto)
//         {
//             try
//             {
//                 var user = await _userRepository.GetUserByRefreshTokenAsync();
//                 if (user == null)
//                     return Unauthorized(new { message = "User not found" });

//                 var userId = Guid.Parse(user.Id);
                
//                 //stamp the type to purchase
//                 createDto.TransactionType = TransactionType.ADJS;
//                 await _transactionService.CompleteTransationProcess(createDto, TransactionResultsType.Creation);

//                 return StatusCode(200);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, new { message = ex.Message });
//             }
//         }

//         [HttpPut]
//         public async Task<ActionResult<GetPurchaseDto>> UpdatePurchase([FromBody] UpdateAdjustmentPurchaseDto updateDto)
//         {
//             try
//             {
//                 var user = await _userRepository.GetUserByRefreshTokenAsync();
//                 if (user == null)
//                     return Unauthorized(new { message = "User not found" });

//                 var userId = Guid.Parse(user.Id);
//                 await _service.ManagerCheck( updateDto);

//                 //if (result == null)
//                 //    return NotFound(new { message = "Purchase not found" });

//                 return Ok();
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, new { message = ex.Message });
//             }
//         }

//     }
// }