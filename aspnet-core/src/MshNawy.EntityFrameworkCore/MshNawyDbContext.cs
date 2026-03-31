using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using MshNawy.Domain.Wallet;
using MshNawy.Domain.Fees;
using MshNawy.Domain.Identity;
using MshNawy.Domain.Shared;

namespace MshNawy.EntityFrameworkCore
{
    /// <summary>
    /// MshNawy database context - inherits from ABP's AbpDbContext for infrastructure services.
    /// Per Constitution II: All ledger entries are immutable; all monetary movements recorded via double-entry.
    /// Per Constitution VII: Follows ABP layering conventions and DDD patterns.
    /// </summary>
    [ConnectionStringName("Default")]
    public class MshNawyDbContext : AbpDbContext<MshNawyDbContext>
    {
        public MshNawyDbContext(DbContextOptions<MshNawyDbContext> options) : base(options) { }

        /// <summary>
        /// Ledger entries - immutable record of all monetary movements
        /// Per Constitution II: Core financial audit trail
        /// </summary>
        public DbSet<LedgerEntry> LedgerEntries { get; set; }

        /// <summary>
        /// Fee policy configuration - versioned and effective-dated
        /// Per Constitution IV: Configurable fee rules with historical tracking
        /// </summary>
        public DbSet<FeePolicy> FeePolicies { get; set; }

        /// <summary>
        /// App user aggregate - OTP and KYC extension fields
        /// </summary>
        public DbSet<AppUser> AppUsers { get; set; }

        /// <summary>
        /// Idempotency records for financial endpoint deduplication
        /// Per Constitution III: All financial operations must be idempotent
        /// </summary>
        public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigureIdentity();
            modelBuilder.ConfigurePermissionManagement();

            // Configure LedgerEntry
            modelBuilder.Entity<LedgerEntry>(b =>
            {
                b.HasKey(x => x.Id);

                // Unique constraint on idempotency key - per Constitution III
                b.HasIndex(x => x.IdempotencyKey).IsUnique()
                    .HasDatabaseName("IX_LedgerEntry_IdempotencyKey");

                // Additional indexes for query performance
                b.HasIndex(x => x.DebitAccount).HasDatabaseName("IX_LedgerEntry_DebitAccount");
                b.HasIndex(x => x.CreditAccount).HasDatabaseName("IX_LedgerEntry_CreditAccount");
                b.HasIndex(x => x.ReferenceEntityId).HasDatabaseName("IX_LedgerEntry_ReferenceEntityId");

                // Immutable - no update property mappings beyond EF auditing
                b.Property(x => x.Timestamp).HasColumnType("datetime2").IsRequired();
                b.Property(x => x.Amount).HasColumnType("bigint").IsRequired();
                b.Property(x => x.IdempotencyKey).HasColumnType("uniqueidentifier").IsRequired();
            });

            // Configure FeePolicy
            modelBuilder.Entity<FeePolicy>(b =>
            {
                b.HasKey(x => x.Id);

                b.Property(x => x.Version).IsRequired();
                b.Property(x => x.EffectiveFrom).HasColumnType("datetime2").IsRequired();
                b.Property(x => x.EntryFeePercent).HasColumnType("decimal(5,2)").IsRequired();
                b.Property(x => x.PaymentFeePercent).HasColumnType("decimal(5,2)").IsRequired();
                b.Property(x => x.ExitFeePercent).HasColumnType("decimal(5,2)").IsRequired();
                b.Property(x => x.ExitBrokeragePercent).HasColumnType("decimal(5,2)").IsRequired();
                b.Property(x => x.ExitPlatformProfitPercent).HasColumnType("decimal(5,2)").IsRequired();

                // Index for effective date lookups
                b.HasIndex(x => x.EffectiveFrom).HasDatabaseName("IX_FeePolicy_EffectiveFrom");
            });

            // Configure AppUser
            modelBuilder.Entity<AppUser>(b =>
            {
                b.HasKey(x => x.Id);

                b.Property(x => x.IdentityUserId).IsRequired();
                b.HasIndex(x => x.IdentityUserId).IsUnique();

                b.Property(x => x.PhoneNumber).HasMaxLength(15).IsRequired();
                b.HasIndex(x => x.PhoneNumber).IsUnique();

                b.Property(x => x.KycStatus).HasConversion<int>().IsRequired();
                b.Property(x => x.KycRejectionReason).HasMaxLength(500);
                b.Property(x => x.KycSubmittedAt).HasColumnType("datetime2");

                b.Property(x => x.FullNameArabic).HasMaxLength(200);
                b.Property(x => x.DateOfBirth).HasColumnType("date");
                b.Property(x => x.NationalIdNumber).HasMaxLength(14);
                b.HasIndex(x => x.NationalIdNumber).IsUnique();

                b.Property(x => x.NationalIdFrontImagePath).HasMaxLength(500);
                b.Property(x => x.NationalIdBackImagePath).HasMaxLength(500);
                b.Property(x => x.SelfiePath).HasMaxLength(500);

                b.Property(x => x.OtpCodeHash).HasMaxLength(64);
                b.Property(x => x.OtpExpiresAt).HasColumnType("datetime2");
                b.Property(x => x.OtpWindowStart).HasColumnType("datetime2");
                b.Property(x => x.OtpLockedUntil).HasColumnType("datetime2");
            });

            // Configure IdempotencyRecord
            modelBuilder.Entity<IdempotencyRecord>(b =>
            {
                b.HasKey(x => x.Id);

                b.HasIndex(x => x.IdempotencyKey).IsUnique()
                    .HasDatabaseName("IX_IdempotencyRecord_Key");

                b.Property(x => x.IdempotencyKey).HasColumnType("uniqueidentifier").IsRequired();
                b.Property(x => x.ResponseBody).IsRequired();
                b.Property(x => x.StatusCode).IsRequired();
                b.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
                b.Property(x => x.ExpiresAt).HasColumnType("datetime2").IsRequired();

                b.HasIndex(x => x.ExpiresAt).HasDatabaseName("IX_IdempotencyRecord_ExpiresAt");
            });
        }
    }
}
