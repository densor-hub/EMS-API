using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface ICompanyRepository
    {
        Task<Company> GetByIdAsync(Guid? id);
    }
}
