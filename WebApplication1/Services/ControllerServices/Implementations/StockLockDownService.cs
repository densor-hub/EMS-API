using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Controllers;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repositories;
using WebApplication1.Domain.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;
//using WebApplication1.Exceptions; // You'll need to create this

namespace WebApplication1.Services
{
    public class StockLockDownService : IStockLockDownService
    {
        private readonly IStockLockDownRequestRepository _requestRepository;
        private readonly IStockLockDownItemRepository _itemRepository;
        private readonly IStockTakeItemSubmissionRepository _submissionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionCodeRepository _transactionCodeRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly AppDbContext _context;

        public StockLockDownService(
            IStockLockDownRequestRepository requestRepository,
            IStockLockDownItemRepository itemRepository,
            IStockTakeItemSubmissionRepository submissionRepository,
            IUserRepository userRepository,
            ITransactionCodeRepository transactionCodeRepository,
            ILocationRepository locationRepository,
            AppDbContext context
            )
        {
            _requestRepository = requestRepository ?? throw new ArgumentNullException(nameof(requestRepository));
            _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
            _submissionRepository = submissionRepository ?? throw new ArgumentNullException(nameof(submissionRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _transactionCodeRepository = transactionCodeRepository ?? throw new ArgumentNullException(nameof(transactionCodeRepository));
            _locationRepository = locationRepository ?? throw new ArgumentNullException(nameof(locationRepository));
        }

        public async Task<StockLockDownRequest> CreateStockLockDownWithItemsAsync(CreateStockLockDownRequestDto createDto)
        {
            
            // Save everything in a transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                var user = await _userRepository.GetUserByRefreshTokenAsync();

                if (user == null) throw new Exception("Invalid request, user not found");


                if (createDto.ItemIds == null || !createDto.ItemIds.Any())
                    throw new ArgumentException("At least one item is required", nameof(createDto.ItemIds));

                var location = await _locationRepository.GetByIdAsync(createDto.LocationId);
                if (location == null) throw new Exception("Shop not found");
                // Check for duplicate items
                var duplicateItems = createDto.ItemIds.GroupBy(x => x)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateItems.Any())
                    throw new InvalidOperationException($"Duplicate items found: {string.Join(", ", duplicateItems)}");

                // Check if any items already have pending submissions
                var invalidItems = GetItemsWithPendingSubmissionsAsync(createDto.ItemIds);

                if (invalidItems.Any())
                {
                    var invalidItemNames = string.Join(", ", invalidItems.Select(i => i.Item.Name));
                    throw new InvalidOperationException(
                        $"The following items have pending submissions: {invalidItemNames}");
                }

                // Create the request

                var transactionNumber = await _transactionCodeRepository.GenerateTransactionCodeAsync("STK", createDto.LocationId);
                var request = StockLockDownRequest.Create(
                    Guid.NewGuid(),
                     DateTime.SpecifyKind(createDto.TransactionDate, DateTimeKind.Utc),
                     transactionNumber,
                     user.Id,
                     location.Id,
                     DateTime.UtcNow,
                     createDto.TurnAroundTime.HasValue ? DateTime.SpecifyKind(createDto.TurnAroundTime.Value, DateTimeKind.Utc) : null
                );

                // Create stock lock down items
                var items = createDto.ItemIds.Select(itemId =>
                    StockLockDownItem.Create(
                        Guid.NewGuid(),
                        itemId,
                        request.Id,
                        Guid.Parse(user.Id),
                        request.CreatedAt
                    )
                ).ToList();
                await _requestRepository.AddAsync(request);
                await _itemRepository.AddRangeAsync(items);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return request;

            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

           
        }

        //2. GET endpoint - Get StockLockDownRequests by LocationId with all related data
        public async Task<IEnumerable<GetAllStockLockDownDto>> GetStockLockDownRequestsByLocationAsync(GetStockLockDownQuery query)
        {

            var requests =  _requestRepository.GetAllAsync(query.LocationId, query.StartDate, query.EndDate);

            if (requests == null || !requests.Any())
                return Enumerable.Empty<GetAllStockLockDownDto>();

            return await requests.Select(x=> new GetAllStockLockDownDto
            {
                Id = x.Id,
                LocationId = x.LocationId,
                LocationName = x.Location.Name,
                TransactionDate = x.TransactionDate,
                SystemCreationDate = x.CreatedAt,
                TransactionNumber = x.TransactionNumber,
                TurnAroundTime= x.TurnAroundTime 
              
            }).ToListAsync();
        }

        //2.1 GET endpoint - Get Single StockLockDownRequests by Id with all related data
        public async Task<GetStockLockDownByIdDto> GetStockLockDownRequestsByIdAsync(Guid id)
        {

            var x = await _requestRepository.GetByIdAsync(id);

            if (x == null)
                return new GetStockLockDownByIdDto();

            return new GetStockLockDownByIdDto
            {
                Id = x.Id,
                LocationId = x.LocationId,
                LocationName = x.Location.Name,
                TransactionDate = x.TransactionDate,
                SystemCreationDate = x.CreatedAt,
                TransactionNumber = x.TransactionNumber,
                TurnAroundTime = x.TurnAroundTime,
                Items = x.StockLockDownItems.Select(sldi => new StockLockDownItemDto
                {
                    Id = sldi.Id,
                    IsEscalated = sldi.IsDeleted,
                    ItemCode = sldi.Item.Code,
                    ItemId = sldi.ItemId,
                    ItemName = sldi.Item.Name,
                    QuantityPerUnitOfMeasure = sldi.Item.QuantityInUnit,
                    UnitOfMeasure = sldi.Item.UnitOfMeasure.ToString(),
                    Submissions = sldi.Submissions.Any() ? sldi.Submissions.Select(subs => new GetStockLockDownSubmittedItemDto
                    {
                        Id = subs.Id,
                        PhysicalUnitOfMeasureQuantity = subs.PhysicalUnitOfMeasureQuantity,
                        PhysicalAdditionalPiecesQuantity = subs.PhysicalAdditionalPiecesQuantity,
                        Status = subs.Status.ToString(),
                        SystemAvailableQuantity = subs.SystemAvailableQuantity,
                        TotalInPieces = (subs.PhysicalUnitOfMeasureQuantity * sldi.Item.QuantityInUnit) + subs.PhysicalAdditionalPiecesQuantity,
                        TotalVariance =  ((subs.PhysicalUnitOfMeasureQuantity * sldi.Item.QuantityInUnit) + subs.PhysicalAdditionalPiecesQuantity) - subs.SystemAvailableQuantity
                    }).ToList() : new List<GetStockLockDownSubmittedItemDto>()

                }).ToList()
            };
        }

        //2.3 GET endpoint - Get StockLockDownRequests by LocationId with all related data
        public async Task<IEnumerable<GetStockLockDownSubmittedItemDto>> GetSubmittedStockLockedItems(GetStockLockDownQuery query)
        {

            var data = _submissionRepository.GetAllAsync();

            if (data == null || !data.Any())
                return Enumerable.Empty<GetStockLockDownSubmittedItemDto>();

            data = data.Where(x => x.StockLockDownItem.StockLockDownRequest.LocationId == query.LocationId);


            if (query.StartDate.HasValue && query.StartDate != DateTime.MinValue)
            {
                var specifiedStartDate = DateTime.SpecifyKind(query.StartDate.Value, DateTimeKind.Utc);

                data = data.Where(x => x.CreatedAt >= specifiedStartDate);
            }

            if (query.EndDate.HasValue && query.EndDate != DateTime.MinValue)
            {
                var specifiedEndateDate = DateTime.SpecifyKind(query.EndDate.Value, DateTimeKind.Utc);
                data = data.Where(x => x.CreatedAt <= specifiedEndateDate);
            }

            return await data.OrderByDescending(x=> x.CreatedAt).Select(x => new GetStockLockDownSubmittedItemDto
            {
                Id = x.Id,
                SystemAvailableQuantity = x.SystemAvailableQuantity,
                PhysicalAdditionalPiecesQuantity = x.PhysicalAdditionalPiecesQuantity,
                PhysicalUnitOfMeasureQuantity = x.PhysicalUnitOfMeasureQuantity,
                QuantityPerUnitOfMeasure = x.QuanityPerUnitOfMeasure,
                Status = x.Status.ToString(),
                SubmittedBy = x.CreatedByUser.FullName,
                UnitOfMeasure = x.StockLockDownItem.Item.UnitOfMeasure.ToString(),
                TotalVariance = x.Variance,
                TotalInPieces = (x.PhysicalUnitOfMeasureQuantity * x.QuanityPerUnitOfMeasure) + x.PhysicalAdditionalPiecesQuantity
            }).ToListAsync();
        }

        //3. POST endpoint - Save a single StockTakeItemSubmission
        public async Task<StockTakeItemSubmission> SubmitStockTakeItem(CreateStockSubmissionDto createDto
           )
        {
           
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null) throw new Exception("User not found");

                if (createDto.PhysicalUnitOfMeasureQuantity < 0 && createDto.PhysicalAdditionalPiecesQuantity < 0)
                    throw new ArgumentException("Physical available quantity cannot be negative");


                // Get the stock lock down item with its request
                var stockLockDownItem = await _itemRepository.GetByIdAsync(createDto.StockLockDownItemId);
                if (stockLockDownItem == null)
                    throw new Exception($"Stock lock down item with not found");

                // Check if submission already exists for this item
                var existingSubmission = stockLockDownItem.Submissions.Where(x => x.Status == StocktakeSubmissionStatusEnum.Pending);

                if (existingSubmission.Any())
                    throw new InvalidOperationException($"Submission already exists for the item submited");

                // Get system available quantity from StockLevels
                var locationId = stockLockDownItem.StockLockDownRequest.LocationId;
                var itemId = stockLockDownItem.ItemId;

                var stockLevel = await _context.StockLevels.FirstOrDefaultAsync(x => x.LocationId == locationId && x.ItemId == itemId);

                if (stockLevel == null)
                {
                    stockLevel = StockLevel.Create(Guid.NewGuid(), 0, 0, itemId, locationId);
                    await _context.StockLevels.AddAsync(stockLevel);
                }
                // throw new Exception($"Stock level not found for the item submitted at location.");

                var systemAvailableQuantity = stockLevel.AvailableQuanity;

                var submission = StockTakeItemSubmission.Create(
                      Guid.NewGuid(),
                      stockLockDownItem.Id,
                      StocktakeSubmissionStatusEnum.Pending,
                      DateTime.UtcNow,
                      user.Id,
                      systemAvailableQuantity,
                      createDto.PhysicalUnitOfMeasureQuantity,
                      stockLockDownItem.Item.QuantityInUnit,
                      createDto.PhysicalAdditionalPiecesQuantity,
                      ((createDto.PhysicalUnitOfMeasureQuantity * stockLockDownItem.Item.QuantityInUnit) + createDto.PhysicalAdditionalPiecesQuantity) - systemAvailableQuantity
                  );

                // Create the submission
                var result = await _submissionRepository.AddAsync(submission);


                //create comment
                if (string.IsNullOrEmpty(createDto.Remarks))
                {
                    var comment = StockLockDownComment.Create(Guid.NewGuid(), stockLockDownItem.StockLockDownRequestId, "STK", StocktakeSubmissionStatusEnum.Pending.ToString(), createDto.Remarks, DateTime.UtcNow, user.Id);
                    await _context.StockLockDownComments.AddAsync(comment);

                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return submission;

            }
            catch (Exception ex) {
                await transaction.CommitAsync();

                throw;
            }
           

            // Return the created submission with includes
        }

