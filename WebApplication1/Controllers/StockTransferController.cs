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

        [HttpGet("Requests")]
        public async Task<ActionResult<IEnumerable<StockTransfersDTO>>> GetInflows([FromQuery] BrowseStockTransfersFilters filters)
        {
            try
            {
                filters.Type = 1;
                var result = await _stockTransferService.GetStockTransferRecordsAsync(filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("Manager-Check")]
        public async Task<ActionResult<IEnumerable<StockTransfersDTO>>> GetOutflow([FromQuery] BrowseStockTransfersFilters filters)
        {
            try
            {
                filters.Type = 2;
                filters.Approval = true;
                filters.Stage = Domain.Enums.StockTransferEnum.Pending;
                var result = await _stockTransferService.GetStockTransferRecordsAsync(filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("Approved-For-Delivery")]
        public async Task<ActionResult<IEnumerable<StockTransfersDTO>>> GetApproved([FromQuery] BrowseStockTransfersFilters filters)
        {
            try
            {
                filters.Type = 2;
                filters.Approval = true;
                filters.Stage = Domain.Enums.StockTransferEnum.Approved;
                var result = await _stockTransferService.GetStockTransferRecordsAsync(filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("Receivals")]
        public async Task<ActionResult<IEnumerable<StockTransfersDTO>>> Receivals([FromQuery] BrowseStockTransfersFilters filters)
        {
            try
            {
                filters.Type = 1;
                filters.Approval = true;
                filters.Stage = Domain.Enums.StockTransferEnum.Approved;
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