using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    // ---- Requests ----

    public record CreateItemSplitSetupRequest(
        Guid SourceItemId,
        Guid TargetItemId,
        int SourceQuantity,
        int TargetQuantity,
        Guid LocationId);

    public record UpdateItemSplitSetupRequest(
        int TargetQuantity,
        GeneralStatus GeneralStatus);

    // ---- Response ----

    public record ItemSplitSetupDto(
        Guid Id,
        Guid SourceItemId,
        string SourceItemName,
        Guid TargetItemId,
        string TargetItemName,
        int SourceQuantity,
        int TargetQuantity,
        GeneralStatus GeneralStatus
        );

    public record CreateItemSplitTransactionRequest(
        Guid SourceItemId,
        Guid TargetItemId,
        Guid LocationId
     );

    public record ItemSplitTransactionDto(
        Guid Id,
        Guid ItemSplitSetupId,
        Guid SourceItemId,
        string SourceItemName,
        Guid TargetItemId,
        string TargetItemName,
        int SourceQuantity,
        int TargetQuantity,
        int TargetQuantitySnapShot,
        string CreatedById,
        DateTime CreatedAt
      );
}