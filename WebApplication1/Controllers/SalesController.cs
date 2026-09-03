// Controllers/SalesController.cs
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Domain.Repository;
using WebApplication1.Domain.QueryFilters;
using WebApplication1.Domain.Enums;
using WebApplication1.Services.ControllerServices;
using WebApplication1.Domain.DTO;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;
        private readonly ILocationRepository _locationRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransactionService _transactionService;

        public SalesController(
            ISaleService saleService,
            ILocationRepository locationRepository,
            ICustomerRepository customerRepository,
            IUserRepository userRepository,
            ITransactionRepository transactionRepository,
            ITransactionService transactionService)
        {
            _saleService = saleService;
            _locationRepository = locationRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<ActionResult<GetSaleDto>> GetAll([FromQuery] BrowseSalesFilters filter)
        {
            var sales =  await _saleService.GetAllAsync(filter.LocationId, filter.GeneralStatus, filter.Type, filter.CustomerId, filter.SalesPersonId);
            return Ok(sales);
        }

        [HttpGet("Generate-Receipt/{deliveryRequestId}")]
        public async Task<ActionResult<GetSaleDto>> GenerateReceipt([FromRoute] string deliveryRequestId)
        {
            try
            {
                var validGuid = Guid.TryParse(deliveryRequestId, out var id);

                
                var sales = await _saleService.GenerateReceipt(validGuid ? id : Guid.Empty, deliveryRequestId );
                return Ok(sales);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
   
        }


        [HttpPost("General")]
        public async Task<ActionResult<TransactionCreatedReturnDataDto>> Create([FromBody] CreateTransactionDto createDto)
        {
            
            try
            {
                if (!Enum.IsDefined(typeof(PaymentMethods), createDto.PaymentMethod)) return BadRequest("Invalid payment method");
                if (!Enum.IsDefined(typeof(TransactionResultsType), createDto.TransactionResultsType)) return BadRequest("Invalid transaction type");

                var location = await _locationRepository.GetByIdAsync(createDto.LocationId);
                if (location == null) BadRequest("Shop not found");

                //this endpoint is only used for general sale where customers are not in the system
                createDto.BusinessPartnerId = Guid.Empty;
                createDto.TransactionResultsType = TransactionResultsType.Deposit;

                var user = await  _userRepository.GetUserByRefreshTokenAsync();


                //use configuration to select which best fits
               var results =  await _transactionService.CompleteTransationProcess(createDto, TransactionResultsType.Deposit); // Deposit becuase initail payment for QrCode, debit is done when Stock Person generates receipt

                return StatusCode(201,  results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Customer/{customerId}")]
        public async Task<ActionResult<TransactionCreatedReturnDataDto>> CreateCustomerSale([FromRoute] Guid customerId, [FromBody] CreateTransactionDto createDto)
        {
            try
            {
                if (!Enum.IsDefined(typeof(PaymentMethods), createDto.PaymentMethod)) return BadRequest("Invalid payment method");
                if (!Enum.IsDefined(typeof(TransactionResultsType), createDto.TransactionResultsType)) return BadRequest("Invalid transaction type");

                var location = await _locationRepository.GetByIdAsync(createDto.LocationId);
                if (location == null) BadRequest("Shop not found");

                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null) BadRequest("Customer not found");

                var user = await _userRepository.GetUserByRefreshTokenAsync();

                //every customer firstly has to deposit before delivery will be made our of that deposit
                createDto.TransactionResultsType = TransactionResultsType.Deposit;
                createDto.BusinessPartnerId = customerId;

                var results = await _transactionService.CompleteTransationProcess(createDto, TransactionResultsType.Deposit);

                return StatusCode(201, results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}