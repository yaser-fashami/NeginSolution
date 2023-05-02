using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Negin.Infra.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class editInvoiceDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceDetails",
                schema: "Billing");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "a20720c4-0edd-4fb2-9c9c-0065b2d0e776" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a20720c4-0edd-4fb2-9c9c-0065b2d0e776");

            migrationBuilder.CreateTable(
                name: "CleaningServiceInvoiceDetails",
                schema: "Billing",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    VesselStoppageId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    CleaningServiceTariffId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint", nullable: false),
                    DwellingHour = table.Column<long>(type: "bigint", nullable: false),
                    PriceR = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    PriceD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceRVat = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CleaningServiceInvoiceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CleaningServiceInvoiceDetails_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CleaningServiceInvoiceDetails_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CleaningServiceInvoiceDetails_CleaningServiceTariff_CleaningServiceTariffId",
                        column: x => x.CleaningServiceTariffId,
                        principalSchema: "Basic",
                        principalTable: "CleaningServiceTariff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CleaningServiceInvoiceDetails_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "Basic",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CleaningServiceInvoiceDetails_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalSchema: "Billing",
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CleaningServiceInvoiceDetails_VesselStoppage_VesselStoppageId",
                        column: x => x.VesselStoppageId,
                        principalSchema: "Operation",
                        principalTable: "VesselStoppage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "VesselStoppageInvoiceDetails",
                schema: "Billing",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    VesselStoppageId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    VesselStoppageTariffId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint", nullable: false),
                    DwellingHour = table.Column<long>(type: "bigint", nullable: false),
                    PriceR = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    PriceD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceRVat = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselStoppageInvoiceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VesselStoppageInvoiceDetails_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VesselStoppageInvoiceDetails_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VesselStoppageInvoiceDetails_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "Basic",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_VesselStoppageInvoiceDetails_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalSchema: "Billing",
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VesselStoppageInvoiceDetails_VesselStoppageTariff_VesselStoppageTariffId",
                        column: x => x.VesselStoppageTariffId,
                        principalSchema: "Basic",
                        principalTable: "VesselStoppageTariff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_VesselStoppageInvoiceDetails_VesselStoppage_VesselStoppageId",
                        column: x => x.VesselStoppageId,
                        principalSchema: "Operation",
                        principalTable: "VesselStoppage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "2f308e89-dc31-4eb0-8260-48283f1f3062", 0, "e6d69f35-5347-4702-a859-532fa8bb185b", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "BAlXxPWdiBrRVO/2gw3HjMXTN2KvpkL8Pef/0MPJ7ho=", null, false, "c7dbc50a-e7fc-43ab-b95e-6fe02e6ea947", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "2f308e89-dc31-4eb0-8260-48283f1f3062" });

            migrationBuilder.CreateIndex(
                name: "IX_CleaningServiceInvoiceDetails_CleaningServiceTariffId",
                schema: "Billing",
                table: "CleaningServiceInvoiceDetails",
                column: "CleaningServiceTariffId");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningServiceInvoiceDetails_CreatedById",
                schema: "Billing",
                table: "CleaningServiceInvoiceDetails",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningServiceInvoiceDetails_CurrencyId",
                schema: "Billing",
                table: "CleaningServiceInvoiceDetails",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningServiceInvoiceDetails_InvoiceId",
                schema: "Billing",
                table: "CleaningServiceInvoiceDetails",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningServiceInvoiceDetails_ModifiedById",
                schema: "Billing",
                table: "CleaningServiceInvoiceDetails",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningServiceInvoiceDetails_VesselStoppageId",
                schema: "Billing",
                table: "CleaningServiceInvoiceDetails",
                column: "VesselStoppageId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppageInvoiceDetails_CreatedById",
                schema: "Billing",
                table: "VesselStoppageInvoiceDetails",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppageInvoiceDetails_CurrencyId",
                schema: "Billing",
                table: "VesselStoppageInvoiceDetails",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppageInvoiceDetails_InvoiceId",
                schema: "Billing",
                table: "VesselStoppageInvoiceDetails",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppageInvoiceDetails_ModifiedById",
                schema: "Billing",
                table: "VesselStoppageInvoiceDetails",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppageInvoiceDetails_VesselStoppageId",
                schema: "Billing",
                table: "VesselStoppageInvoiceDetails",
                column: "VesselStoppageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VesselStoppageInvoiceDetails_VesselStoppageTariffId",
                schema: "Billing",
                table: "VesselStoppageInvoiceDetails",
                column: "VesselStoppageTariffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CleaningServiceInvoiceDetails",
                schema: "Billing");

            migrationBuilder.DropTable(
                name: "VesselStoppageInvoiceDetails",
                schema: "Billing");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "2f308e89-dc31-4eb0-8260-48283f1f3062" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2f308e89-dc31-4eb0-8260-48283f1f3062");

            migrationBuilder.AddColumn<int>(
                name: "CleaningServiceTariffId",
                schema: "Billing",
                table: "Invoices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VesselStoppageTariffId",
                schema: "Billing",
                table: "Invoices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InvoiceDetails",
                schema: "Billing",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CleaningServiceTariffId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CurrencyId = table.Column<long>(type: "bigint", nullable: false),
                    InvoiceId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    VesselStoppageId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    VesselStoppageTariffId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DwellingHour = table.Column<long>(type: "bigint", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PriceD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceR = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    PriceRVat = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_CleaningServiceTariff_CleaningServiceTariffId",
                        column: x => x.CleaningServiceTariffId,
                        principalSchema: "Basic",
                        principalTable: "CleaningServiceTariff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "Basic",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalSchema: "Billing",
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_VesselStoppageTariff_VesselStoppageTariffId",
                        column: x => x.VesselStoppageTariffId,
                        principalSchema: "Basic",
                        principalTable: "VesselStoppageTariff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_VesselStoppage_VesselStoppageId",
                        column: x => x.VesselStoppageId,
                        principalSchema: "Operation",
                        principalTable: "VesselStoppage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "a20720c4-0edd-4fb2-9c9c-0065b2d0e776", 0, "219664bc-692a-4a91-99bf-e4a83ce5d170", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "Top6SJ628gZWhkXXKh8rOML2ZL+XPfr1+ra7pJ4JbQI=", null, false, "bec61cc3-7dde-4078-9f8f-faedb9c834d9", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "a20720c4-0edd-4fb2-9c9c-0065b2d0e776" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CleaningServiceTariffId",
                schema: "Billing",
                table: "Invoices",
                column: "CleaningServiceTariffId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_VesselStoppageTariffId",
                schema: "Billing",
                table: "Invoices",
                column: "VesselStoppageTariffId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_CleaningServiceTariffId",
                schema: "Billing",
                table: "InvoiceDetails",
                column: "CleaningServiceTariffId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_CreatedById",
                schema: "Billing",
                table: "InvoiceDetails",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_CurrencyId",
                schema: "Billing",
                table: "InvoiceDetails",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoiceId",
                schema: "Billing",
                table: "InvoiceDetails",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_ModifiedById",
                schema: "Billing",
                table: "InvoiceDetails",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_VesselStoppageId",
                schema: "Billing",
                table: "InvoiceDetails",
                column: "VesselStoppageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_VesselStoppageTariffId",
                schema: "Billing",
                table: "InvoiceDetails",
                column: "VesselStoppageTariffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_CleaningServiceTariff_CleaningServiceTariffId",
                schema: "Billing",
                table: "Invoices",
                column: "CleaningServiceTariffId",
                principalSchema: "Basic",
                principalTable: "CleaningServiceTariff",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_VesselStoppageTariff_VesselStoppageTariffId",
                schema: "Billing",
                table: "Invoices",
                column: "VesselStoppageTariffId",
                principalSchema: "Basic",
                principalTable: "VesselStoppageTariff",
                principalColumn: "Id");
        }
    }
}
