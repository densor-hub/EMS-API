namespace WebApplication1.Domain.Entities
{
    public class ConfirmationCodes : BaseEntity
    {
        public string Code { get; set; }
        public bool Used { get; set; }
        public Guid? AllowedUser { get; set; }
    }
}
