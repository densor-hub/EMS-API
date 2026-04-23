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
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseRepository _purchaseService;
        private readonly IUserRepository _userRepository;
        private readonly ISupplierLocationRepository _supplierLocationRepository;
        private readonly ITransactionRepository _transactionRepository;
        public PurchasesController(
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetAll([FromQuery] BrowsePurchasesFilters filters )
        {
            var purchases = await _purchaseService.GetAllAsync(filters.LocationId, filters.GeneralStatus, filters?.SupplierId);
            return Ok(purchases);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute]Guid id, [FromQuery] GeneralStatus generalStatus)
        {
            var purchase = await _purchaseService.GetByIdAsync(id, generalStatus);
            if (purchase == null)
                return NotFound();
            return Ok(purchase);
        }


        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateTransactionDto createDto)
        {
            try
            {
                if (!Enum.IsDefined(typeof(TransactinType), createDto.TransactinType)) return BadRequest("Invalid transaction type");

                var supplierLocationExisit = await _supplierLocationRepository.Exists(createDto.BusinessPartnerId, createDto.LocationId);
                if (!supplierLocationExisit) return BadRequest("Supplier does not have access to the shop selected");

                var user = await _userRepository.GetUserByRefreshTokenAsync();
                var itemCreatedId = await _purchaseService.CreateAsync(createDto, Guid.Parse(user.Id));

                return StatusCode(201, new { id = itemCreatedId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPost("Payment")]
        //public async Task<ActionResult> CreatePayment([FromBody] TransactionPaymentsDto createDto)
        //{
        //    try
        //    {
        //        if (!Enum.IsDefined(typeof(PaymentMethods), createDto.PaymentMethod)) return BadRequest("Invalid payment metho");


        //        var user = await _userRepository.GetUserByRefreshTokenAsync();
        //        var itemCreatedId = await _transactionRepository.AddPayment(createDto, Guid.Parse(user.Id));

        //        return StatusCode(201, new { id = itemCreatedId });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        //[HttpGet("ItemsRecieved")]
        //public async Task<IActionResult> GetAllRecievedToDate([FromQuery] Guid TransactionId)
        //{
        //    var purchases = await _transactionRepository.GetAllDeliveredItemsToDate(TransactionId);
        //    return Ok(purchases);
        //}



        //[HttpPost("ConfirmRecieval")]
        //public async Task<ActionResult> CreatConfirmRecievalePayment([FromBody] ConfirmTransactionDeliveryDTO createDto)
        //{
        //    try
        //    {
        //    //    if (!Enum.IsDefined(typeof(PaymentMethods), createDto.PaymentMethod)) return BadRequest("Invalid payment metho");


        //        var user = await _userRepository.GetUserByRefreshTokenAsync();
        //       await  _transactionRepository.DeliverItems(createDto, Guid.Parse(user.Id));

        //        return StatusCode(201);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

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
                return CreatedAtAction(nameof(GetById), new { id = cancellation.Id }, cancellation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}