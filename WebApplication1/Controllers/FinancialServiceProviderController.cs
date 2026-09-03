// Controllers/BankController.cs
// Controllers/BankController.cs
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs;
using WebApplication1.Services.ControllerServices;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinancialServiceProviderController : ControllerBase
    {
        private readonly IFinancialServiceProviderService _bankService;

        public FinancialServiceProviderController(IFinancialServiceProviderService bankService)
        {
            _bankService = bankService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBank([FromBody] CreateFinancialServiceProviderDto createDto)
        {
            try
            {
                var result = await _bankService.CreateBankAsync(createDto);
                return CreatedAtAction(nameof(GetBanksForDropdown), new { locationId = createDto.LocationId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("/{financialServiceProvider}")]
        public async Task<IActionResult> UpdateBank(Guid financialServiceProvider, [FromBody] FinancialServiceProviderUpdateDto updateDto)
        {
            try
            {
                var result = await _bankService.UpdateBankAsync(financialServiceProvider, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{financialServiceProvider}")]
        public async Task<IActionResult> DeleteBank(Guid bankId)
        {
            try
            {
                await _bankService.DeleteBankAsync(bankId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetBanksForDropdown([FromQuery] Guid locationId, [FromQuery] GeneralStatus? status)
        {
            try
            {
                var result = await _bankService.GetBanksForDropdownAsync(locationId, status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{financialServiceProvider}/contact-persons")]
        public async Task<IActionResult> GetContactPersons(Guid bankId, [FromQuery] string? filter, [FromQuery] GeneralStatus? status)
        {
            try
            {
                var result = await _bankService.GetAllContactPersonsAsync(bankId, filter, status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> MakeDeposit([FromBody] CreateTransactionDto depositDto)
        {
            try
            {
                  await _bankService.MakeDepositAsync(depositDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}