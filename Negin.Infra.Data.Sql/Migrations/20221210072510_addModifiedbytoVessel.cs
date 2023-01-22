using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Negin.Infra.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class addModifiedbytoVessel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vessels_AspNetUsers_CreatedById",
                schema: "Basic",
                table: "Vessels");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "b95fc3c8-38ce-4286-95d8-f29bedb88cc8" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b95fc3c8-38ce-4286-95d8-f29bedb88cc8");

            migrationBuilder.AlterColumn<string>(
                name: "CallSign",
                schema: "Basic",
                table: "Vessels",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,0)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                schema: "Basic",
                table: "Vessels",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "0ae24bca-4618-46ff-b936-fefc5ca23a67", 0, "27138e5d-d29f-49f4-a84f-91f79881daa0", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3", null, false, "cc03d0a7-3cf0-4c7b-b7c3-8a540a7c41f9", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "0ae24bca-4618-46ff-b936-fefc5ca23a67" });

            migrationBuilder.CreateIndex(
                name: "IX_Vessels_ModifiedById",
                schema: "Basic",
                table: "Vessels",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Vessels_AspNetUsers_CreatedById",
                schema: "Basic",
                table: "Vessels",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vessels_AspNetUsers_ModifiedById",
                schema: "Basic",
                table: "Vessels",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vessels_AspNetUsers_CreatedById",
                schema: "Basic",
                table: "Vessels");

            migrationBuilder.DropForeignKey(
                name: "FK_Vessels_AspNetUsers_ModifiedById",
                schema: "Basic",
                table: "Vessels");

            migrationBuilder.DropIndex(
                name: "IX_Vessels_ModifiedById",
                schema: "Basic",
                table: "Vessels");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "0ae24bca-4618-46ff-b936-fefc5ca23a67" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0ae24bca-4618-46ff-b936-fefc5ca23a67");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                schema: "Basic",
                table: "Vessels");

            migrationBuilder.AlterColumn<decimal>(
                name: "CallSign",
                schema: "Basic",
                table: "Vessels",
                type: "decimal(20,0)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b95fc3c8-38ce-4286-95d8-f29bedb88cc8", 0, "6a0c84b5-caef-4d2f-988f-1f69abc0f042", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3", null, false, "cb78ac80-182c-40ea-a1af-48e4a9cad83f", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "b95fc3c8-38ce-4286-95d8-f29bedb88cc8" });

            migrationBuilder.AddForeignKey(
                name: "FK_Vessels_AspNetUsers_CreatedById",
                schema: "Basic",
                table: "Vessels",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
