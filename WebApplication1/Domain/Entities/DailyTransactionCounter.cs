using System.Diagnostics.Metrics;

namespace WebApplication1.Domain.Entities
{
    public class DailyTransactionCounter
    {
        public Guid Id { get; private  set; }
        public Guid LocationId { get; private set; }
        public DateTime CounterDate { get; private set; }
        public long Value { get; private set; }

        private DailyTransactionCounter()
        {
            
        }

        private DailyTransactionCounter(Guid id, Guid locationid, DateTime counterDate, long value)
        {
            Id= id; LocationId=locationid; CounterDate=counterDate; Value=value;
        }

        public static DailyTransactionCounter Create(Guid id, Guid locationid, DateTime counterDate, long value)
        => new DailyTransactionCounter(id, locationid, counterDate, value);

        public void IncreaseCount ()
        {
            Value++;
        }
    }
}
