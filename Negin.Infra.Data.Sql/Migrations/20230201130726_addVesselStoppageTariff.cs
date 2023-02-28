using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Negin.Infra.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class addVesselStoppageTariff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "28283831-e6b6-4e66-8c7f-7454d513027a" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "28283831-e6b6-4e66-8c7f-7454d513027a");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                schema: "Operation",
                table: "VesselStoppage");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                schema: "Basic",
                table: "Currencies");

            migrationBuilder.CreateTable(
                name: "VesselStoppageTariff",
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
                    table.PrimaryKey("PK_VesselStoppageTariff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VesselStoppageTariff_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VesselStoppageTariff_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VesselStoppageTariffDetails",
                schema: "Basic",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VesselStoppageTarrifId = table.Column<int>(type: "int", nullable: false),
                    VesselTypeId = table.Column<byte>(type: "tinyint", nullable: false),
                    NormalHour = table.Column<int>(type: "int", nullable: false),
                    NormalPrice = table.Column<double>(type: "float", nullable: false),
                    ExtraPrice = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselStoppageTariffDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VesselStoppageTariffDetails_VesselStoppageTariff_VesselStoppageTarrifId",
                        column: x => x.VesselStoppageTarrifId,
                        principalSchema: "Basic",
                        principalTable: "VesselStoppageTariff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VesselStoppageTariffDetails_VesselTypes_VesselTypeId",
                        column: x => x.VesselTypeId,
                        principalSchema: "Basic",
                        principalTable: "VesselTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "4b86cee8-7cab-4119-8314-7f147363c7d4", 0, "6b24d306-fa5a-47bb-b970-be6eae6f19c6", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "TNfsNLq8p+n1Dyf/Jqi/1NFVS4Kn6j+tIOJLUd/WdA8=", null, false, "386f167a-c9f1-455b-8607-a3d602c61444", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "4b86cee8-7cab-4119-8314-7f147363c7d4" });

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppageTariff_CreatedById",
                schema: "Basic",
                table: "VesselStoppageTariff",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppageTariff_ModifiedById",
                schema: "Basic",
                table: "VesselStoppageTariff",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppageTariffDetails_VesselStoppageTarrifId",
                schema: "Basic",
                table: "VesselStoppageTariffDetails",
                column: "VesselStoppageTarrifId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppageTariffDetails_VesselTypeId",
                schema: "Basic",
                table: "VesselStoppageTariffDetails",
                column: "VesselTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VesselStoppageTariffDetails",
                schema: "Basic");

            migrationBuilder.DropTable(
                name: "VesselStoppageTariff",
                schema: "Basic");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "4b86cee8-7cab-4119-8314-7f147363c7d4" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4b86cee8-7cab-4119-8314-7f147363c7d4");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                schema: "Operation",
                table: "VesselStoppage",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                schema: "Basic",
                table: "Currencies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "28283831-e6b6-4e66-8c7f-7454d513027a", 0, "5b8b2388-4a8f-4a5f-a844-621834aec9f4", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "W9IkAOpyc+tPHOmFQJ3tDhhAyoSuLF/nE28bA3YOGIg=", null, false, "549eb845-8f89-4156-b1c9-f011dd9a23df", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "28283831-e6b6-4e66-8c7f-7454d513027a" });
        }
    }
}
