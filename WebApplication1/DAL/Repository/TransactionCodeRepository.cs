using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Cryptography;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;
using WebApplication1.Helpers;

namespace WebApplication1.DAL.Repository
{
    public class TransactionCodeRepository : ITransactionCodeRepository
    {
        private readonly AppDbContext _context;
        private static readonly ThreadLocal<Random> _threadRandom =new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
        private readonly IUserRepository _userRepository;
        private static DateTime _lastCleanupDate = DateTime.MinValue;
        private static readonly SemaphoreSlim _cleanupLock = new SemaphoreSlim(1, 1);

        public TransactionCodeRepository(AppDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
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

        public string GenerateEntityCodeAsync( [Required] string type, int entityIncrementalNumber, int locationIncrementalNumber)
        {
            return $"{type}-{locationIncrementalNumber:D5}-{entityIncrementalNumber:D5}";
        }



        public async Task<string> GenerateTransactionCodeAsync(string type, Guid locationId)
        {
            // 1. Fast, lightweight validation (no full entity load or joins)
            var locationExists = await _context.Locations
                .AnyAsync(x => x.Id == locationId);

            if (!locationExists)
                throw new KeyNotFoundException($"Location with ID '{locationId}' not found.");

            // 2. Safe type prefix
            var cleanType = (type ?? "TXN").Trim();
            var typeCode = cleanType.Length >= 3
                ? cleanType.Substring(0, 3).ToUpper()
                : cleanType.PadRight(3, 'X').ToUpper();

            var today = DateTime.UtcNow;
            var timeStamp = today.ToString("yyMMdd");

            // 3. Atomic daily sequence
            var sequenceNumber = await GetDailySequenceAsync(locationId, today);

            // 4. Unguessable cryptographically secure suffix
            var randomSuffix = GetSecureRandomString(5);

            // Raw format: TYP + YYMMDD + 000001 + RANDOM (20 chars total)
            var rawCode = $"{typeCode}{timeStamp}{randomSuffix}{sequenceNumber:D6}";

            // 5. Chunk into 5-character segments (Output format: TYP26-08270-00001-K8X9P)
            return string.Join("-", Enumerable.Range(0, (int)Math.Ceiling(rawCode.Length / 5.0))
                .Select(i => rawCode.Substring(i * 5, Math.Min(5, rawCode.Length - i * 5))))
                .ToUpper();
        }

        private static string GetSecureRandomString(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
            }

            return new string(result);
        }

        private async Task<long> GetDailySequenceAsync(Guid locationId, DateTime date)
        {
            var dateOnly = date.Date;
            var today = DateTime.UtcNow.Date;

            // Cleanup (once per day)
            //if (_lastCleanupDate != today)
            //{
            //    await _cleanupLock.WaitAsync();
            //    try
            //    {
            //        if (_lastCleanupDate != today)
            //        {
            //            await _context.DailyTransactionCounters
            //                .Where(c => c.CounterDate < today)
            //                .ExecuteDeleteAsync();
            //            _lastCleanupDate = today;
            //        }
            //    }
            //    finally
            //    {
            //        _cleanupLock.Release();
            //    }
            //}

            // 1. Get advisory lock for this location+date
            var lockKey1 = locationId.GetHashCode();
            var lockKey2 = dateOnly.ToString("yyyyMMdd").GetHashCode();

            await _context.Database.ExecuteSqlRawAsync(
                "SELECT pg_advisory_xact_lock({0}, {1})", lockKey1, lockKey2);

            // 2. Now safely update the counter (serialized)
            var counter = await _context.DailyTransactionCounters
                .FirstOrDefaultAsync(c => c.LocationId == locationId && c.CounterDate == dateOnly);

            if (counter == null)
            {
                counter = DailyTransactionCounter.Create(Guid.NewGuid(), locationId, dateOnly, 1);
                await _context.DailyTransactionCounters.AddAsync(counter);
            }
            else
            {
                counter.IncreaseCount();
            }

            await _context.SaveChangesAsync();
            return counter.Value;

            // Advisory lock auto-released on transaction commit
        }
    }
}