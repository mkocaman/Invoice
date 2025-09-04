using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Db.Migrations
{
    /// <inheritdoc />
    public partial class Add_InvoiceAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvoiceAudits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InvoiceId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ExternalInvoiceNumber = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    EventType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    StatusFrom = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    StatusTo = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    SystemCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    TraceId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    XmlPayload = table.Column<string>(type: "text", nullable: true),
                    JsonPayload = table.Column<string>(type: "text", nullable: true),
                    RequestBody = table.Column<string>(type: "text", nullable: true),
                    ResponseBody = table.Column<string>(type: "text", nullable: true),
                    RedactionNotes = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    XmlSha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    JsonSha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    RequestSha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ResponseSha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Simulation = table.Column<bool>(type: "boolean", nullable: false),
                    Actor = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceAudits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAudits_CreatedAtUtc",
                table: "InvoiceAudits",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAudits_EventType_SystemCode",
                table: "InvoiceAudits",
                columns: new[] { "EventType", "SystemCode" });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAudits_InvoiceId",
                table: "InvoiceAudits",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAudits_Simulation",
                table: "InvoiceAudits",
                column: "Simulation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceAudits");
        }
    }
}
