using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Negin.Infra.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class addUniqueIndexonVoyage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "00ecbcad-4e87-4f14-b03d-31e90555453b" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00ecbcad-4e87-4f14-b03d-31e90555453b");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "98f199bf-ebbb-49da-ba4f-3786f2c2c38c", 0, "7bdc3a3b-f598-466f-9be6-78cd038505e7", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "7sqeQGlsN9Xz60SEEATfOqVBIhxcwsls96tq2qq6aV8=", null, false, "71047f8b-77d9-4fd0-b7a1-6344d2973c86", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "98f199bf-ebbb-49da-ba4f-3786f2c2c38c" });

            migrationBuilder.CreateIndex(
                name: "IX_Voyages_VoyageNoIn_VesselId",
                schema: "Basic",
                table: "Voyages",
                columns: new[] { "VoyageNoIn", "VesselId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Voyages_VoyageNoIn_VesselId",
                schema: "Basic",
                table: "Voyages");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "98f199bf-ebbb-49da-ba4f-3786f2c2c38c" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "98f199bf-ebbb-49da-ba4f-3786f2c2c38c");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "00ecbcad-4e87-4f14-b03d-31e90555453b", 0, "cdc560e7-558e-47eb-9dc1-c1eec8c1b53a", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "mCFsD2zsT5d5rYETpUCvIvIXzMPbDZM/IoZMYpVnvqQ=", null, false, "8c75e3cc-f6f8-4d6c-9655-6355d733feaa", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "00ecbcad-4e87-4f14-b03d-31e90555453b" });
        }
    }
}
