using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WallStreetCompact.Data.Migrations
{
    public partial class UpdateCompanyOverview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyOverviews_Stocks_StockId",
                table: "CompanyOverviews");

            migrationBuilder.DropIndex(
                name: "IX_CompanyOverviews_StockId",
                table: "CompanyOverviews");

            migrationBuilder.DropColumn(
                name: "StockId",
                table: "CompanyOverviews");

            migrationBuilder.RenameColumn(
                name: "PERatio",
                table: "CompanyOverviews",
                newName: "SharesOutstanding");

            migrationBuilder.RenameColumn(
                name: "PEGRatio",
                table: "CompanyOverviews",
                newName: "RevenuePerShareTTM");

            migrationBuilder.RenameColumn(
                name: "LatestQuarter",
                table: "CompanyOverviews",
                newName: "ProfitMargin");

            migrationBuilder.RenameColumn(
                name: "FiscalYearEnd",
                table: "CompanyOverviews",
                newName: "EVToRevenue");

            migrationBuilder.RenameColumn(
                name: "BookValue",
                table: "CompanyOverviews",
                newName: "EVToEBITDA");

            migrationBuilder.AlterColumn<string>(
                name: "AnalystTargetPrice",
                table: "CompanyOverviews",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SharesOutstanding",
                table: "CompanyOverviews",
                newName: "PERatio");

            migrationBuilder.RenameColumn(
                name: "RevenuePerShareTTM",
                table: "CompanyOverviews",
                newName: "PEGRatio");

            migrationBuilder.RenameColumn(
                name: "ProfitMargin",
                table: "CompanyOverviews",
                newName: "LatestQuarter");

            migrationBuilder.RenameColumn(
                name: "EVToRevenue",
                table: "CompanyOverviews",
                newName: "FiscalYearEnd");

            migrationBuilder.RenameColumn(
                name: "EVToEBITDA",
                table: "CompanyOverviews",
                newName: "BookValue");

            migrationBuilder.AlterColumn<decimal>(
                name: "AnalystTargetPrice",
                table: "CompanyOverviews",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "StockId",
                table: "CompanyOverviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOverviews_StockId",
                table: "CompanyOverviews",
                column: "StockId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyOverviews_Stocks_StockId",
                table: "CompanyOverviews",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
