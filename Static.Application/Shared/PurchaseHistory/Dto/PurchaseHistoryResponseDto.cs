namespace Static.Application.Shared.PurchaseHistory.Dto;

public record PurchaseHistoryResponseDto
{
    public string ProductName { get; init; }

    public Guid PurchaseHistoryId { get; init; }

    public Guid PackageTermId { get; init; }
    public decimal Amount { get; init; }
}