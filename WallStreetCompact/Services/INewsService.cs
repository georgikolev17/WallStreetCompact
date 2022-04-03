using System.Threading.Tasks;
using WallStreetCompact.Models;

namespace WallStreetCompact.Services
{
    public interface INewsService
    {
        Task CreateNewsAsync(News news);
    }
}