        //3. POST endpoint - Save a single StockTakeItemSubmission
        public async Task<StockTakeItemSubmission> UpdateStockTakeItem(UpdateStockTakeSubmissionDto createDto
           )
        {

            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
            if (user == null) throw new Exception("User not found");

            var allowedStatus = new List<int> { (int)StocktakeSubmissionStatusEnum.Approved, (int)StocktakeSubmissionStatusEnum.Declined };
            if (allowedStatus.Contains((int)createDto.status) == false) throw new Exception("Invalid status detected");

            // Get the stock lock down item with its request
            var stockSubmission = await _submissionRepository.GetByIdAsync(createDto.SubmissionId);
            if (stockSubmission == null)
                throw new Exception($"Stock lock down item with not found");

          
            // Check if submission already exists for this item
           // var existingSubmission = stockLockDownItem.Submissions.Where(x => x.Status == StocktakeSubmissionStatusEnum.Pending);

            if (stockSubmission.Status != StocktakeSubmissionStatusEnum.Pending)
                throw new InvalidOperationException($"The submitted record is not in Pending state, hence tranaction cannot proceed");

            var locationId = stockSubmission.StockLockDownItem.StockLockDownRequest.LocationId;
            var itemId = stockSubmission.StockLockDownItem.ItemId;
            var totalQuantityInPieces = (stockSubmission.PhysicalUnitOfMeasureQuantity * stockSubmission.StockLockDownItem.Item.QuantityInUnit) + stockSubmission.PhysicalAdditionalPiecesQuantity;//stockSubmission.PhysicalAvailableQuantity;

            var stockLevel = await _context.StockLevels.FirstOrDefaultAsync(x => x.LocationId == locationId && x.ItemId == itemId);

            if (stockLevel == null)
                throw new Exception($"Stock level not found for the item submitted at location.");

           

                if (createDto.status == StocktakeSubmissionStatusEnum.Approved)
                {
                    //update stock
                    stockLevel.AddActual(totalQuantityInPieces);
                    stockLevel.AddAvailable(totalQuantityInPieces);
                    _context.StockLevels.Update(stockLevel);

                    //update stocklocksubmission
                    stockSubmission.Approve(user.Id);
                    await _submissionRepository.UpdateAsync(stockSubmission);
                }

                if (createDto.status == StocktakeSubmissionStatusEnum.Declined)
                {
                    //update stocklocksubmission
                    stockSubmission.Decline(user.Id);
                    await _submissionRepository.UpdateAsync(stockSubmission);
                }


                 //save remarks
                 if (string.IsNullOrEmpty(createDto.Remarks))
                {
                    var comment = StockLockDownComment.Create(Guid.NewGuid(), stockSubmission.StockLockDownItem.StockLockDownRequestId, "STK", createDto.status.ToString().ToString(), createDto.Remarks, DateTime.UtcNow, user.Id);
                    await _context.StockLockDownComments.AddAsync(comment);

                }

                await transaction.CommitAsync();
                await _context.SaveChangesAsync();


                return stockSubmission;


            }
            catch (Exception ex) {
                await transaction.RollbackAsync();
                throw;
            }


        }

