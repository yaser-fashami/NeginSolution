using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Negin.Infra.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class editInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "f886dd58-9a6a-4dc2-966d-5878350630d3" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f886dd58-9a6a-4dc2-966d-5878350630d3");

            migrationBuilder.AddColumn<int>(
                name: "ApplyCurrencyRate",
                schema: "Billing",
                table: "VesselStoppageInvoiceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ApplyExtraPrice",
                schema: "Billing",
                table: "VesselStoppageInvoiceDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ApplyNormalPrice",
                schema: "Billing",
                table: "VesselStoppageInvoiceDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "SumPriceD",
                schema: "Billing",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SumPriceR",
                schema: "Billing",
                table: "Invoices",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SumPriceRVat",
                schema: "Billing",
                table: "Invoices",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalDwellingHour",
                schema: "Billing",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApplyCurrencyRate",
                schema: "Billing",
                table: "CleaningServiceInvoiceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ApplyPrice",
                schema: "Billing",
                table: "CleaningServiceInvoiceDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "ba6327b6-db02-44c4-b7b9-896a34945c56", 0, "62f81727-d2fa-4b51-8adb-05f4154a5b6d", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "uo3okL0WvmROimsb/i7R3obVurNf2sJI2JgT+Cj1hlU=", null, false, "a8758b98-1a6f-44fb-b08a-c52efaa76c8f", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "ba6327b6-db02-44c4-b7b9-896a34945c56" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "ba6327b6-db02-44c4-b7b9-896a34945c56" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ba6327b6-db02-44c4-b7b9-896a34945c56");

            migrationBuilder.DropColumn(
                name: "ApplyCurrencyRate",
                schema: "Billing",
                table: "VesselStoppageInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "ApplyExtraPrice",
                schema: "Billing",
                table: "VesselStoppageInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "ApplyNormalPrice",
                schema: "Billing",
                table: "VesselStoppageInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "SumPriceD",
                schema: "Billing",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "SumPriceR",
                schema: "Billing",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "SumPriceRVat",
                schema: "Billing",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TotalDwellingHour",
                schema: "Billing",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ApplyCurrencyRate",
                schema: "Billing",
                table: "CleaningServiceInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "ApplyPrice",
                schema: "Billing",
                table: "CleaningServiceInvoiceDetails");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f886dd58-9a6a-4dc2-966d-5878350630d3", 0, "7a144623-47a0-4217-96af-076942e63f76", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "V1XR9c+GxrztWapRrA0QBOThBAwaTaLMEBM8fPCoZbo=", null, false, "a77d4fc3-8a86-43f4-b4d9-5af9195fe0ed", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "f886dd58-9a6a-4dc2-966d-5878350630d3" });

        }
    }
}
