namespace WebApplication1.Domain.Entities
{
    public class Currency
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Code { get; private set; }  
        public ICollection<Payment> Payments { get; private set; }
    }
}
