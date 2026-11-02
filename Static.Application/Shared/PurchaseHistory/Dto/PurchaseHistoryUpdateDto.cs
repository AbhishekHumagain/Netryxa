using Static.Helper.Constants;

namespace Static.Application.Shared.PurchaseHistory.Dto;

public record PurchaseHistoryUpdateDto
{
    public Guid Id { get; init; }

    public bool IsPaymentSuccess { get; init; }

    public bool IsVerified { get; init; }

    public string? TransactionCode { get; init; }

    public string Details { get; init; }

    public StaticConstants.PaymentType PaymentMode { get; init; }

    public DateTime TransactionDate { get; init; }

    public int PaymentStatus { get; init; }
}