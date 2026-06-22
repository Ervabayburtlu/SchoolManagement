using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "refresh_token",
                table: "STUDENT",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "refresh_token_expiry",
                table: "STUDENT",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "refresh_token",
                table: "ADVISOR",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "refresh_token_expiry",
                table: "ADVISOR",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "refresh_token",
                table: "ACADEMICIAN",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "refresh_token_expiry",
                table: "ACADEMICIAN",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "refresh_token",
                table: "STUDENT");

            migrationBuilder.DropColumn(
                name: "refresh_token_expiry",
                table: "STUDENT");

            migrationBuilder.DropColumn(
                name: "refresh_token",
                table: "ADVISOR");

            migrationBuilder.DropColumn(
                name: "refresh_token_expiry",
                table: "ADVISOR");

            migrationBuilder.DropColumn(
                name: "refresh_token",
                table: "ACADEMICIAN");

            migrationBuilder.DropColumn(
                name: "refresh_token_expiry",
                table: "ACADEMICIAN");
        }
    }
}
