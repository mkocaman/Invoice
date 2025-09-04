using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Db.Migrations
{
    /// <inheritdoc />
    public partial class Add_InvoiceStatusHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvoiceStatusHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InvoiceId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ExternalInvoiceNumber = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    EventType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    StatusFrom = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    StatusTo = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    SystemCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    OccurredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LatencyMs = table.Column<long>(type: "bigint", nullable: true),
                    EventKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Simulation = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceStatusHistory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceStatusHistory_EventType_SystemCode",
                table: "InvoiceStatusHistory",
                columns: new[] { "EventType", "SystemCode" });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceStatusHistory_InvoiceId",
                table: "InvoiceStatusHistory",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceStatusHistory_InvoiceId_EventKey",
                table: "InvoiceStatusHistory",
                columns: new[] { "InvoiceId", "EventKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceStatusHistory_OccurredAtUtc",
                table: "InvoiceStatusHistory",
                column: "OccurredAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceStatusHistory");
        }
    }
}
