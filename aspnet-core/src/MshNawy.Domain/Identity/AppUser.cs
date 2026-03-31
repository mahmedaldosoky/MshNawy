using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using MshNawy.Domain.Shared;

namespace MshNawy.Domain.Identity;

public class AppUser : Entity<Guid>
{
    public Guid IdentityUserId { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty;

    public KycStatus KycStatus { get; private set; } = KycStatus.Draft;
    public string? KycRejectionReason { get; private set; }
    public DateTime? KycSubmittedAt { get; private set; }

    public string? FullNameArabic { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public string? NationalIdNumber { get; private set; }
    public string? NationalIdFrontImagePath { get; private set; }
    public string? NationalIdBackImagePath { get; private set; }
    public string? SelfiePath { get; private set; }

    public string? OtpCodeHash { get; private set; }
    public DateTime? OtpExpiresAt { get; private set; }
    public int OtpAttemptCount { get; private set; }
    public DateTime? OtpWindowStart { get; private set; }
    public DateTime? OtpLockedUntil { get; private set; }

    private AppUser() { }

    public AppUser(Guid id, Guid identityUserId, string phoneNumber)
    {
        Id = id;
        IdentityUserId = identityUserId;
        PhoneNumber = phoneNumber;
    }

    public void SetOtp(string otpHash, DateTime expiresAt, DateTime windowStart)
    {
        OtpCodeHash = otpHash;
        OtpExpiresAt = expiresAt;
        OtpWindowStart = windowStart;
        OtpAttemptCount = 0;
    }

    public void RegisterFailedOtpAttempt(DateTime now, int maxAttempts, TimeSpan lockoutDuration)
    {
        OtpAttemptCount += 1;
        if (OtpAttemptCount >= maxAttempts)
        {
            OtpLockedUntil = now.Add(lockoutDuration);
        }
    }

    public void ClearOtpState()
    {
        OtpCodeHash = null;
        OtpExpiresAt = null;
        OtpAttemptCount = 0;
        OtpWindowStart = null;
        OtpLockedUntil = null;
    }

    public void SubmitKyc(string fullNameArabic, DateTime dateOfBirth, string nationalIdNumber, string frontPath, string backPath, string selfiePath, DateTime submittedAt)
    {
        if (KycStatus != KycStatus.Draft && KycStatus != KycStatus.Rejected && KycStatus != KycStatus.NeedsResubmission)
        {
            throw new BusinessException(MshNawyErrorCodes.InvalidInput);
        }

        FullNameArabic = fullNameArabic;
        DateOfBirth = dateOfBirth;
        NationalIdNumber = nationalIdNumber;
        NationalIdFrontImagePath = frontPath;
        NationalIdBackImagePath = backPath;
        SelfiePath = selfiePath;
        KycSubmittedAt = submittedAt;
        KycRejectionReason = null;
        KycStatus = KycStatus.Submitted;
    }

    public void MoveToUnderReview()
    {
        if (KycStatus != KycStatus.Submitted)
        {
            throw new BusinessException(MshNawyErrorCodes.InvalidInput);
        }

        KycStatus = KycStatus.UnderReview;
    }

    public void ApproveKyc()
    {
        if (KycStatus != KycStatus.UnderReview)
        {
            throw new BusinessException(MshNawyErrorCodes.InvalidInput);
        }

        KycStatus = KycStatus.Approved;
    }

    public void RejectKyc(string reason)
    {
        if (KycStatus != KycStatus.UnderReview)
        {
            throw new BusinessException(MshNawyErrorCodes.InvalidInput);
        }

        KycStatus = KycStatus.Rejected;
        KycRejectionReason = reason;
    }

    public void RequestResubmission(string reason)
    {
        if (KycStatus != KycStatus.UnderReview)
        {
            throw new BusinessException(MshNawyErrorCodes.InvalidInput);
        }

        KycStatus = KycStatus.NeedsResubmission;
        KycRejectionReason = reason;
    }

    public void Resubmit(DateTime submittedAt)
    {
        if (KycStatus != KycStatus.Rejected && KycStatus != KycStatus.NeedsResubmission)
        {
            throw new BusinessException(MshNawyErrorCodes.InvalidInput);
        }

        KycSubmittedAt = submittedAt;
        KycRejectionReason = null;
        KycStatus = KycStatus.Submitted;
    }
}
