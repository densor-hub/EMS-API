// Controllers/BankController.cs
// Controllers/BankController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs;
using WebApplication1.Services.ControllerServices;

namespace WebApplication1.Controllers
{
     [ApiController]
    [Route("[controller]")]
    [Authorize]
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

        [HttpPut("{financialServiceProvider}")]
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
        public async Task<IActionResult> DeleteBank([FromRoute]Guid financialServiceProvider)
        {
            try
            {
                await _bankService.DeleteBankAsync(financialServiceProvider);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{locationId:Guid}")]
        public async Task<ActionResult<FinancialServiceProviderResponseDto>> GetAllBanksPerLocattion([FromRoute] Guid locationId, [FromQuery] GeneralStatus? status)
        {
            try
            {
                var result = await _bankService.GetFinancialServiceProviders(locationId, status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("dropdown")]
        public async Task<ActionResult<FinancialServiceProviderDropdownDto>> GetBanksForDropdown([FromQuery] Guid locationId, [FromQuery] GeneralStatus? status)
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
        public async Task<ActionResult<FinancialServiceProviderContactPersonResponseDto>> GetContactPersons(Guid bankId, [FromQuery] string? filter, [FromQuery] GeneralStatus? status)
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
        public async Task<IActionResult> MakeDeposit([FromBody] CreateFinancialServiceDisbursementDTO depositDto)
        {
            try
            {
                  await _bankService.Disbursement(depositDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("deposits/{locationId:Guid}")]
        public async Task<ActionResult<List<GetFinancialServiceDisbursementDTO>>> GetDeposits([FromRoute] Guid locationId, [FromQuery] Guid financialServiceProvider, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var data = await _bankService.GetDisbursements(locationId, financialServiceProvider, startDate, endDate );
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}