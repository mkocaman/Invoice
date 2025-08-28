using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invoice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Normalize_ProviderSendResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "ProviderConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiKeyHeaderName",
                table: "ProviderConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ProviderConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                table: "ProviderConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Extra1",
                table: "ProviderConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Extra2",
                table: "ProviderConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirmCode",
                table: "ProviderConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrantType",
                table: "ProviderConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "ProviderConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProviderType",
                table: "ProviderConfigs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TokenUrl",
                table: "ProviderConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "ProviderConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "ProviderConfigs",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "IdempotencyKeys",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "IdempotencyKeys",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Method",
                table: "IdempotencyKeys",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "IdempotencyKeys",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "IdempotencyKeys",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestHash",
                table: "IdempotencyKeys",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "IdempotencyKeys",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UsedAt",
                table: "IdempotencyKeys",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "ProviderConfigs");

            migrationBuilder.DropColumn(
                name: "ApiKeyHeaderName",
                table: "ProviderConfigs");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ProviderConfigs");

            migrationBuilder.DropColumn(
                name: "ClientSecret",
                table: "ProviderConfigs");

            migrationBuilder.DropColumn(
                name: "Extra1",
                table: "ProviderConfigs");

            migrationBuilder.DropColumn(
                name: "Extra2",
                table: "ProviderConfigs");

            migrationBuilder.DropColumn(
                name: "FirmCode",
                table: "ProviderConfigs");

            migrationBuilder.DropColumn(
                name: "GrantType",
                table: "ProviderConfigs");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "ProviderConfigs");

            migrationBuilder.DropColumn(
                name: "ProviderType",
                table: "ProviderConfigs");

            migrationBuilder.DropColumn(
                name: "TokenUrl",
                table: "ProviderConfigs");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "ProviderConfigs");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "ProviderConfigs");

            migrationBuilder.DropColumn(
                name: "RequestHash",
                table: "IdempotencyKeys");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "IdempotencyKeys");

            migrationBuilder.DropColumn(
                name: "UsedAt",
                table: "IdempotencyKeys");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "IdempotencyKeys",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "IdempotencyKeys",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Method",
                table: "IdempotencyKeys",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "IdempotencyKeys",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "IdempotencyKeys",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
