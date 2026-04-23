using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AppDbContext _context;
        public CompanyRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Company> GetByIdAsync(Guid? id)
        {
            return await _context.Companies.Where(x=> x.Id == id).FirstOrDefaultAsync();
        }
    }
}
