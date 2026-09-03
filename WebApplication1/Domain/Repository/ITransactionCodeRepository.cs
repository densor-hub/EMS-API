
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface ITransactionCodeRepository
    {
        Task<string> GenerateUniquePinAsync();
        Task<bool> CheckPinExistsAndValidAsync(string pin);
        string GenerateEntityCodeAsync([Required] string type, int entityIncrementalNumber, int locationIncrementalNumber);
        Task<string> GenerateTransactionCodeAsync(string type, Guid locationId);
        //Task<string> GenerateSaleItemCode(string type, Guid locationId);

    }
}
