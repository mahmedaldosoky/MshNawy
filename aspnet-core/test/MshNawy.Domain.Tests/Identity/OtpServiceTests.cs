using System;
using Xunit;
using Volo.Abp;
using MshNawy.Domain.Identity;
using MshNawy.Domain.Shared;

namespace MshNawy.Domain.Tests.Identity
{
    public class OtpServiceTests
    {
        private readonly OtpService otpService = new OtpService();

        [Fact]
        public void GenerateAndVerifyOtp_Succeeds()
        {
            var user = new AppUser(Guid.NewGuid(), Guid.NewGuid(), "+201000000000");
            var now = DateTime.UtcNow;

            var otp = otpService.GenerateOtp(user, now);

            otpService.VerifyOtp(user, otp, now.AddMinutes(1));
            Assert.Null(user.OtpCodeHash);
        }

        [Fact]
        public void VerifyOtp_WithInvalidCode_Throws()
        {
            var user = new AppUser(Guid.NewGuid(), Guid.NewGuid(), "+201000000000");
            var now = DateTime.UtcNow;

            otpService.GenerateOtp(user, now);

            var ex = Assert.Throws<BusinessException>(() =>
                otpService.VerifyOtp(user, "000000", now.AddMinutes(1))
            );

            Assert.Equal(MshNawyErrorCodes.OtpInvalid, ex.Code);
        }

        [Fact]
        public void VerifyOtp_WithExpiredCode_Throws()
        {
            var user = new AppUser(Guid.NewGuid(), Guid.NewGuid(), "+201000000000");
            var now = DateTime.UtcNow;

            var otp = otpService.GenerateOtp(user, now);

            var ex = Assert.Throws<BusinessException>(() =>
                otpService.VerifyOtp(user, otp, now.AddMinutes(10))
            );

            Assert.Equal(MshNawyErrorCodes.OtpExpired, ex.Code);
        }

        [Fact]
        public void VerifyOtp_WithTooManyAttempts_LocksOut()
        {
            var user = new AppUser(Guid.NewGuid(), Guid.NewGuid(), "+201000000000");
            var now = DateTime.UtcNow;

            otpService.GenerateOtp(user, now);

            for (var i = 0; i < 4; i++)
            {
                Assert.Throws<BusinessException>(() =>
                    otpService.VerifyOtp(user, "111111", now.AddMinutes(1))
                );
            }

            var ex = Assert.Throws<BusinessException>(() =>
                otpService.VerifyOtp(user, "222222", now.AddMinutes(1))
            );

            Assert.Equal(MshNawyErrorCodes.OtpPhoneLockedOut, ex.Code);
            Assert.NotNull(user.OtpLockedUntil);
        }
    }
}
