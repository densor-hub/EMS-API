using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface IPositionRepository
    {
        IQueryable<Position> GetAll();
        Task<Position?> GetByIdAsync(Guid id);
        Task UpdateAsync(Position position);
        Task AddAsync(Position position);
        Task DeleteAsync(Position position);
    }
}
