// Controllers/SalesController.cs
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Domain.Repository;
using WebApplication1.Domain.QueryFilters;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Entities;
using WebApplication1.DAL.Repository;
using WebApplication1.Domain.DTO;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISaleRepository _saleService;
        private readonly ILocationRepository _locationRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;

        public SalesController(
            ISaleRepository saleService,
            ILocationRepository locationRepository,
            ICustomerRepository customerRepository,
            IUserRepository userRepository,
            ITransactionRepository transactionRepository)
        {
            _saleService = saleService;
            _locationRepository = locationRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public async Task<ActionResult<GetSaleDto>> GetAll([FromQuery] BrowseSalesFilters filter)
        {
            var sales =  await _saleService.GetAllAsync(filter.LocationId, filter.GeneralStatus, filter.CustomerId, filter.SalesPersonId);
            return Ok(sales);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetSaleDto>> GetById(Guid id)
        {
            var x = await _saleService.GetByIdAsync(id);
            if (x == null)  return NotFound();

            var dataToReturn =  new GetSaleDto
            {
                Id = x.Id,
                CustomerName = $"{x.Customer.FirstName} {x.Customer.LastName}",
                CustomerId = x.CustomerId,
                Date = x.CreatedAt,
                LocationId = x.LocationId,
                LocationName = x.Location.Name,
                TaxAmount = x.Transaction.TaxAmount,
                DiscountAmount = x.Transaction.DiscountAmount,
                TotalAmount = x.Transaction.TotalAmount,
                Items = x.Transaction.TransactionItems.Select(x => new TransactionItemDto
                {
                    ItemId = x.Item.Id,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,  // Purchase Price
                    DeliveredQuantity = x.TransactionItemsDelivered.Sum(x => x.Quanity),
                    ItemName = x.Item.Name,
                    Code = x.Item.Code,
                    CostPrice = x.Item.CostPrice
                }).ToList(),
                Payments = x.Transaction.TransactionPayments.Select(x => new TransactionPaymentsDto
                {
                    Amount = x.Amount,
                    PaymentDate = x.PaymentDate,
                    PaymentMethod = x.PaymentMethod
                }).ToList()


            };
            return Ok(dataToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTransactionDto createDto)
        {
            
            try
            {
                if (!Enum.IsDefined(typeof(PaymentMethods), createDto.PaymentMethod)) return BadRequest("Invalid payment method");
                if (!Enum.IsDefined(typeof(TransactinType), createDto.TransactinType)) return BadRequest("Invalid transaction type");

                var location = await _locationRepository.GetByIdAsync(createDto.LocationId);
                if (location == null) BadRequest("Shop not found");

                if ( createDto.BusinessPartnerId != Guid.Empty)
                {
                    var customer = await _customerRepository.GetByIdAsync(createDto.BusinessPartnerId);
                    if (customer == null) BadRequest("Customer not found");
                }

                var user = await  _userRepository.GetUserByRefreshTokenAsync();
                var transNumber = await _saleService.CreateAsync(createDto, Guid.Parse(user.Id));

                createDto.TransactionCode = transNumber;
                return StatusCode(201,  createDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("ItemsDelivered")]
        //public async Task<IActionResult> GetAllDeliveredToDate([FromQuery] Guid TransactionId)
        //{
        //    var purchases = await _transactionRepository.GetAllDeliveredItemsToDate(TransactionId);
        //    return Ok(purchases);
        //}

        //[HttpPost("ConfirmDelivery")]
        //public async Task<ActionResult> CreatConfirmRecievalePayment([FromBody] ConfirmTransactionDeliveryDTO createDto)
        //{
        //    try
        //    {
        //        if (!Enum.IsDefined(typeof(PaymentMethods), createDto.PaymentMethod)) return BadRequest("Invalid payment metho");


        //        var user = await _userRepository.GetUserByRefreshTokenAsync();
        //        await _transactionRepository.DeliverItems(createDto, Guid.Parse(user.Id));

        //        return StatusCode(201);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPost("Cancel")]
        public async Task<IActionResult> Cancel([FromBody] CreateSaleCancellationDto cancellationDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var saleId = await _saleService.CancellationAsync(cancellationDto, userId);
                return CreatedAtAction(nameof(GetById), new { id = saleId }, cancellationDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSale( [FromBody] UpdateSaleDto updateDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var sale = await _saleService.DeleteAsync(updateDto.Id, userId, updateDto.Notes);
                if (sale == null)  return NotFound();
                return Ok(sale);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

   

        private Guid GetCurrentUserId()
        {
            var userIdClaim =  User.Claims.FirstOrDefault(c => c.Type == "userId");
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
        }
    }
}