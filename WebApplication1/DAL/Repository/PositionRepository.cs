using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class PositionRepository : IPositionRepository
    {
        private readonly AppDbContext _context;
        public PositionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Position position)
        {
            await _context.Positions.AddAsync(position);
           // await _context.SaveChangesAsync();
        }

        public IQueryable<Position> GetAll()
        {
            return _context.Positions;
        }

        public async Task<Position?> GetByIdAsync(Guid id)
        {
            return await _context.Positions.Where(x=> x.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Position position)
        {
             _context.Positions.Update(position);
       //     await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Position position)
        {
            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();
        }
    }
}
