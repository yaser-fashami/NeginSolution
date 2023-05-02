using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Negin.Infra.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class addUserAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "ba6327b6-db02-44c4-b7b9-896a34945c56" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ba6327b6-db02-44c4-b7b9-896a34945c56");

            migrationBuilder.AddColumn<string>(
                name: "UserAvatar",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "blank.png");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "408dc71b-4a06-4c17-be33-b12a8c9ccc69", 0, "736f3d66-fe37-4595-926d-817eda52152a", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "KN2dFrXZrplk2BcLgsp09+2bILqhgI6xK9hQC5lI/Lo=", null, false, "c574a3d6-0458-465a-a9b8-cdc1fd7892ff", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "408dc71b-4a06-4c17-be33-b12a8c9ccc69" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "408dc71b-4a06-4c17-be33-b12a8c9ccc69" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "408dc71b-4a06-4c17-be33-b12a8c9ccc69");

            migrationBuilder.DropColumn(
                name: "UserAvatar",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "ba6327b6-db02-44c4-b7b9-896a34945c56", 0, "62f81727-d2fa-4b51-8adb-05f4154a5b6d", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "uo3okL0WvmROimsb/i7R3obVurNf2sJI2JgT+Cj1hlU=", null, false, "a8758b98-1a6f-44fb-b08a-c52efaa76c8f", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "ba6327b6-db02-44c4-b7b9-896a34945c56" });
        }
    }
}
