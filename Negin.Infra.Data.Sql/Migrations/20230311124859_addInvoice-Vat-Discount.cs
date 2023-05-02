using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Negin.Infra.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class addInvoiceVatDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "06b26aed-28a0-4752-935f-8f3d43360c79" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "06b26aed-28a0-4752-935f-8f3d43360c79");

            migrationBuilder.EnsureSchema(
                name: "Billing");

            migrationBuilder.CreateTable(
                name: "DiscountTariff",
                schema: "Basic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FlagId = table.Column<int>(type: "int", nullable: true),
                    ToGrossTonage = table.Column<long>(type: "bigint", nullable: true),
                    DiscountPercent = table.Column<double>(type: "float", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountTariff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountTariff_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiscountTariff_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiscountTariff_Countries_FlagId",
                        column: x => x.FlagId,
                        principalSchema: "Basic",
                        principalTable: "Countries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                schema: "Billing",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNo = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VoyageId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    IsPaied = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_Voyages_VoyageId",
                        column: x => x.VoyageId,
                        principalSchema: "Basic",
                        principalTable: "Voyages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VatTariff",
                schema: "Basic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    TollRate = table.Column<double>(type: "float", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VatTariff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VatTariff_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VatTariff_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceDetails",
                schema: "Billing",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    VesselStoppageId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    VesselStoppageTariffId = table.Column<int>(type: "int", nullable: false),
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
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
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
                name: "IX_DiscountTariff_CreatedById",
                schema: "Basic",
                table: "DiscountTariff",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountTariff_FlagId",
                schema: "Basic",
                table: "DiscountTariff",
                column: "FlagId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountTariff_ModifiedById",
                schema: "Basic",
                table: "DiscountTariff",
                column: "ModifiedById");

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

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CreatedById",
                schema: "Billing",
                table: "Invoices",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ModifiedById",
                schema: "Billing",
                table: "Invoices",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_VoyageId",
                schema: "Billing",
                table: "Invoices",
                column: "VoyageId");

            migrationBuilder.CreateIndex(
                name: "IX_VatTariff_CreatedById",
                schema: "Basic",
                table: "VatTariff",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VatTariff_ModifiedById",
                schema: "Basic",
                table: "VatTariff",
                column: "ModifiedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscountTariff",
                schema: "Basic");

            migrationBuilder.DropTable(
                name: "InvoiceDetails",
                schema: "Billing");

            migrationBuilder.DropTable(
                name: "VatTariff",
                schema: "Basic");

            migrationBuilder.DropTable(
                name: "Invoices",
                schema: "Billing");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "a20720c4-0edd-4fb2-9c9c-0065b2d0e776" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a20720c4-0edd-4fb2-9c9c-0065b2d0e776");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreateDate", "Email", "EmailConfirmed", "FirstName", "IsActived", "LastLogInDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "06b26aed-28a0-4752-935f-8f3d43360c79", 0, "e1f1c391-adc9-45ed-9cc9-76e9cfd88f29", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, false, null, null, null, "tGY/+dga2x7kbCTLx1iDVhcPzz4OmFyJPOLyVKddecA=", null, false, "f6f4fba8-d5e8-447a-b98e-743d1db2c967", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "06b26aed-28a0-4752-935f-8f3d43360c79" });
        }
    }
}
