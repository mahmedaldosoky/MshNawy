using System;
using Xunit;
using RealInvest.Domain.Fees;

namespace RealInvest.Domain.Tests.Fees
{
    /// <summary>
    /// Tests for FeeCalculator domain service.
    /// Per Constitution II: All calculations use integer arithmetic on piasters (no floating-point).
    /// Per Constitution IV: Fee calculations must be reproducible with identical inputs → identical outputs.
    /// </summary>
    public class FeeCalculatorTests
    {
        private readonly FeeCalculator _calculator = new FeeCalculator();

        private FeePolicy CreatePolicy()
        {
            // Fee policy: Entry 1%, Payment 3%, Exit 5% (2.5% brokerage + 2.5% platform)
            return new FeePolicy(
                Guid.NewGuid(),
                version: 1,
                effectiveFrom: DateTime.UtcNow,
                entryFeePercent: 1m,
                paymentFeePercent: 3m,
                exitFeePercent: 5m,
                exitBrokeragePercent: 2.5m,
                exitPlatformProfitPercent: 2.5m
            );
        }

        [Fact]
        public async System.Threading.Tasks.Task CalculateEntryFee_WithStandardAmount_ReturnsOnePercent()
        {
            // Arrange: 1,000,000 piasters (10 EGP)
            var amountPiasters = 1_000_000L;
            var policy = CreatePolicy();

            // Act
            var fee = await _calculator.CalculateEntryFeeAsync(amountPiasters, policy);

            // Assert: 1% of 1,000,000 = 10,000 piasters
            Assert.Equal(10_000L, fee);
        }

        [Fact]
        public async System.Threading.Tasks.Task CalculateEntryFee_WithZeroAmount_ReturnsZero()
        {
            var policy = CreatePolicy();
            var fee = await _calculator.CalculateEntryFeeAsync(0, policy);
            Assert.Equal(0L, fee);
        }

        [Fact]
        public async System.Threading.Tasks.Task CalculateEntryFee_WithMaxAmount_CalculatesCorrectly()
        {
            // Arrange: Large amount to verify no overflow
            var amountPiasters = long.MaxValue / 100; // Ensure calculation doesn't overflow
            var policy = CreatePolicy();

            // Act
            var fee = await _calculator.CalculateEntryFeeAsync(amountPiasters, policy);

            // Assert: Fee should be 1% of amount
            var expectedFee = amountPiasters / 100;
            Assert.Equal(expectedFee, fee);
        }

        [Fact]
        public async System.Threading.Tasks.Task CalculatePaymentFee_WithStandardAmount_ReturnsThreePercent()
        {
            // Arrange: 100,000 piasters (1 EGP installment)
            var amountPiasters = 100_000L;
            var policy = CreatePolicy();

            // Act
            var fee = await _calculator.CalculatePaymentFeeAsync(amountPiasters, policy);

            // Assert: 3% of 100,000 = 3,000 piasters
            Assert.Equal(3_000L, fee);
        }

        [Fact]
        public async System.Threading.Tasks.Task CalculatePaymentFee_WithSmallAmount_RoundsDown()
        {
            // Arrange: 1 piaster (no integer division loss expected for 3% calculation)
            var amountPiasters = 1L;
            var policy = CreatePolicy();

            // Act
            var fee = await _calculator.CalculatePaymentFeeAsync(amountPiasters, policy);

            // Assert: 3% of 1 = 0.03, integer division rounds down to 0
            Assert.Equal(0L, fee);
        }

        [Fact]
        public async System.Threading.Tasks.Task CalculateExitFee_WithStandardAmount_ReturnsBrokerageAndPlatform()
        {
            // Arrange: 10,000,000 piasters (100 EGP)
            var amountPiasters = 10_000_000L;
            var policy = CreatePolicy();

            // Act
            var (brokerage, platform) = await _calculator.CalculateExitFeeAsync(amountPiasters, policy);

            // Assert: 5% split 2.5% + 2.5%
            // 2.5% of 10,000,000 = 250,000 piasters each
            Assert.Equal(250_000L, brokerage);
            Assert.Equal(250_000L, platform);
        }

        [Fact]
        public async System.Threading.Tasks.Task CalculateExitFee_ExitFeesSplitEvenly()
        {
            var amountPiasters = 100_000_000L; // 1,000 EGP
            var policy = CreatePolicy();

            var (brokerage, platform) = await _calculator.CalculateExitFeeAsync(amountPiasters, policy);

            // Both should be 2.5% each
            var expectedEach = amountPiasters * 250 / 10000; // 2.5% = 250 / 10000
            Assert.Equal(expectedEach, brokerage);
            Assert.Equal(expectedEach, platform);
        }

        [Fact]
        public async System.Threading.Tasks.Task AllFeeCalculations_AreDeterministic()
        {
            // Arrange
            var amountPiasters = 5_000_000L;
            var policy = CreatePolicy();

            // Act - run calculations multiple times
            var fee1 = await _calculator.CalculateEntryFeeAsync(amountPiasters, policy);
            var fee2 = await _calculator.CalculateEntryFeeAsync(amountPiasters, policy);
            var fee3 = await _calculator.CalculateEntryFeeAsync(amountPiasters, policy);

            // Assert - all results should be identical
            Assert.Equal(fee1, fee2);
            Assert.Equal(fee2, fee3);
        }
    }
}