        public async Task<IEnumerable<StockTakeItemSubmission>> GetSubmissionsByStockLockDownItemAsync(Guid stockLockDownItemId)
        {
            var itemExists = await _itemRepository.ExistsAsync(stockLockDownItemId);
            if (!itemExists)
                throw new Exception($"Stock lock down item with not found");

            return await _submissionRepository.GetByStockLockDownItemIdAsync(stockLockDownItemId);
        }

       
        public IQueryable<StockLockDownItem> GetItemsWithPendingSubmissionsAsync(List<Guid> itemIds)
        {
            var invalidItems = new List<Guid>();

            // Get all stock lock down items for these item IDs
            var stockLockDownItems = _context.StockLockDownItems
                .Include(x=> x.Item)
                .Include(x => x.Submissions)
                .Where(x => itemIds.Contains(x.ItemId) 
                            && (!x.Submissions.Any() || x.Submissions.Any(x => x.Status == StocktakeSubmissionStatusEnum.Pending))
                );

            return stockLockDownItems;
        }

        public async Task<IEnumerable<CommentResponseDto>> GetComments(Guid StockLockDownRequestId)
        {
            var comments =  _context.StockLockDownComments
                .Where(x=> x.StockLockDownRequestId == StockLockDownRequestId) ;

            return await comments.Select(x => new CommentResponseDto
            {
                CreatedAt = x.CreatedAt,
                Comment = x.Comment,
                CreatedBy = x.CreatedBy.FullName,
                Id = x.Id,
                Stage = x.Stage,
                TransactionType = x.TransactionType
            }).OrderByDescending(X=> X.CreatedAt)
            .ToListAsync();
        }
    }
}