using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ACADEMICIAN",
                columns: table => new
                {
                    academician_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    first_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    academician_email = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    academician_phone = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACADEMICIAN", x => x.academician_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ADVISOR",
                columns: table => new
                {
                    advisor_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name_surname = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    advisor_mail = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADVISOR", x => x.advisor_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SUBJECT",
                columns: table => new
                {
                    subject_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    subject_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    academician_id = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SUBJECT", x => x.subject_id);
                    table.ForeignKey(
                        name: "FK_SUBJECT_ACADEMICIAN_academician_id",
                        column: x => x.academician_id,
                        principalTable: "ACADEMICIAN",
                        principalColumn: "academician_id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "STUDENT",
                columns: table => new
                {
                    student_no = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    advisor_id = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name_surname = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    grade = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GPA = table.Column<decimal>(type: "DECIMAL(3,2)", nullable: false),
                    student_mail = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STUDENT", x => x.student_no);
                    table.ForeignKey(
                        name: "FK_STUDENT_ADVISOR_advisor_id",
                        column: x => x.advisor_id,
                        principalTable: "ADVISOR",
                        principalColumn: "advisor_id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EXAM",
                columns: table => new
                {
                    exam_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    subject_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    exam_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    exam_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    exam_description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXAM", x => x.exam_id);
                    table.ForeignKey(
                        name: "FK_EXAM_SUBJECT_subject_id",
                        column: x => x.subject_id,
                        principalTable: "SUBJECT",
                        principalColumn: "subject_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EXCUSE",
                columns: table => new
                {
                    excuse_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    student_no = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    advisor_id = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    exam_id = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    excuse_description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    request_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    response_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    document_path = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXCUSE", x => x.excuse_id);
                    table.ForeignKey(
                        name: "FK_EXCUSE_ADVISOR_advisor_id",
                        column: x => x.advisor_id,
                        principalTable: "ADVISOR",
                        principalColumn: "advisor_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EXCUSE_STUDENT_student_no",
                        column: x => x.student_no,
                        principalTable: "STUDENT",
                        principalColumn: "student_no",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "STUDENT_SUBJECT",
                columns: table => new
                {
                    student_no_subject_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    subject_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    student_no = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STUDENT_SUBJECT", x => x.student_no_subject_id);
                    table.ForeignKey(
                        name: "FK_STUDENT_SUBJECT_STUDENT_student_no",
                        column: x => x.student_no,
                        principalTable: "STUDENT",
                        principalColumn: "student_no",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_SUBJECT_SUBJECT_subject_id",
                        column: x => x.subject_id,
                        principalTable: "SUBJECT",
                        principalColumn: "subject_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "STUDENT_EXAM",
                columns: table => new
                {
                    student_no_exam_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    student_no = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    exam_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    participation_status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    consistency_note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STUDENT_EXAM", x => x.student_no_exam_id);
                    table.ForeignKey(
                        name: "FK_STUDENT_EXAM_EXAM_exam_id",
                        column: x => x.exam_id,
                        principalTable: "EXAM",
                        principalColumn: "exam_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_EXAM_STUDENT_student_no",
                        column: x => x.student_no,
                        principalTable: "STUDENT",
                        principalColumn: "student_no",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ACADEMICIAN_academician_email",
                table: "ACADEMICIAN",
                column: "academician_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ADVISOR_advisor_mail",
                table: "ADVISOR",
                column: "advisor_mail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EXAM_subject_id",
                table: "EXAM",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_EXCUSE_advisor_id",
                table: "EXCUSE",
                column: "advisor_id");

            migrationBuilder.CreateIndex(
                name: "IX_EXCUSE_student_no",
                table: "EXCUSE",
                column: "student_no");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_advisor_id",
                table: "STUDENT",
                column: "advisor_id");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_student_mail",
                table: "STUDENT",
                column: "student_mail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_EXAM_exam_id",
                table: "STUDENT_EXAM",
                column: "exam_id");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_EXAM_student_no",
                table: "STUDENT_EXAM",
                column: "student_no");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_SUBJECT_student_no",
                table: "STUDENT_SUBJECT",
                column: "student_no");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_SUBJECT_subject_id",
                table: "STUDENT_SUBJECT",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_SUBJECT_academician_id",
                table: "SUBJECT",
                column: "academician_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EXCUSE");

            migrationBuilder.DropTable(
                name: "STUDENT_EXAM");

            migrationBuilder.DropTable(
                name: "STUDENT_SUBJECT");

            migrationBuilder.DropTable(
                name: "EXAM");

            migrationBuilder.DropTable(
                name: "STUDENT");

            migrationBuilder.DropTable(
                name: "SUBJECT");

            migrationBuilder.DropTable(
                name: "ADVISOR");

            migrationBuilder.DropTable(
                name: "ACADEMICIAN");
        }
    }
}
