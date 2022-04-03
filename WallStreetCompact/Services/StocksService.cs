using Microsoft.EntityFrameworkCore;
using WallStreetCompact.Data;
using WallStreetCompact.Models;

namespace WallStreetCompact.Services
{
    public class StocksService : IStocksService
    {
        private readonly ApplicationDbContext db;
        public StocksService()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=aspnet-WallStreetCompact-E55E9634-0CC7-4F1B-B2A7-FB71238526D3;Trusted_Connection=True;MultipleActiveResultSets=true");
            this.db= new ApplicationDbContext(optionsBuilder.Options);
        }

        public async Task CreateStockAsync(Stock stock)
        {
            await this.db.Stocks.AddAsync(stock);
            await this.db.SaveChangesAsync();
        }

        public List<Stock> GetAllStocks()
        {
            return this.db.Stocks.ToList();
        }
    }
}
