namespace MshNawy.Application.Contracts.Identity;

public class SendOtpResponseDto
{
    public int ExpiresInSeconds { get; set; }
    public int AttemptsRemaining { get; set; }
}
