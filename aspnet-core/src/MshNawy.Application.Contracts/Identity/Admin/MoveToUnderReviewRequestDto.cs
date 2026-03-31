using System;
using MediatR;

namespace MshNawy.Application.Contracts.Identity.Admin;

public class MoveToUnderReviewRequestDto : IRequest
{
    public Guid UserId { get; set; }
}
