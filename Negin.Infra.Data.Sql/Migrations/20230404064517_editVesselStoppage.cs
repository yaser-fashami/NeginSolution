using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Negin.Infra.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class editVesselStoppage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CleaningServiceInvoiceDetails_VesselStoppageId",
                schema: "Billing",
                table: "CleaningServiceInvoiceDetails");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "2f308e89-dc31-4eb0-8260-48283f1f3062" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2f308e89-dc31-4eb0-8260-48283f1f3062");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f886dd58-9a6a-4dc2-966d-5878350630d3", 0, "7a144623-47a0-4217-96af-076942e63f76", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "V1XR9c+GxrztWapRrA0QBOThBAwaTaLMEBM8fPCoZbo=", null, false, "a77d4fc3-8a86-43f4-b4d9-5af9195fe0ed", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "f886dd58-9a6a-4dc2-966d-5878350630d3" });

            migrationBuilder.CreateIndex(
                name: "IX_CleaningServiceInvoiceDetails_VesselStoppageId",
                schema: "Billing",
                table: "CleaningServiceInvoiceDetails",
                column: "VesselStoppageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CleaningServiceInvoiceDetails_VesselStoppageId",
                schema: "Billing",
                table: "CleaningServiceInvoiceDetails");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "f886dd58-9a6a-4dc2-966d-5878350630d3" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f886dd58-9a6a-4dc2-966d-5878350630d3");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "2f308e89-dc31-4eb0-8260-48283f1f3062", 0, "e6d69f35-5347-4702-a859-532fa8bb185b", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "BAlXxPWdiBrRVO/2gw3HjMXTN2KvpkL8Pef/0MPJ7ho=", null, false, "c7dbc50a-e7fc-43ab-b95e-6fe02e6ea947", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "2f308e89-dc31-4eb0-8260-48283f1f3062" });

            migrationBuilder.CreateIndex(
                name: "IX_CleaningServiceInvoiceDetails_VesselStoppageId",
                schema: "Billing",
                table: "CleaningServiceInvoiceDetails",
                column: "VesselStoppageId");
        }
    }
}
