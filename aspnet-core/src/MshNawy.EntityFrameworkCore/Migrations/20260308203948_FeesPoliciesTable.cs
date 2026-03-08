using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MshNawy.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class FeesPoliciesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeePolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EntryFeePercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PaymentFeePercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ExitFeePercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ExitBrokeragePercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ExitPlatformProfitPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeePolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LedgerEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntryType = table.Column<int>(type: "int", nullable: false),
                    DebitAccount = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreditAccount = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    IdempotencyKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferenceEntityType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompensatingEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PostedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeePolicy_EffectiveFrom",
                table: "FeePolicies",
                column: "EffectiveFrom");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEntry_CreditAccount",
                table: "LedgerEntries",
                column: "CreditAccount");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEntry_DebitAccount",
                table: "LedgerEntries",
                column: "DebitAccount");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEntry_IdempotencyKey",
                table: "LedgerEntries",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEntry_ReferenceEntityId",
                table: "LedgerEntries",
                column: "ReferenceEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeePolicies");

            migrationBuilder.DropTable(
                name: "LedgerEntries");
        }
    }
}
