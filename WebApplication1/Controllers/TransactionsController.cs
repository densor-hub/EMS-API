// Controllers/PurchasesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DAL.Repository;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.QueryFilters;
using WebApplication1.Domain.Repository;
using WebApplication1.DTOs;
using WebApplication1.Services;
using WebApplication1.Services.ControllerServices;
using WebApplication1.Services.TokenService;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionService _transactionService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICouponRepository _couponRepository;
       
        public TransactionsController(
            IPurchaseService purchaseService, 
            IUserRepository userRepository,
             ITransactionService transactionService,
             ICouponRepository couponRepository,
            ITransactionRepository transactionRepository
             )
        {
            _purchaseService = purchaseService;
            _userRepository = userRepository;
            _transactionService = transactionService;
            _couponRepository = couponRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<GetTransactionDto>> GetTransactionDetails([FromRoute] Guid id)
        {
            try
            {

                var user = await _userRepository.GetUserByRefreshTokenAsync();
               

                var paymentResults = await _transactionService.GetTransactionDetails(id);

                return paymentResults;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("Payment")]
        public async Task<ActionResult> CreatePayment([FromBody] TransactionPaymentsDto createDto)
        {
            try
            {
                if (!Enum.IsDefined(typeof(PaymentMethods), createDto.PaymentMethod)) return BadRequest("Invalid payment metho");

                var user = await _userRepository.GetUserByRefreshTokenAsync();
                var transaction = await _transactionRepository.GetByIdAsync(createDto.TransationId);
                if (transaction == null) return BadRequest("Transaction not found");

                 var paymentResults =  await _transactionService.AddPaymentExternalCallAsync(transaction, createDto, null);

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("ItemsDelivered")]
        public async Task<IActionResult> GetAllRecievedToDate([FromQuery] Guid TransactionId)
        {
            var purchases = await _transactionService.GetAllDeliveredItemsToDate(TransactionId);
            return Ok(purchases);
        }

        [HttpPost("Delivery")]
        public async Task<ActionResult<TransactionCreatedReturnDataDto>> DeliverItems([FromBody] ConfirmTransactionDeliveryDTO createDto)
        {
            try
            {
                //    if (!Enum.IsDefined(typeof(PaymentMethods), createDto.PaymentMethod)) return BadRequest("Invalid payment metho");


                var user = await _userRepository.GetUserByRefreshTokenAsync();
                var qrCode = await _transactionService.DeliverItems(createDto, user, null);

                return StatusCode(201, new TransactionCreatedReturnDataDto {QrCode = qrCode });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPut]
        //public async Task<IActionResult> Update([FromBody] UpdatePurchaseDto updateDto)
        //{
        //    try
        //    {
        //        var user = await _userRepository.GetUserByRefreshTokenAsync();
        //        var purchase = await _purchaseService.UpdateAsync(updateDto.PurchaseId, updateDto, Guid.Parse(user.Id));
        //        if (purchase == null)
        //            return NotFound();
        //        return Ok(purchase);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost("DeliveryRequest")]
        //public async Task<ActionResult> DeliveryRequest([FromBody] CreateDeliveryRequestDto createDto)
        //{
        //    try
        //    {
        //        //    if (!Enum.IsDefined(typeof(PaymentMethods), createDto.PaymentMethod)) return BadRequest("Invalid payment metho");


        //        var user = await _userRepository.GetUserByRefreshTokenAsync();
        //       var responseQrCode =  await _transactionService.DeliveryRequest(createDto.TransactionId);

        //        return StatusCode(201, new {QrCode = responseQrCode });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete([FromBody] UpdateSaleDto dto)
        //{
        //    try
        //    {
        //        var user = await _userRepository.GetUserByRefreshTokenAsync();
        //        var result = await _purchaseService.DeleteAsync(dto.Id, Guid.Parse(user.Id), dto.Notes);
        //        if (!result)
        //            return NotFound();
        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPost("Cancel")]
        public async Task<IActionResult> Create([FromBody] TransactionCancellationDto cancelDto)
        {
            try
            {

                 await _transactionService.CancelAsync(cancelDto);
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       


    }
}