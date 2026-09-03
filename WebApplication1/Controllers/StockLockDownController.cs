// Controllers/StockTakeController.cs
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Domain.DTO;
using WebApplication1.Services;
using WebApplication1.Services.ControllerServices;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StockLockDownController : ControllerBase
    {
        private readonly IStockLockDownService _stockLockDownService;

        public StockLockDownController(IStockLockDownService stockLockDownService)
        {
            _stockLockDownService = stockLockDownService;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> CreateStockLockDownRequest([FromBody] CreateStockLockDownRequestDto dto)
        {
            try
            {
                var result = await _stockLockDownService.CreateStockLockDownWithItemsAsync(dto);
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAllStockLockDownDto>>> GetStockLockDowns ([FromQuery] GetStockLockDownQuery filter)
        {
            try
            {
                var result = await _stockLockDownService.GetStockLockDownRequestsByLocationAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetStockLockDownByIdDto>> GetStockLockDownById([FromRoute] Guid id)
        {
            try
            {
                var result = await _stockLockDownService.GetStockLockDownRequestsByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("Submit-Stock")]
        public async Task<IActionResult> CreateStockTake([FromBody] CreateStockSubmissionDto dto)
        {
            try
            {
                var result = await _stockLockDownService.SubmitStockTakeItem(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("Submitted")]
        public async Task<ActionResult<IEnumerable<GetStockLockDownSubmittedItemDto>>> GetSumittedStocks([FromQuery] GetStockLockDownQuery filter)
        {
            try
            {
                var result = await _stockLockDownService.GetSubmittedStockLockedItems(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("Verify")]
        public async Task<IActionResult> VerifyStockTake([FromBody] UpdateStockTakeSubmissionDto dto)
        {
            try
            {
                var result = await _stockLockDownService.UpdateStockTakeItem(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Escalate")]
        public async Task<IActionResult> EscalateStockVariance([FromBody] UpdateStockTakeSubmissionDto dto)
        {
            try
            {
                var result = await _stockLockDownService.UpdateStockTakeItem(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("Comments")]
        public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetComments(Guid StockLockDownRequestId)
        {
            try
            {
                var result = await _stockLockDownService.GetComments(StockLockDownRequestId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}