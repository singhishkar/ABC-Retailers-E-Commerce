using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ABCRetail.Migrations
{
    /// <inheritdoc />
    public partial class NewMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaimsPeriodEnd",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "ClaimsPeriodStart",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "HoursWorked",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "RatePerHour",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Claims");

            migrationBuilder.RenameColumn(
                name: "LecturerID",
                table: "Claims",
                newName: "DocumentName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DocumentName",
                table: "Claims",
                newName: "LecturerID");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClaimsPeriodEnd",
                table: "Claims",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ClaimsPeriodStart",
                table: "Claims",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "HoursWorked",
                table: "Claims",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "RatePerHour",
                table: "Claims",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalAmount",
                table: "Claims",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
