using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WallStreetCompact.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Portfolios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Portfolios_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ticker = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Close = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    High = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Low = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Volume = table.Column<long>(type: "bigint", nullable: false),
                    NumberOfTransactions = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PortfolioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_Portfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "Portfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyOverviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ticker = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssetType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CIK = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Exchange = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sector = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Industry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FiscalYearEnd = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatestQuarter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarketCapitalization = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EBITDA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PERatio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PEGRatio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DividendPerShare = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DividendYield = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FiftyTwoWeekHigh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FiftyTwoWeekLow = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FiftyDayMovingAverage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TwoHundredDayMovingAverage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnalystTargetPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyOverviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyOverviews_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOverviews_StockId",
                table: "CompanyOverviews",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_UserId1",
                table: "Portfolios",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_PortfolioId",
                table: "Stocks",
                column: "PortfolioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyOverviews");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "Portfolios");
        }
    }
}
