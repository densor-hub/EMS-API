using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class TransactionCodeRepository : ITransactionCodeRepository
    {
        private readonly AppDbContext _context;
        private static readonly ThreadLocal<Random> _threadRandom =
            new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        public TransactionCodeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateUniquePinAsync()
        {
            const int maxAttempts = 100;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Generate 9-digit PIN (100,000,000 to 999,999,999)
                int pinNumber = _threadRandom.Value.Next(100000000, 1000000000);
                string pin = pinNumber.ToString("D9"); // Ensures 9 digits with leading zeros

                if (!await CheckPinExistsAndValidAsync(pin))
                {
                    var pinCode = PinCode.CreateCardCode(Guid.NewGuid(), pin, DateTime.UtcNow);
                    await _context.TransactionCodes.AddAsync(pinCode);
                    await _context.SaveChangesAsync(); // Don't forget to save!

                    return FormatPinWithHyphens(pin);
                }
            }

            throw new InvalidOperationException("Unable to generate unique Code after multiple attempts");
        }

        public async Task<bool> CheckPinExistsAndValidAsync(string pin)
        {
            // Remove hyphens if they exist in the input (for searching)
            string cleanPin = pin.Replace("-", "");

            // Check if PIN exists and is still valid (not expired)
            var oneYearAgo = DateTime.UtcNow.AddYears(-1);

            return await _context.TransactionCodes
                .AnyAsync(p => p.Code == cleanPin); // Only consider PINs from the last year as "existing"
        }

        private string FormatPinWithHyphens(string pin)
        {
            if (pin.Length != 9)
                throw new ArgumentException("PIN must be 9 digits long");

            return $"{pin.Substring(0, 3)}-{pin.Substring(3, 3)}-{pin.Substring(6, 3)}";
        }

        // Optional: Helper method to validate user input
        public bool TryParsePin(string input, out string cleanPin)
        {
            cleanPin = input?.Replace("-", "").Replace(" ", "");
            return cleanPin?.Length == 9 && cleanPin.All(char.IsDigit);
        }
    }
}