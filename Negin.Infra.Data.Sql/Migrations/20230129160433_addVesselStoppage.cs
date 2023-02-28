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
                name: "Operation");

            migrationBuilder.CreateTable(
                name: "VesselStoppage",
                schema: "Operation",
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
                values: new object[] { "0d284c30-0fd3-49bd-9efd-94a2c8550f47", 0, "652836df-c611-40c2-a613-57239cc7d720", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "A/a/bMO08WdyYnpFwlZubLb5JdQcy5IdT+vZ3ffwNjc=", null, false, "30971e60-44d9-4678-ad9b-daf61c0d09c2", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "0d284c30-0fd3-49bd-9efd-94a2c8550f47" });

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppage_CreatedById",
                schema: "Operation",
                table: "VesselStoppage",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppage_ModifiedById",
                schema: "Operation",
                table: "VesselStoppage",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppage_NextPortId",
                schema: "Operation",
                table: "VesselStoppage",
                column: "NextPortId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppage_OriginPortId",
                schema: "Operation",
                table: "VesselStoppage",
                column: "OriginPortId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppage_PreviousPortId",
                schema: "Operation",
                table: "VesselStoppage",
                column: "PreviousPortId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppage_VoyageId",
                schema: "Operation",
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
                schema: "Operation");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "0d284c30-0fd3-49bd-9efd-94a2c8550f47" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0d284c30-0fd3-49bd-9efd-94a2c8550f47");

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
