using Static.Helper.Constants;

namespace Static.Application.Shared.PurchaseHistory.Dto;

public record PurchaseHistoryRequestDto
{
    public Guid UserId { get; set; }
    public Guid? PackageId { get; init; }
    public Guid PackageTermId { get; init; }
    public Guid? TestPaperPackageId { get; init; }
    public decimal Amount { get; set; }
    public StaticConstants.PaymentType PaymentMode { get; init; }
    public string Source { get; init; }
    public string Remarks { get; init; }
}