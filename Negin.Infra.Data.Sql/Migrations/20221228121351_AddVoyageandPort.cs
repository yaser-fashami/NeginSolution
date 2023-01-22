using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Negin.Infra.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddVoyageandPort : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "1413ab64-b60c-4f5f-88a3-71352af6133a" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1413ab64-b60c-4f5f-88a3-71352af6133a");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "Basic",
                table: "ShippingLineCompany",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Ports",
                schema: "Basic",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    PortName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PortEnglishName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    PortSymbol = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ports_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "Basic",
                        principalTable: "Countries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Voyages",
                schema: "Basic",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoyageNoIn = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    VoyageNoOut = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    VesselId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    AgentId = table.Column<long>(type: "bigint", nullable: false),
                    ETA = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    ATA = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    ETD = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    ATD = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    OriginPortId = table.Column<long>(type: "bigint", nullable: true),
                    PreviousPortId = table.Column<long>(type: "bigint", nullable: true),
                    NextPortId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voyages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Voyages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Voyages_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Voyages_Ports_NextPortId",
                        column: x => x.NextPortId,
                        principalSchema: "Basic",
                        principalTable: "Ports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Voyages_Ports_OriginPortId",
                        column: x => x.OriginPortId,
                        principalSchema: "Basic",
                        principalTable: "Ports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Voyages_Ports_PreviousPortId",
                        column: x => x.PreviousPortId,
                        principalSchema: "Basic",
                        principalTable: "Ports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Voyages_ShippingLineCompany_AgentId",
                        column: x => x.AgentId,
                        principalSchema: "Basic",
                        principalTable: "ShippingLineCompany",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Voyages_ShippingLineCompany_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Basic",
                        principalTable: "ShippingLineCompany",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Voyages_Vessels_VesselId",
                        column: x => x.VesselId,
                        principalSchema: "Basic",
                        principalTable: "Vessels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "c114fe61-efda-45d3-b5c7-213aa48a80b2", 0, "e69c9186-611f-4748-91ba-5f7629142b3c", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3", null, false, "48cd4ac8-38ab-485d-8802-00c2508045bf", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "c114fe61-efda-45d3-b5c7-213aa48a80b2" });

            migrationBuilder.CreateIndex(
                name: "IX_Ports_CountryId",
                schema: "Basic",
                table: "Ports",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Voyages_AgentId",
                schema: "Basic",
                table: "Voyages",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Voyages_CreatedById",
                schema: "Basic",
                table: "Voyages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Voyages_ModifiedById",
                schema: "Basic",
                table: "Voyages",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Voyages_NextPortId",
                schema: "Basic",
                table: "Voyages",
                column: "NextPortId");

            migrationBuilder.CreateIndex(
                name: "IX_Voyages_OriginPortId",
                schema: "Basic",
                table: "Voyages",
                column: "OriginPortId");

            migrationBuilder.CreateIndex(
                name: "IX_Voyages_OwnerId",
                schema: "Basic",
                table: "Voyages",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Voyages_PreviousPortId",
                schema: "Basic",
                table: "Voyages",
                column: "PreviousPortId");

            migrationBuilder.CreateIndex(
                name: "IX_Voyages_VesselId",
                schema: "Basic",
                table: "Voyages",
                column: "VesselId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Voyages",
                schema: "Basic");

            migrationBuilder.DropTable(
                name: "Ports",
                schema: "Basic");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "c114fe61-efda-45d3-b5c7-213aa48a80b2" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c114fe61-efda-45d3-b5c7-213aa48a80b2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "Basic",
                table: "ShippingLineCompany",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "1413ab64-b60c-4f5f-88a3-71352af6133a", 0, "26d81483-da96-4304-b13b-e7012542427f", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3", null, false, "a197bac6-1819-44b9-9eea-54b8ae058337", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "1413ab64-b60c-4f5f-88a3-71352af6133a" });
        }
    }
}
