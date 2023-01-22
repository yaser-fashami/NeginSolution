using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Negin.Infra.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingLineandAgents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "f0a5fea2-0d7a-40f3-89ae-67aaaf804490" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f0a5fea2-0d7a-40f3-89ae-67aaaf804490");

            migrationBuilder.CreateTable(
                name: "ShippingLineCompany",
                schema: "Basic",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingLineName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EconomicCode = table.Column<string>(type: "nchar(20)", fixedLength: true, maxLength: 20, nullable: false),
                    NationalCode = table.Column<string>(type: "nchar(20)", fixedLength: true, maxLength: 20, nullable: false),
                    Tel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsOwner = table.Column<bool>(type: "bit", nullable: false),
                    IsAgent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingLineCompany", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingLineCompany_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShippingLineCompany_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AgentShippingLine",
                schema: "Basic",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingLineCompanyId = table.Column<long>(type: "bigint", nullable: false),
                    AgentShippingLineCompanyId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentShippingLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentShippingLine_ShippingLineCompany_AgentShippingLineCompanyId",
                        column: x => x.AgentShippingLineCompanyId,
                        principalSchema: "Basic",
                        principalTable: "ShippingLineCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgentShippingLine_ShippingLineCompany_ShippingLineCompanyId",
                        column: x => x.ShippingLineCompanyId,
                        principalSchema: "Basic",
                        principalTable: "ShippingLineCompany",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "1413ab64-b60c-4f5f-88a3-71352af6133a", 0, "26d81483-da96-4304-b13b-e7012542427f", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3", null, false, "a197bac6-1819-44b9-9eea-54b8ae058337", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "1413ab64-b60c-4f5f-88a3-71352af6133a" });

            migrationBuilder.CreateIndex(
                name: "IX_AgentShippingLine_AgentShippingLineCompanyId",
                schema: "Basic",
                table: "AgentShippingLine",
                column: "AgentShippingLineCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentShippingLine_ShippingLineCompanyId",
                schema: "Basic",
                table: "AgentShippingLine",
                column: "ShippingLineCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingLineCompany_CreatedById",
                schema: "Basic",
                table: "ShippingLineCompany",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingLineCompany_ModifiedById",
                schema: "Basic",
                table: "ShippingLineCompany",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingLineCompany_ShippingLineName",
                schema: "Basic",
                table: "ShippingLineCompany",
                column: "ShippingLineName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentShippingLine",
                schema: "Basic");

            migrationBuilder.DropTable(
                name: "ShippingLineCompany",
                schema: "Basic");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "1413ab64-b60c-4f5f-88a3-71352af6133a" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1413ab64-b60c-4f5f-88a3-71352af6133a");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f0a5fea2-0d7a-40f3-89ae-67aaaf804490", 0, "59074833-2307-4649-aac2-3eb389e598d9", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3", null, false, "79e408fc-9689-4136-84bd-cb11e1371135", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "f0a5fea2-0d7a-40f3-89ae-67aaaf804490" });
        }
    }
}
