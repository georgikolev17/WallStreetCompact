using WallStreetCompact.Models;

namespace WallStreetCompact.Services
{
    public interface IStocksService
    {
        Task CreateStockAsync(Stock stock);
    }
}
