using WallStreetCompact.Models;

namespace WallStreetCompact.Services
{
    public interface IDataSeeder
    {
        Dictionary<string, string> ReadFiles();

        Task SeedNews();

        Task SeedStocks();
    }
}
