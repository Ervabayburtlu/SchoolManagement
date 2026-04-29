using System;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace SchoolManagement.Infrastructure.Migrations
{
    public partial class AddConsistencyFieldsToStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "active_bar_count",
                table: "STUDENT",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "is_locked",
                table: "STUDENT",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "locked_at",
                table: "STUDENT",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "unlocked_at",
                table: "STUDENT",
                type: "datetime(6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "active_bar_count", table: "STUDENT");
            migrationBuilder.DropColumn(name: "is_locked", table: "STUDENT");
            migrationBuilder.DropColumn(name: "locked_at", table: "STUDENT");
            migrationBuilder.DropColumn(name: "unlocked_at", table: "STUDENT");
        }
    }
}