using System.Threading;
using System.Threading.Tasks;

namespace MshNawy.Application.Identity;

public interface IOtpSender
{
    Task SendAsync(string phoneNumber, string otpCode, CancellationToken cancellationToken = default);
}
