using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class SeedDailyReminders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DailyRemiders",
                columns: new[] { "DailyRemiderId", "Recurrence", "Text" },
                values: new object[,]
                {
                    { 1, 1, "Drink 2L Water" },
                    { 2, 1, "Morning Meditation" },
                    { 3, 1, "Read 10 Pages" },
                    { 4, 1, "Evening Walk" },
                    { 6, 1, "Practice Coding for 30 Minutes" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DailyRemiders",
                keyColumn: "DailyRemiderId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DailyRemiders",
                keyColumn: "DailyRemiderId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DailyRemiders",
                keyColumn: "DailyRemiderId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DailyRemiders",
                keyColumn: "DailyRemiderId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "DailyRemiders",
                keyColumn: "DailyRemiderId",
                keyValue: 6);
        }
    }
}
