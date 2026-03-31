using System;
using MediatR;

namespace MshNawy.Application.Contracts.Identity.Admin;

public class ReviewKycRequestDto : IRequest
{
    public Guid UserId { get; set; }
    public KycReviewDecisionRequestDto Input { get; set; } = new();
}
