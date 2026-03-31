using System;
using MshNawy.Domain.Shared;

namespace MshNawy.Application.Contracts.Identity;

public class KycStatusResponseDto
{
    public KycStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? SubmittedAt { get; set; }
}
