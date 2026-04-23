using WebApplication1.Domain.Enums;

public class StockTakeFilterDto
{
    public GeneralStatus GenealStatus { get; set; }
    public Guid? LocationId { get; set; }
    public StockTakeStatus? Stage { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? ConductedBy { get; set; }
    public bool? HasVariance { get; set; }
    public string? SortBy { get; set; } = "date";
    public bool SortDescending { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public class StockTakingListDto
{
    public Guid Id { get; set; }
    public string LocationName { get; set; }
    public string Stage { get; set; }
    public DateTime StockTakingDate { get; set; }
    public string ConductedBy { get; set; }
    public string? VerifiedBy { get; set; }
    public int ItemCount { get; set; }
    public int TotalVariance { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class StockTakingDto
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public string? LocationName { get; set; }
    public string Stage { get; set; }
    public DateTime StockTakingDate { get; set; }
    public DateTime? NextStockTakingDate { get; set; }
    public string ConductedBy { get; set; }
    public string? VerifiedBy { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<StockTakingItemDto>? Items { get; set; }
    public List<CommentDto>? Comments { get; set; }
}

public class StockTakingItemDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string? ItemName { get; set; }
    public string? ItemCode { get; set; }
    public int ExpectedQuantity { get; set; }
    public int ActualQuantity { get; set; }
    public int VerifiedQuantity { get; set; }
    public int Variance { get; set; }
    public string? Stage { get; set; }
}

public class CommentDto
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class LocationStockTakeSummaryDto
{
    public Guid LocationId { get; set; }
    public string LocationName { get; set; }
    public int TotalStockTakes { get; set; }
    public int CompletedStockTakes { get; set; }
    public int ApprovedStockTakes { get; set; }
    public int DeclinedStockTakes { get; set; }
    public int InProgressStockTakes { get; set; }
    public DateTime? LastStockTakeDate { get; set; }
    public DateTime? NextScheduledDate { get; set; }
}

public class VarianceReportDto
{
    public Guid StockTakeId { get; set; }
    public string LocationName { get; set; }
    public DateTime StockTakeDate { get; set; }
    public string Stage { get; set; }
    public int TotalItemsCounted { get; set; }
    public int ItemsWithVariance { get; set; }
    public int TotalVarianceValue { get; set; }
    public int SurplusTotal { get; set; }
    public int ShortageTotal { get; set; }
    public List<VarianceItemDto> VarianceItems { get; set; }
}

public class VarianceItemDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; }
    public string ItemCode { get; set; }
    public int ExpectedQuantity { get; set; }
    public int ActualQuantity { get; set; }
    public int VerifiedQuantity { get; set; }
    public int Variance { get; set; }
    public string VarianceType { get; set; }
    public int VarianceValue { get; set; }
}

public class StockTakeHistoryDto
{
    public DateTime Date { get; set; }
    public string Action { get; set; }
    public string User { get; set; }
    public string Details { get; set; }
}