using System;
using Xunit;
using Volo.Abp;
using MshNawy.Domain.Identity;
using MshNawy.Domain.Shared;

namespace MshNawy.Domain.Tests.Identity
{
    public class KycStateMachineTests
    {
        [Fact]
        public void SubmitKyc_FromDraft_Succeeds()
        {
            var user = new AppUser(Guid.NewGuid(), Guid.NewGuid(), "+201000000000");

            user.SubmitKyc("اسم المستخدم", new DateTime(1990, 1, 1), "12345678901234", "front", "back", "selfie", DateTime.UtcNow);

            Assert.Equal(KycStatus.Submitted, user.KycStatus);
        }

        [Fact]
        public void UnderReviewToApproved_Succeeds()
        {
            var user = new AppUser(Guid.NewGuid(), Guid.NewGuid(), "+201000000000");
            user.SubmitKyc("اسم المستخدم", new DateTime(1990, 1, 1), "12345678901234", "front", "back", "selfie", DateTime.UtcNow);

            user.MoveToUnderReview();
            user.ApproveKyc();

            Assert.Equal(KycStatus.Approved, user.KycStatus);
        }

        [Fact]
        public void UnderReviewToReject_Succeeds()
        {
            var user = new AppUser(Guid.NewGuid(), Guid.NewGuid(), "+201000000000");
            user.SubmitKyc("اسم المستخدم", new DateTime(1990, 1, 1), "12345678901234", "front", "back", "selfie", DateTime.UtcNow);

            user.MoveToUnderReview();
            user.RejectKyc("الصورة غير واضحة");

            Assert.Equal(KycStatus.Rejected, user.KycStatus);
            Assert.Equal("الصورة غير واضحة", user.KycRejectionReason);
        }

        [Fact]
        public void InvalidTransition_Throws()
        {
            var user = new AppUser(Guid.NewGuid(), Guid.NewGuid(), "+201000000000");

            var ex = Assert.Throws<BusinessException>(() => user.ApproveKyc());
            Assert.Equal(MshNawyErrorCodes.InvalidInput, ex.Code);
        }
    }
}
