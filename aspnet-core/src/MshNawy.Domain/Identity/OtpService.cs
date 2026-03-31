using System;
using System.Security.Cryptography;
using System.Text;
using Volo.Abp;
using MshNawy.Domain.Shared;

namespace MshNawy.Domain.Identity;

public class OtpService : IOtpService
{
    private const int OtpLength = 6;
    private static readonly TimeSpan OtpExpiration = TimeSpan.FromMinutes(3);
    private static readonly TimeSpan OtpWindow = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan OtpLockout = TimeSpan.FromMinutes(30);
    private const int MaxAttempts = 5;

    public string GenerateOtp(AppUser user, DateTime now)
    {
        CheckRateLimit(user, now);

        var otp = GenerateNumericOtp(OtpLength);
        var hash = HashOtp(otp);
        user.SetOtp(hash, now.Add(OtpExpiration), now);

        return otp;
    }

    public void VerifyOtp(AppUser user, string otp, DateTime now)
    {
        CheckRateLimit(user, now);

        if (user.OtpExpiresAt.HasValue && user.OtpExpiresAt.Value < now)
        {
            user.RegisterFailedOtpAttempt(now, MaxAttempts, OtpLockout);
            throw new BusinessException(user.OtpLockedUntil.HasValue
                ? MshNawyErrorCodes.OtpPhoneLockedOut
                : MshNawyErrorCodes.OtpExpired);
        }

        var hash = HashOtp(otp);
        if (!string.Equals(hash, user.OtpCodeHash, StringComparison.Ordinal))
        {
            user.RegisterFailedOtpAttempt(now, MaxAttempts, OtpLockout);
            throw new BusinessException(user.OtpLockedUntil.HasValue
                ? MshNawyErrorCodes.OtpPhoneLockedOut
                : MshNawyErrorCodes.OtpInvalid);
        }

        user.ClearOtpState();
    }

    public void CheckRateLimit(AppUser user, DateTime now)
    {
        if (user.OtpLockedUntil.HasValue && user.OtpLockedUntil.Value > now)
        {
            throw new BusinessException(MshNawyErrorCodes.OtpPhoneLockedOut);
        }

        if (user.OtpWindowStart.HasValue && now - user.OtpWindowStart.Value > OtpWindow)
        {
            user.ClearOtpState();
        }

        if (user.OtpAttemptCount >= MaxAttempts)
        {
            user.RegisterFailedOtpAttempt(now, MaxAttempts, OtpLockout);
            throw new BusinessException(MshNawyErrorCodes.OtpPhoneLockedOut);
        }
    }

    private static string GenerateNumericOtp(int length)
    {
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        var builder = new StringBuilder(length);
        foreach (var b in bytes)
        {
            builder.Append((b % 10).ToString());
        }
        return builder.ToString();
    }

    private static string HashOtp(string otp)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(otp));
        return Convert.ToHexString(bytes);
    }
}
