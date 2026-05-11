using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseNoteSharingSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCourseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_CourseName_Department",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "CourseName",
                table: "Courses",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Courses",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Courses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Courses",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Semester",
                table: "Courses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Courses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Code", "CreatedAt", "Description", "Semester", "UpdatedAt" },
                values: new object[] { "CENG201", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3716), "Fundamental data structures and algorithms.", "Fall", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3717) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Code", "CreatedAt", "Description", "Semester", "UpdatedAt" },
                values: new object[] { "CENG301", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3720), "Relational databases and SQL.", "Spring", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3720) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Code", "CreatedAt", "Description", "Semester", "UpdatedAt" },
                values: new object[] { "CENG302", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3721), "OS concepts and design.", "Fall", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3721) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Code", "CreatedAt", "Description", "Semester", "UpdatedAt" },
                values: new object[] { "CENG401", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3722), "Network protocols and architecture.", "Fall", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3722) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Code", "CreatedAt", "Description", "Semester", "UpdatedAt" },
                values: new object[] { "CENG402", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3723), "Software development lifecycle.", "Spring", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3723) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Code", "CreatedAt", "Description", "Semester", "UpdatedAt" },
                values: new object[] { "MATH101", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3724), "Differential and integral calculus.", "Fall", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3724) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Code", "CreatedAt", "Description", "Semester", "UpdatedAt" },
                values: new object[] { "MATH102", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3725), "Vector spaces and linear transformations.", "Spring", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3725) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Code", "CreatedAt", "Description", "Semester", "UpdatedAt" },
                values: new object[] { "PHYS101", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3726), "Mechanics and thermodynamics.", "Fall", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3726) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Code", "CreatedAt", "Description", "Semester", "UpdatedAt" },
                values: new object[] { "CENG101", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3727), "Basic programming concepts.", "Fall", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3727) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Code", "CreatedAt", "Description", "Semester", "UpdatedAt" },
                values: new object[] { "CENG403", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3728), "AI fundamentals and machine learning.", "Spring", new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3728) });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Code",
                table: "Courses",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_Code",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Semester",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Courses",
                newName: "CourseName");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseName_Department",
                table: "Courses",
                columns: new[] { "CourseName", "Department" },
                unique: true);
        }
    }
}
