namespace MshNawy.Application.Contracts.Identity.Admin;

public class KycReviewDecisionRequestDto
{
    public KycReviewDecision Decision { get; set; }
    public string? Reason { get; set; }
}
