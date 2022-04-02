using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WallStreetCompact.Data.Migrations
{
    public partial class RemovePortfoloToStockRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Portfolios_PortfolioId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_PortfolioId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "PortfolioId",
                table: "Stocks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PortfolioId",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_PortfolioId",
                table: "Stocks",
                column: "PortfolioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Portfolios_PortfolioId",
                table: "Stocks",
                column: "PortfolioId",
                principalTable: "Portfolios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
