using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Negin.Infra.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class addVesselStoppage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingLineCompany_AspNetUsers_CreatedById",
                schema: "Basic",
                table: "ShippingLineCompany");

            migrationBuilder.DropForeignKey(
                name: "FK_Vessels_AspNetUsers_CreatedById",
                schema: "Basic",
                table: "Vessels");

            migrationBuilder.DropForeignKey(
                name: "FK_Voyages_Ports_NextPortId",
                schema: "Basic",
                table: "Voyages");

            migrationBuilder.DropForeignKey(
                name: "FK_Voyages_Ports_OriginPortId",
                schema: "Basic",
                table: "Voyages");

            migrationBuilder.DropForeignKey(
                name: "FK_Voyages_Ports_PreviousPortId",
                schema: "Basic",
                table: "Voyages");

            migrationBuilder.DropIndex(
                name: "IX_Voyages_NextPortId",
                schema: "Basic",
                table: "Voyages");

            migrationBuilder.DropIndex(
                name: "IX_Voyages_OriginPortId",
                schema: "Basic",
                table: "Voyages");

            migrationBuilder.DropIndex(
                name: "IX_Voyages_PreviousPortId",
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

            migrationBuilder.DropColumn(
                name: "ATA",
                schema: "Basic",
                table: "Voyages");

            migrationBuilder.DropColumn(
                name: "ATD",
                schema: "Basic",
                table: "Voyages");

            migrationBuilder.DropColumn(
                name: "ETA",
                schema: "Basic",
                table: "Voyages");

            migrationBuilder.DropColumn(
                name: "ETD",
                schema: "Basic",
                table: "Voyages");

            migrationBuilder.DropColumn(
                name: "NextPortId",
                schema: "Basic",
                table: "Voyages");

            migrationBuilder.DropColumn(
                name: "OriginPortId",
                schema: "Basic",
                table: "Voyages");

            migrationBuilder.DropColumn(
                name: "PreviousPortId",
                schema: "Basic",
                table: "Voyages");

            migrationBuilder.EnsureSchema(
                name: "Billing");

            migrationBuilder.CreateTable(
                name: "VesselStoppage",
                schema: "Billing",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoyageId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
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
                    table.PrimaryKey("PK_VesselStoppage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VesselStoppage_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VesselStoppage_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VesselStoppage_Ports_NextPortId",
                        column: x => x.NextPortId,
                        principalSchema: "Basic",
                        principalTable: "Ports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VesselStoppage_Ports_OriginPortId",
                        column: x => x.OriginPortId,
                        principalSchema: "Basic",
                        principalTable: "Ports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VesselStoppage_Ports_PreviousPortId",
                        column: x => x.PreviousPortId,
                        principalSchema: "Basic",
                        principalTable: "Ports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VesselStoppage_Voyages_VoyageId",
                        column: x => x.VoyageId,
                        principalSchema: "Basic",
                        principalTable: "Voyages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "08f67744-d10c-45a0-afa2-d56c585a11db", 0, "adcf00b8-b748-41e1-885e-1ba78f8b014b", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "qbjtGQpUXg0VcfDTXH27G8wwhgO5h6xUCBUDCKrvF2Q=", null, false, "1eafad85-9777-4a62-9ecb-1125cd562680", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "08f67744-d10c-45a0-afa2-d56c585a11db" });

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppage_CreatedById",
                schema: "Billing",
                table: "VesselStoppage",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppage_ModifiedById",
                schema: "Billing",
                table: "VesselStoppage",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppage_NextPortId",
                schema: "Billing",
                table: "VesselStoppage",
                column: "NextPortId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppage_OriginPortId",
                schema: "Billing",
                table: "VesselStoppage",
                column: "OriginPortId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppage_PreviousPortId",
                schema: "Billing",
                table: "VesselStoppage",
                column: "PreviousPortId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppage_VoyageId",
                schema: "Billing",
                table: "VesselStoppage",
                column: "VoyageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingLineCompany_AspNetUsers_CreatedById",
                schema: "Basic",
                table: "ShippingLineCompany",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Vessels_AspNetUsers_CreatedById",
                schema: "Basic",
                table: "Vessels",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingLineCompany_AspNetUsers_CreatedById",
                schema: "Basic",
                table: "ShippingLineCompany");

            migrationBuilder.DropForeignKey(
                name: "FK_Vessels_AspNetUsers_CreatedById",
                schema: "Basic",
                table: "Vessels");

            migrationBuilder.DropTable(
                name: "VesselStoppage",
                schema: "Billing");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "08f67744-d10c-45a0-afa2-d56c585a11db" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "08f67744-d10c-45a0-afa2-d56c585a11db");

            migrationBuilder.AddColumn<DateTime>(
                name: "ATA",
                schema: "Basic",
                table: "Voyages",
                type: "smalldatetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ATD",
                schema: "Basic",
                table: "Voyages",
                type: "smalldatetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ETA",
                schema: "Basic",
                table: "Voyages",
                type: "smalldatetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ETD",
                schema: "Basic",
                table: "Voyages",
                type: "smalldatetime",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NextPortId",
                schema: "Basic",
                table: "Voyages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OriginPortId",
                schema: "Basic",
                table: "Voyages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PreviousPortId",
                schema: "Basic",
                table: "Voyages",
                type: "bigint",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "98f199bf-ebbb-49da-ba4f-3786f2c2c38c", 0, "7bdc3a3b-f598-466f-9be6-78cd038505e7", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "7sqeQGlsN9Xz60SEEATfOqVBIhxcwsls96tq2qq6aV8=", null, false, "71047f8b-77d9-4fd0-b7a1-6344d2973c86", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "98f199bf-ebbb-49da-ba4f-3786f2c2c38c" });

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
                name: "IX_Voyages_PreviousPortId",
                schema: "Basic",
                table: "Voyages",
                column: "PreviousPortId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingLineCompany_AspNetUsers_CreatedById",
                schema: "Basic",
                table: "ShippingLineCompany",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vessels_AspNetUsers_CreatedById",
                schema: "Basic",
                table: "Vessels",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Voyages_Ports_NextPortId",
                schema: "Basic",
                table: "Voyages",
                column: "NextPortId",
                principalSchema: "Basic",
                principalTable: "Ports",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Voyages_Ports_OriginPortId",
                schema: "Basic",
                table: "Voyages",
                column: "OriginPortId",
                principalSchema: "Basic",
                principalTable: "Ports",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Voyages_Ports_PreviousPortId",
                schema: "Basic",
                table: "Voyages",
                column: "PreviousPortId",
                principalSchema: "Basic",
                principalTable: "Ports",
                principalColumn: "Id");
        }
    }
}
