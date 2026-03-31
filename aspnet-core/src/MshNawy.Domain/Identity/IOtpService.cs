using System;

namespace MshNawy.Domain.Identity;

public interface IOtpService
{
    string GenerateOtp(AppUser user, DateTime now);
    void VerifyOtp(AppUser user, string otp, DateTime now);
    void CheckRateLimit(AppUser user, DateTime now);
}
