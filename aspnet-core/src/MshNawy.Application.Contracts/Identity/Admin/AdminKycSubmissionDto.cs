using System;
using MshNawy.Domain.Shared;

namespace MshNawy.Application.Contracts.Identity.Admin;

public class AdminKycSubmissionDto
{
    public Guid UserId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? FullNameArabic { get; set; }
    public KycStatus Status { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string? RejectionReason { get; set; }
    public string? NationalIdFrontToken { get; set; }
    public string? NationalIdBackToken { get; set; }
    public string? SelfieToken { get; set; }
}
