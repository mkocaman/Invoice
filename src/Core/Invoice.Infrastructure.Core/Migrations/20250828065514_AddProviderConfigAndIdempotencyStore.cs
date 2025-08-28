using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invoice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderConfigAndIdempotencyStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SigningMode",
                table: "Invoices",
                newName: "SignMode");

            migrationBuilder.CreateTable(
                name: "IdempotencyKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Method = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Path = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FirstSeenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastStatusCode = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdempotencyKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ApiBaseUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ApiKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ApiSecret = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    WebhookSecret = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    VknTckn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BranchCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SignMode = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSec = table.Column<int>(type: "integer", nullable: false),
                    RetryCountOverride = table.Column<int>(type: "integer", nullable: true),
                    CircuitTripThreshold = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderConfigs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyKeys_ExpiresAt",
                table: "IdempotencyKeys",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyKeys_Key",
                table: "IdempotencyKeys",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyKeys_TenantId",
                table: "IdempotencyKeys",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderConfigs_IsActive",
                table: "ProviderConfigs",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderConfigs_ProviderKey",
                table: "ProviderConfigs",
                column: "ProviderKey");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderConfigs_TenantId_ProviderKey",
                table: "ProviderConfigs",
                columns: new[] { "TenantId", "ProviderKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdempotencyKeys");

            migrationBuilder.DropTable(
                name: "ProviderConfigs");

            migrationBuilder.RenameColumn(
                name: "SignMode",
                table: "Invoices",
                newName: "SigningMode");
        }
    }
}
