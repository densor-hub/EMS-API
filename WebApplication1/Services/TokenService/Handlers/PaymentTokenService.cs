using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Services.TokenService.Handlers
{
    public class PaymentTokenService : IPaymentTokenService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PaymentTokenService> _logger;
        private readonly TokenConfiguration _config;

        public PaymentTokenService(
            AppDbContext dbContext,
            ILogger<PaymentTokenService> logger,
            IOptions<TokenConfiguration> config)
        {
            _dbContext = dbContext;
            _logger = logger;
            _config = config.Value;
        }



        public async Task<ApprovalTokenResult> GenerateTokenAsync( string userEmail, Payment payment)
        {

            if (string.IsNullOrEmpty(userEmail)) { throw new Exception("Email required"); }
            // Generate a cryptographically secure token
            var tokenString = GenerateSecureToken();

            // Set expiration (default: 30 minutes)
            var expirationMinutes = _config.TokenExpirationMinutes ?? 30;
            var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

            // Create token record
            var approvalToken = PaymentConfirmationToken.Create(Guid.NewGuid(), payment.Id, tokenString, DateTime.UtcNow, expiresAt, userEmail);

            // Revoke any existing tokens for this transaction
            await RevokeExistingTokensAsync(payment.Id);

            // Save to database
            _dbContext.PaymentConfirmationTokens.Add(approvalToken);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Generated approval token for transaction {paymentId}", payment.Id);

            return new ApprovalTokenResult
            {
                Token = tokenString,
                ExpiresAt = expiresAt,
                ApprovalUrl = $"{_config.BaseUrl}/approve?token={Uri.EscapeDataString(tokenString)}"
            };
        }

        public async Task<TokenValidationResult> ValidateAndUseTokenAsync(
            string token,
            string ipAddress,
            string userAgent)
        {
            var result = new TokenValidationResult();

            if (string.IsNullOrWhiteSpace(token))
            {
                result.IsValid = false;
                result.ErrorMessage = "Token is required";
                return result;
            }

            // Find the token in the database
            var approvalToken = await _dbContext.PaymentConfirmationTokens
                .Include(t => t.Payment)
                .FirstOrDefaultAsync(t => t.Token == token);

            if (approvalToken == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "Invalid token";
                return result;
            }

            // Check if token is revoked
            if (approvalToken.IsRevoked)
            {
                result.IsValid = false;
                result.ErrorMessage = "This token has been revoked";
                _logger.LogWarning("Attempted to use revoked token: {Token}", token);
                return result;
            }

            // Check if token is already used
            if (approvalToken.IsUsed)
            {
                result.IsValid = false;
                result.ErrorMessage = "This token has already been used";
                _logger.LogWarning("Attempted to reuse token: {Token}", token);
                return result;
            }

            // Check if token has expired
            if (DateTime.UtcNow > approvalToken.ExpiresAt)
            {
                result.IsValid = false;
                result.ErrorMessage = "This token has expired";
                _logger.LogWarning("Attempted to use expired token: {Token}", token);
                return result;
            }

            // Token is valid - mark it as used
            approvalToken.ApplyToken(ipAddress, userAgent);

            await _dbContext.SaveChangesAsync();

            result.IsValid = true;
            result.PaymentId = approvalToken.PaymentId;
           // result.Disbursement = approvalToken.Disbursement;

            _logger.LogInformation("Token validated successfully for transaction {paymentId}",
                approvalToken.PaymentId);

            return result;
        }

        private string GenerateSecureToken()
        {
            // Generate 32 bytes (256 bits) of cryptographically secure random data
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            // Convert to URL-safe base64 string
            return Convert.ToBase64String(randomBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        private async Task RevokeExistingTokensAsync(Guid paymentId)
        {
            var existingTokens = await _dbContext.PaymentConfirmationTokens
                .Where(t => t.PaymentId == paymentId && !t.IsUsed && !t.IsRevoked)
                .ToListAsync();

            foreach (var token in existingTokens)
            {
                token.RevokeToken();
            }

            if (existingTokens.Any())
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Revoked {Count} existing tokens for transaction {paymentId}",
                    existingTokens.Count, paymentId);
            }
        }
    }
}
