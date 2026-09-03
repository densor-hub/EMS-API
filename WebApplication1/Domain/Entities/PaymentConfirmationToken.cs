using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Domain.Entities
{
    public class PaymentConfirmationToken
    {
        public Guid Id { get; private set; }
        public Guid PaymentId { get; private set; }
        public Payment Payment { get; private set; }
        public string Token { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; }
        public bool IsRevoked { get; private set; }
        public string UserEmail { get; private set; }
        public string IpAddress { get; private set; }
        public string UserAgent { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public PaymentConfirmationToken()
        {

        }

        public PaymentConfirmationToken(Guid id, Guid paymentId, string tokenString, DateTime createdAt, DateTime expiresAt, string userEmail)
        {
            Id = id;
            PaymentId = paymentId;
            Token = tokenString;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            IsUsed = false;
            IsRevoked = false;
            UserEmail = userEmail;
        }

        public static PaymentConfirmationToken Create(Guid id, Guid paymentId, string tokenString, DateTime createdAt, DateTime expiresAt, string userEmail)
        => new PaymentConfirmationToken(id, paymentId, tokenString, createdAt, expiresAt, userEmail);

        public void ApplyToken(string ipAddress, string userAgent)
        {
            IpAddress = ipAddress;
            UserAgent = userAgent;
            UpdatedAt = DateTime.UtcNow;
            IsUsed = true;
        }

        public void RevokeToken()
        {
            IsRevoked = true;
            UpdatedAt = DateTime.UtcNow;
        }


    }

}
