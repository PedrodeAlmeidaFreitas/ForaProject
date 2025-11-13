using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForaProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cik = table.Column<int>(type: "int", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StandardFundableAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SpecialFundableAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncomeRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Form = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Frame = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FiledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccessionNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeRecords_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Cik",
                table: "Companies",
                column: "Cik",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncomeRecords_CompanyId",
                table: "IncomeRecords",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeRecords_CompanyId_Year",
                table: "IncomeRecords",
                columns: new[] { "CompanyId", "Year" });

            migrationBuilder.CreateIndex(
                name: "IX_IncomeRecords_Year",
                table: "IncomeRecords",
                column: "Year");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomeRecords");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
