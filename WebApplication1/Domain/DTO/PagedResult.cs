namespace WebApplication1.Domain.DTO
{
    public sealed record PagedResult<T>(
        IReadOnlyList<T> Items,
        int TotalCount,
        int? Page =1,
        int? PageSize =20)
    {
        public int TotalPages =>
            PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / (int)PageSize);

        public bool HasPrevious => Page > 1;
        public bool HasNext    => Page < TotalPages;
    }
}