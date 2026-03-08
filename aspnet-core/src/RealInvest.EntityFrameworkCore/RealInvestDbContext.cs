using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using RealInvest.Domain.Wallet;
using RealInvest.Domain.Fees;

namespace RealInvest.EntityFrameworkCore
{
    /// <summary>
    /// RealInvest database context - inherits from ABP's AbpDbContext for infrastructure services.
    /// Per Constitution II: All ledger entries are immutable; all monetary movements recorded via double-entry.
    /// Per Constitution VII: Follows ABP layering conventions and DDD patterns.
    /// </summary>
    [ConnectionStringName("Default")]
    public class RealInvestDbContext : AbpDbContext<RealInvestDbContext>
    {
        public RealInvestDbContext(DbContextOptions<RealInvestDbContext> options) : base(options) { }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
        }
    }
}
