using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Domain.DTO;
using WebApplication1.Services.ControllerServices;

namespace WebApplication1.API.Controllers
{
    [ApiController]
    [Route("[controller]")]              // ← kept as you had it
    [Authorize]
    public class SplitController : ControllerBase
    {
        private readonly IItemSplitService _service;

        public SplitController(IItemSplitService service)
        {
            _service = service;
        }

        [HttpGet("SetUp/{locationId:Guid}")]
        public async Task<ActionResult<IReadOnlyList<ItemSplitSetupDto>>> GetAllSetUp(Guid locationId,
            CancellationToken ct)
        {
            try
            {
                var result = await _service.GetAllAsync(locationId, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error completing split transaction");
                return StatusCode(500, new { Message = ex.Message ?? "An error occurred while splitting item" });
            }
        }

        [HttpGet("SetUp/{locationId:guid}/{id:guid}")]   // ← added Name
        public async Task<ActionResult<ItemSplitSetupDto>> GetSetUpById(
            Guid locationId,
            Guid id,
            CancellationToken ct)
        {
           try
            {
                var dto = await _service.GetByIdAsync(id, locationId, ct);
                return dto is null ? NotFound() : Ok(dto);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error completing split transaction");
                return StatusCode(500, new { Message = ex.Message ?? "An error occurred while splitting item" });
            }

        }

        [HttpPost("SetUp")]
        public async Task<IActionResult> CreateSetUp(
            [FromBody] CreateItemSplitSetupRequest request,
            CancellationToken ct)
        {
            try
            {
                var id = await _service.CreateAsync(request, ct);
                return CreatedAtRoute(                                    // ← was CreatedAtAction
                    routeName: "GetItemSplitSetUpById",
                    routeValues: new { id },
                    value: new { id });
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error completing split transaction");
                return StatusCode(500, new { Message = ex.Message ?? "An error occurred while splitting item" });
            }
        }

        [HttpPut("SetUp/{id:guid}")]
        public async Task<IActionResult> UpdateSetUp(
            Guid id,
            [FromBody] UpdateItemSplitSetupRequest request,
            CancellationToken ct)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, request, ct);
                return ok ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error completing split transaction");
                return StatusCode(500, new { Message = ex.Message ?? "An error occurred while splitting item" });
            }
        }

        [HttpDelete("SetUp/{id:guid}")]
        public async Task<IActionResult> DeleteSetUp(
            Guid id,
            CancellationToken ct)
        {
           try
            {
                var ok = await _service.DeleteAsync(id, ct);
                return ok ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error completing split transaction");
                return StatusCode(500, new { Message = ex.Message ?? "An error occurred while splitting item" });
            }

        }

        [HttpGet("Transaction")]
        public async Task<ActionResult<IReadOnlyList<ItemSplitTransactionDto>>> GetAllTransactions(
            CancellationToken ct)
        {
            try
            {
                var result = await _service.GetAllTransactionsAsync(ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error completing split transaction");
                return StatusCode(500, new { Message = ex.Message ?? "An error occurred while splitting item" });
            }
        }

        [HttpGet("Transaction/{id:guid}")]
        public async Task<ActionResult<ItemSplitTransactionDto>> GetTransactionById(
            Guid id,
            CancellationToken ct)
        {
            try
            {
                var dto = await _service.GetTransactionByIdAsync(id, ct);
                return dto is null ? NotFound() : Ok(dto);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error completing split transaction");
                return StatusCode(500, new { Message = ex.Message ?? "An error occurred while splitting item" });
            }
        }

        [HttpPost("Transaction")]
        public async Task<IActionResult> CreateTransaction(
            [FromBody] CreateItemSplitTransactionRequest request,
            CancellationToken ct)
        {
           try
            {
                var id = await _service.CreateTransactionAsync(request, ct);
                return CreatedAtRoute(
                    routeValues: new { id },
                    value: new { id });
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error completing split transaction");
                return StatusCode(500, new { Message = ex.Message ?? "An error occurred while splitting item" });
            }
        }
    }
}