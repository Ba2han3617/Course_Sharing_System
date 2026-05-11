using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseNoteSharingSystem.Migrations
{
    /// <inheritdoc />
    public partial class SyncCourseFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Courses",
                newName: "CourseName");

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4986), new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4987) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4990), new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4990) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4992), new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4992) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4993), new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4993) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4994), new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4994) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4995), new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4995) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4996), new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4996) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4997), new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4997) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4998), new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4998) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4999), new DateTime(2026, 5, 11, 15, 49, 43, 614, DateTimeKind.Utc).AddTicks(4999) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CourseName",
                table: "Courses",
                newName: "Name");

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3716), new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3717) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3720), new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3720) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3721), new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3721) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3722), new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3722) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3723), new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3723) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3724), new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3724) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3725), new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3725) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3726), new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3726) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3727), new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3727) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3728), new DateTime(2026, 5, 11, 15, 31, 10, 62, DateTimeKind.Utc).AddTicks(3728) });
        }
    }
}
