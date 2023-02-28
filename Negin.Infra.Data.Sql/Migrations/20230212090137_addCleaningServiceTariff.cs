using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Negin.Infra.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class addCleaningServiceTariff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "4b86cee8-7cab-4119-8314-7f147363c7d4" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4b86cee8-7cab-4119-8314-7f147363c7d4");

            migrationBuilder.CreateTable(
                name: "CleaningServiceTariff",
                schema: "Basic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CleaningServiceTariff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CleaningServiceTariff_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CleaningServiceTariff_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CleaningServiceTariffDetails",
                schema: "Basic",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CleaningServiceTariffId = table.Column<int>(type: "int", nullable: false),
                    GrossWeight = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Vat = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CleaningServiceTariffDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CleaningServiceTariffDetails_CleaningServiceTariff_CleaningServiceTariffId",
                        column: x => x.CleaningServiceTariffId,
                        principalSchema: "Basic",
                        principalTable: "CleaningServiceTariff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "06b26aed-28a0-4752-935f-8f3d43360c79", 0, "e1f1c391-adc9-45ed-9cc9-76e9cfd88f29", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "tGY/+dga2x7kbCTLx1iDVhcPzz4OmFyJPOLyVKddecA=", null, false, "f6f4fba8-d5e8-447a-b98e-743d1db2c967", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "06b26aed-28a0-4752-935f-8f3d43360c79" });

            migrationBuilder.CreateIndex(
                name: "IX_CleaningServiceTariff_CreatedById",
                schema: "Basic",
                table: "CleaningServiceTariff",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningServiceTariff_ModifiedById",
                schema: "Basic",
                table: "CleaningServiceTariff",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningServiceTariffDetails_CleaningServiceTariffId",
                schema: "Basic",
                table: "CleaningServiceTariffDetails",
                column: "CleaningServiceTariffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CleaningServiceTariffDetails",
                schema: "Basic");

            migrationBuilder.DropTable(
                name: "CleaningServiceTariff",
                schema: "Basic");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "06b26aed-28a0-4752-935f-8f3d43360c79" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "06b26aed-28a0-4752-935f-8f3d43360c79");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "4b86cee8-7cab-4119-8314-7f147363c7d4", 0, "6b24d306-fa5a-47bb-b970-be6eae6f19c6", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "TNfsNLq8p+n1Dyf/Jqi/1NFVS4Kn6j+tIOJLUd/WdA8=", null, false, "386f167a-c9f1-455b-8607-a3d602c61444", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "4b86cee8-7cab-4119-8314-7f147363c7d4" });
        }
    }
}
