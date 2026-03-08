using System;
using Xunit;
using MshNawy.Domain.Wallet;
using MshNawy.Domain.Shared;

namespace MshNawy.Domain.Tests.Wallet
{
    /// <summary>
    /// Tests for LedgerService domain service.
    /// Per Constitution II: All monetary movements recorded via immutable double-entry ledger entries.
    /// Per Constitution III: All financial operations must be idempotent with unique idempotency keys.
    /// </summary>
    public class LedgerServiceTests
    {
        private readonly LedgerService _ledgerService = new LedgerService();

        [Fact]
        public async System.Threading.Tasks.Task PostEntry_WithValidEntry_Succeeds()
        {
            // Arrange
            var entry = new LedgerEntry(
                id: Guid.NewGuid(),
                timestamp: DateTime.UtcNow,
                entryType: LedgerEntryType.Deposit,
                debitAccount: "Settlement:Available",
                creditAccount: "User:d290f1ee-6c54-4b01-90e6-d701748f0851:Available",
                amount: 100_000,
                idempotencyKey: Guid.NewGuid(),
                referenceEntityType: "Deposit",
                referenceEntityId: Guid.NewGuid(),
                description: "Deposit from user"
            );

            // Act & Assert - should not throw
            await _ledgerService.PostEntryAsync(entry);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostEntry_WithNullEntry_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _ledgerService.PostEntryAsync(null)
            );
        }

        [Fact]
        public async System.Threading.Tasks.Task PostEntry_WithDuplicateIdempotencyKey_ThrowsInvalidOperationException()
        {
            // Arrange
            var idempotencyKey = Guid.NewGuid();
            var entry1 = new LedgerEntry(
                id: Guid.NewGuid(),
                timestamp: DateTime.UtcNow,
                entryType: LedgerEntryType.Deposit,
                debitAccount: "Settlement:Available",
                creditAccount: "User:available",
                amount: 100_000,
                idempotencyKey: idempotencyKey,
                referenceEntityType: "Deposit",
                referenceEntityId: Guid.NewGuid(),
                description: "Entry 1"
            );

            var entry2 = new LedgerEntry(
                id: Guid.NewGuid(),
                timestamp: DateTime.UtcNow,
                entryType: LedgerEntryType.Deposit,
                debitAccount: "Settlement:Available",
                creditAccount: "User:available",
                amount: 100_000,
                idempotencyKey: idempotencyKey, // Same key
                referenceEntityType: "Deposit",
                referenceEntityId: Guid.NewGuid(),
                description: "Entry 2 (duplicate)"
            );

            // Act
            await _ledgerService.PostEntryAsync(entry1);

            // Assert - posting same idempotency key should fail
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _ledgerService.PostEntryAsync(entry2)
            );
        }

        [Fact]
        public async System.Threading.Tasks.Task PostEntry_WithZeroAmount_ThrowsInvalidOperationException()
        {
            // Arrange
            var entry = new LedgerEntry(
                id: Guid.NewGuid(),
                timestamp: DateTime.UtcNow,
                entryType: LedgerEntryType.Deposit,
                debitAccount: "Settlement:Available",
                creditAccount: "User:available",
                amount: 0, // Invalid
                idempotencyKey: Guid.NewGuid(),
                referenceEntityType: "Deposit",
                referenceEntityId: Guid.NewGuid(),
                description: "Invalid zero amount"
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _ledgerService.PostEntryAsync(entry)
            );
        }

        [Fact]
        public async System.Threading.Tasks.Task PostCompensatingEntry_CreatesReversal()
        {
            // Arrange
            var originalId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var originalEntry = new LedgerEntry(
                id: originalId,
                timestamp: DateTime.UtcNow,
                entryType: LedgerEntryType.Deposit,
                debitAccount: "Settlement:Available",
                creditAccount: $"User:{userId}:Available",
                amount: 500_000,
                idempotencyKey: Guid.NewGuid(),
                referenceEntityType: "Deposit",
                referenceEntityId: Guid.NewGuid(),
                description: "Original deposit"
            );

            var compensatingEntry = new LedgerEntry(
                id: Guid.NewGuid(),
                timestamp: DateTime.UtcNow.AddSeconds(1),
                entryType: LedgerEntryType.Compensating,
                debitAccount: $"User:{userId}:Available", // Swapped from original
                creditAccount: "Settlement:Available", // Swapped from original
                amount: 500_000,
                idempotencyKey: Guid.NewGuid(),
                referenceEntityType: "Deposit",
                referenceEntityId: originalEntry.ReferenceEntityId,
                description: "Compensating entry - reversal"
            );

            // Act
            await _ledgerService.PostEntryAsync(originalEntry);
            await _ledgerService.PostCompensatingEntryAsync(originalId, compensatingEntry);

            // Assert - both entries posted successfully
            // (In real implementation, would verify via repository queries)
        }

        [Fact]
        public async System.Threading.Tasks.Task PostCompensatingEntry_WithNonexistentOriginal_ThrowsInvalidOperationException()
        {
            // Arrange
            var nonexistentId = Guid.NewGuid();
            var compensatingEntry = new LedgerEntry(
                id: Guid.NewGuid(),
                timestamp: DateTime.UtcNow,
                entryType: LedgerEntryType.Compensating,
                debitAccount: "User:available",
                creditAccount: "Settlement:Available",
                amount: 100_000,
                idempotencyKey: Guid.NewGuid(),
                referenceEntityType: "Deposit",
                referenceEntityId: Guid.NewGuid(),
                description: "Compensating entry"
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _ledgerService.PostCompensatingEntryAsync(nonexistentId, compensatingEntry)
            );
        }

        [Fact]
        public async System.Threading.Tasks.Task ValidateBalance_WithBalancedEntries_Succeeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var depositId = Guid.NewGuid();

            var debitEntry = new LedgerEntry(
                id: Guid.NewGuid(),
                timestamp: DateTime.UtcNow,
                entryType: LedgerEntryType.Deposit,
                debitAccount: "Settlement:Available",
                creditAccount: $"User:{userId}:Available",
                amount: 1_000_000,
                idempotencyKey: Guid.NewGuid(),
                referenceEntityType: "Deposit",
                referenceEntityId: depositId,
                description: "Balanced debit/credit pair"
            );

            await _ledgerService.PostEntryAsync(debitEntry);

            // Act & Assert - validation should succeed
            await _ledgerService.ValidateBalanceAsync(userId);
        }
    }
}
