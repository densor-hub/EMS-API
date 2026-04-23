using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface IItemLocationRepository
    {
         Task AddRangeAsync(List<ItemLocation> itemLoactions);
         IQueryable <ItemLocation> GetAllByItemId(Guid item);
        Task DeleteRangeAsync(List<ItemLocation> itemLoactions);
        IQueryable<ItemLocation> GetAllByLocationId(Guid locId);
        Task ManageLocationAccess(List<Guid> locations, Guid itemId, Guid userId);
    }
}
