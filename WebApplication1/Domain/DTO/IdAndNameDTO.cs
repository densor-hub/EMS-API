namespace WebApplication1.Domain.DTO
{
    public class IdAndNameDTO
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }

    public class IdAndNameQtyDTO
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
