// Controllers/PurchasesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DAL.Repository;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.QueryFilters;
using WebApplication1.Domain.Repository;
using WebApplication1.DTOs;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly IPurchaseRepository _purchaseService;
        private readonly IUserRepository _userRepository;
        private readonly ISupplierLocationRepository _supplierLocationRepository;
        private readonly ITransactionRepository _transactionRepository;
        public TransactionsController(
            IPurchaseRepository purchaseService, 
            IUserRepository userRepository,
             ISupplierLocationRepository supplierLocationRepository,
             ITransactionRepository transactionRepository)
        {
            _purchaseService = purchaseService;
            _userRepository = userRepository;
            _supplierLocationRepository = supplierLocationRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPost("Payment")]
        public async Task<ActionResult> CreatePayment([FromBody] TransactionPaymentsDto createDto)
        {
            try
            {
                if (!Enum.IsDefined(typeof(PaymentMethods), createDto.PaymentMethod)) return BadRequest("Invalid payment metho");


                var user = await _userRepository.GetUserByRefreshTokenAsync();
                var itemCreatedId = await _transactionRepository.AddPayment(createDto, Guid.Parse(user.Id));

                return StatusCode(201, new { id = itemCreatedId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("ItemsDelivered")]
        public async Task<IActionResult> GetAllRecievedToDate([FromQuery] Guid TransactionId)
        {
            var purchases = await _transactionRepository.GetAllDeliveredItemsToDate(TransactionId);
            return Ok(purchases);
        }

        [HttpPost("ConfirmDelivery")]
        public async Task<ActionResult> CreatConfirmRecievalPayment([FromBody] ConfirmTransactionDeliveryDTO createDto)
        {
            try
            {
            //    if (!Enum.IsDefined(typeof(PaymentMethods), createDto.PaymentMethod)) return BadRequest("Invalid payment metho");


                var user = await _userRepository.GetUserByRefreshTokenAsync();
               await  _transactionRepository.DeliverItems(createDto, Guid.Parse(user.Id));

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdatePurchaseDto updateDto)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                var purchase = await _purchaseService.UpdateAsync(updateDto.PurchaseId, updateDto, Guid.Parse(user.Id));
                if (purchase == null)
                    return NotFound();
                return Ok(purchase);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromBody] UpdateSaleDto dto)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                var result = await _purchaseService.DeleteAsync(dto.Id, Guid.Parse(user.Id), dto.Notes);
                if (!result)
                    return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Cancel")]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseCancellationDto cancelDto)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                var cancellation = await _purchaseService.CancelAsync(cancelDto, Guid.Parse(user.Id));
                return CreatedAtAction("CancelTransaction", new { id = cancellation.Id }, cancellation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}