using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderEmailSentToStudentExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "student_no_exam_id",
                table: "STUDENT_EXAM",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            
            migrationBuilder.AddColumn<bool>(
                name: "reminder_email_sent",
                table: "STUDENT_EXAM",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "exam_id",
                table: "EXCUSE",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EXCUSE_exam_id",
                table: "EXCUSE",
                column: "exam_id");

            migrationBuilder.AddForeignKey(
                name: "FK_EXCUSE_EXAM_exam_id",
                table: "EXCUSE",
                column: "exam_id",
                principalTable: "EXAM",
                principalColumn: "exam_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EXCUSE_EXAM_exam_id",
                table: "EXCUSE");

            migrationBuilder.DropIndex(
                name: "IX_EXCUSE_exam_id",
                table: "EXCUSE");

            migrationBuilder.DropColumn(
                name: "participation_notification",
                table: "STUDENT_EXAM");

            migrationBuilder.DropColumn(
                name: "reminder_email_sent",
                table: "STUDENT_EXAM");

            migrationBuilder.AlterColumn<string>(
                name: "student_no_exam_id",
                table: "STUDENT_EXAM",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "exam_id",
                table: "EXCUSE",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
