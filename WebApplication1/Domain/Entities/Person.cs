namespace WebApplication1.Domain.Entities
{
    public abstract class Person : BaseEntity
    {
        public  string FirstName { get; set; }
        public  string LastName { get; set; }
        public  string Phone { get; set; }
        public string? Email { get; set; }
        public  string Address { get; set; }
        public bool IsAppUser { get; set; } = false;
    }
}
