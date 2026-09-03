using Microsoft.AspNetCore.Http.HttpResults;

namespace WebApplication1.Domain.Entities
{
    public class StockLockDownRequest
    {
        public Guid Id { get; private  set; }
        public DateTime TransactionDate { get; private  set; }
        public string TransactionNumber { get; private  set; }
        public Guid LocationId { get; private set; }
        public Location Location { get; private set; }
        public DateTime? TurnAroundTime { get; private  set; }
        public string CreatedById { get; private  set; } // Guid converted to string
        public ApplicationUser CreatedByUser { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public ICollection<StockLockDownItem> StockLockDownItems { get; private  set; } // Required
        public ICollection<StockLockDownComment> StockLockDownComments { get; private set; }
        


        private StockLockDownRequest()
        {
            
        }

        private StockLockDownRequest(Guid id, DateTime transactionDate, string transactionNumber, string createdby,Guid locationId, DateTime createdAt, DateTime? turnAroundTime )
        {
            Id = id;
            TransactionDate = transactionDate;
            TransactionNumber = transactionNumber;
            TurnAroundTime = turnAroundTime;
            CreatedById = createdby;
            CreatedAt = createdAt;
            LocationId = locationId;
        }

        public static StockLockDownRequest Create(Guid id, DateTime transactionDate, string transactionNumber, string createdby,  Guid locationId, DateTime createdAt, DateTime? turnAroundTime)
        => new StockLockDownRequest(id,  transactionDate, transactionNumber, createdby,  locationId, createdAt, turnAroundTime);
    }
}
