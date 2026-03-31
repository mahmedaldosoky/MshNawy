using System;
using MshNawy.Domain.Shared;

namespace MshNawy.Application.Contracts.Identity;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public KycStatus KycStatus { get; set; }
}
