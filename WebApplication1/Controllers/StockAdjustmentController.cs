using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;
using WebApplication1.Services.ControllerServices;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class StockAdjustmentController : ControllerBase
    {
        private readonly IStockAdjustmentService _service;

        public StockAdjustmentController(IStockAdjustmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<StockAdjustmentDto>>> GetAll(
            CancellationToken ct)
        {
            return Ok(await _service.GetAllAsync(ct));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<StockAdjustmentDto>> GetById(
            Guid id,
            CancellationToken ct)
        {
            var dto = await _service.GetByIdAsync(id, ct);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateStockAdjustmentRequest request,
            CancellationToken ct)
        {
            var id = await _service.CreateAsync(request, ct);
            return CreatedAtRoute(
                routeValues: new { id },
                value: new { id });
        }

        [HttpPut("verify/{id:guid}")]
        public async Task<IActionResult> Verify(
            Guid id,
            [FromBody] VerifyStockAdjustmentRequest request,
            CancellationToken ct)
        {
            var ok = await _service.VerifyAsync(id, request, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}