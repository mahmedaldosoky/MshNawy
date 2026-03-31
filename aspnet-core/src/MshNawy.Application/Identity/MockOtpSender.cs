using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MshNawy.Application.Identity;

public class MockOtpSender : IOtpSender
{
    private readonly ILogger<MockOtpSender> logger;

    public MockOtpSender(ILogger<MockOtpSender> logger)
    {
        this.logger = logger;
    }

    public Task SendAsync(string phoneNumber, string otpCode, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("OTP sent to {PhoneNumber}: {OtpCode}", phoneNumber, otpCode);
        return Task.CompletedTask;
    }
}
