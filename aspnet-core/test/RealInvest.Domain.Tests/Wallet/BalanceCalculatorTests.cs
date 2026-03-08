using System;
using Xunit;
using RealInvest.Domain.Wallet;
using RealInvest.Domain.Shared;

namespace RealInvest.Domain.Tests.Wallet
{
    /// <summary>
    /// Tests for BalanceCalculator domain service.
    /// Per Constitution II: Balances are derived from ledger entries at any point in time.
    /// BalanceCalculator computes Available, Reserved, Invested, and PendingWithdrawal from ledger.
    /// </summary>
    public class BalanceCalculatorTests
    {
        [Fact]
        public async System.Threading.Tasks.Task GetBalances_WithEmptyLedger_ReturnsAllZeros()
        {
            // Arrange
            var ledgerService = new LedgerService();
            var calculator = new BalanceCalculator(ledgerService);
            var userId = Guid.NewGuid();

            // Act
            var (available, reserved, invested, pendingWithdrawal) = await calculator.GetBalancesAsync(userId);

            // Assert - no ledger entries = all balances zero
            Assert.Equal(0L, available);
            Assert.Equal(0L, reserved);
            Assert.Equal(0L, invested);
            Assert.Equal(0L, pendingWithdrawal);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetBalances_WithSingleDepositEntry_ShowsInAvailable()
        {
            // Arrange
            var ledgerService = new LedgerService();
            var calculator = new BalanceCalculator(ledgerService);
            var userId = Guid.NewGuid();
            var depositAmount = 1_000_000L; // 10 EGP

            // Post a deposit entry crediting Available account
            var depositEntry = new LedgerEntry(
                id: Guid.NewGuid(),
                timestamp: DateTime.UtcNow,
                entryType: LedgerEntryType.Deposit,
                debitAccount: "Settlement:Available",
                creditAccount: $"User:{userId}:Available",
                amount: depositAmount,
                idempotencyKey: Guid.NewGuid(),
                referenceEntityType: "Deposit",
                referenceEntityId: Guid.NewGuid(),
                description: "Test deposit"
            );
            await ledgerService.PostEntryAsync(depositEntry);

            // Act
            var (available, reserved, invested, pendingWithdrawal) = await calculator.GetBalancesAsync(userId);

            // Assert
            Assert.Equal(depositAmount, available);
            Assert.Equal(0L, reserved);
            Assert.Equal(0L, invested);
            Assert.Equal(0L, pendingWithdrawal);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetBalances_WithReservationEntry_ShowsInReserved()
        {
            // Arrange
            var ledgerService = new LedgerService();
            var calculator = new BalanceCalculator(ledgerService);
            var userId = Guid.NewGuid();
            var reservationAmount = 500_000L; // 5 EGP

            // First deposit
            var depositEntry = new LedgerEntry(
                id: Guid.NewGuid(),
                timestamp: DateTime.UtcNow,
                entryType: LedgerEntryType.Deposit,
                debitAccount: "Settlement:Available",
                creditAccount: $"User:{userId}:Available",
                amount: 1_000_000L,
                idempotencyKey: Guid.NewGuid(),
                referenceEntityType: "Deposit",
                referenceEntityId: Guid.NewGuid(),
                description: "Initial deposit"
            );
            await ledgerService.PostEntryAsync(depositEntry);

            // Then reserve for order
            var reservationEntry = new LedgerEntry(
                id: Guid.NewGuid(),
                timestamp: DateTime.UtcNow.AddSeconds(1),
                entryType: LedgerEntryType.OrderReservation,
                debitAccount: $"User:{userId}:Available",
                creditAccount: $"User:{userId}:Reserved",
                amount: reservationAmount,
                idempotencyKey: Guid.NewGuid(),
                referenceEntityType: "Order",
                referenceEntityId: Guid.NewGuid(),
                description: "Order reservation"
            );
            await ledgerService.PostEntryAsync(reservationEntry);

            // Act
            var (available, reserved, invested, pendingWithdrawal) = await calculator.GetBalancesAsync(userId);

            // Assert
            Assert.Equal(500_000L, available); // 1M - 500k
            Assert.Equal(500_000L, reserved);
            Assert.Equal(0L, invested);
            Assert.Equal(0L, pendingWithdrawal);
        }

        [Fact]
        public async System.Threading.Tasks.Task HasSufficientBalance_WithInsufficientFunds_ReturnsFalse()
        {
            // Arrange
            var ledgerService = new LedgerService();
            var calculator = new BalanceCalculator(ledgerService);
            var userId = Guid.NewGuid();

            // Post minimal deposit
            var depositEntry = new LedgerEntry(
                id: Guid.NewGuid(),
                timestamp: DateTime.UtcNow,
                entryType: LedgerEntryType.Deposit,
                debitAccount: "Settlement:Available",
                creditAccount: $"User:{userId}:Available",
                amount: 100_000L, // 1 EGP
                idempotencyKey: Guid.NewGuid(),
                referenceEntityType: "Deposit",
                referenceEntityId: Guid.NewGuid(),
                description: "Small deposit"
            );
            await ledgerService.PostEntryAsync(depositEntry);

            // Act - check if user has 1M piasters (10 EGP)
            var hasSufficient = await calculator.HasSufficientBalanceAsync(userId, 1_000_000L);

            // Assert
            Assert.False(hasSufficient);
        }

        [Fact]
        public async System.Threading.Tasks.Task HasSufficientBalance_WithSufficientFunds_ReturnsTrue()
        {
            // Arrange
            var ledgerService = new LedgerService();
            var calculator = new BalanceCalculator(ledgerService);
            var userId = Guid.NewGuid();

            // Post large deposit
            var depositEntry = new LedgerEntry(
                id: Guid.NewGuid(),
                timestamp: DateTime.UtcNow,
                entryType: LedgerEntryType.Deposit,
                debitAccount: "Settlement:Available",
                creditAccount: $"User:{userId}:Available",
                amount: 5_000_000L, // 50 EGP
                idempotencyKey: Guid.NewGuid(),
                referenceEntityType: "Deposit",
                referenceEntityId: Guid.NewGuid(),
                description: "Large deposit"
            );
            await ledgerService.PostEntryAsync(depositEntry);

            // Act - check if user has 1M piasters (10 EGP)
            var hasSufficient = await calculator.HasSufficientBalanceAsync(userId, 1_000_000L);

            // Assert
            Assert.True(hasSufficient);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetTotalWalletValue_AggregatesAllBalances()
        {
            // Arrange
            var ledgerService = new LedgerService();
            var calculator = new BalanceCalculator(ledgerService);
            var userId = Guid.NewGuid();

            // Post entries across all four account types
            await ledgerService.PostEntryAsync(new LedgerEntry(
                id: Guid.NewGuid(),
                timestamp: DateTime.UtcNow,
                entryType: LedgerEntryType.Deposit,
                debitAccount: "Settlement:Available",
                creditAccount: $"User:{userId}:Available",
                amount: 1_000_000L,
                idempotencyKey: Guid.NewGuid(),
                referenceEntityType: "Deposit",
                referenceEntityId: Guid.NewGuid(),
                description: "Deposit"
            ));

            await ledgerService.PostEntryAsync(new LedgerEntry(
                id: Guid.NewGuid(),
                timestamp: DateTime.UtcNow,
                entryType: LedgerEntryType.OrderReservation,
                debitAccount: $"User:{userId}:Available",
                creditAccount: $"User:{userId}:Reserved",
                amount: 500_000L,
                idempotencyKey: Guid.NewGuid(),
                referenceEntityType: "Order",
                referenceEntityId: Guid.NewGuid(),
                description: "Reservation"
            ));

            await ledgerService.PostEntryAsync(new LedgerEntry(
                id: Guid.NewGuid(),
                timestamp: DateTime.UtcNow,
                entryType: LedgerEntryType.OrderSettlement,
                debitAccount: $"User:{userId}:Reserved",
                creditAccount: $"User:{userId}:Invested",
                amount: 500_000L,
                idempotencyKey: Guid.NewGuid(),
                referenceEntityType: "Order",
                referenceEntityId: Guid.NewGuid(),
                description: "Settlement"
            ));

            // Act
            var total = await calculator.GetTotalWalletValueAsync(userId);

            // Assert - total = Available (500k) + Reserved (0) + Invested (500k) + Pending (0)
            Assert.Equal(1_000_000L, total);
        }
    }
}
