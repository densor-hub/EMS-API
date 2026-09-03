// Controllers/StockTransferController.cs
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.QueryFilters;
using WebApplication1.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StockTransferController : ControllerBase
    {
        private readonly IStockTransferService _stockTransferService;

        public StockTransferController(IStockTransferService stockTransferService)
        {
            _stockTransferService = stockTransferService;
        }

        [HttpPost("Request")]
        public async Task<ActionResult> CreateStockTransfer(CreateTransactionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                 await _stockTransferService.CreateStockTransferAsync(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("Approval")]
        public async Task<IActionResult> ApproveStockTransfer(StockTransferApproveDto dto)
        {
            try
            {
                 await _stockTransferService.ApproveStockTransferAsync(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("Receival")]
        public async Task<IActionResult> ReceiveStockTransfer(StockTransferReceiveDto dto)
        {
            try
            {
                await _stockTransferService.ReceiveStockTransferAsync(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> GetStockTransfer(Guid id)
        {
            try
            {
                var result = await _stockTransferService.GetStockTransferAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockTransfersDTO>>> GetStockTransferRecords([FromQuery] BrowseStockTransfersFilters filters)
        {
            try
            {
                var result = await _stockTransferService.GetStockTransferRecordsAsync(filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}