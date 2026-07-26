using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneLine.AI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAIEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_usages_tenant_date",
                schema: "ai",
                table: "usages");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "ai",
                table: "usages");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "ai",
                table: "messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "ai",
                table: "usages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "ai",
                table: "messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_usages_tenant_date",
                schema: "ai",
                table: "usages",
                columns: new[] { "TenantId", "CreatedAt" });
        }
    }
}
