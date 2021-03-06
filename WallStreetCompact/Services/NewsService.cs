using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WallStreetCompact.Data;
using WallStreetCompact.Models;

namespace WallStreetCompact.Services
{
    public class NewsService : INewsService
    {
        private readonly ApplicationDbContext db;

        public NewsService()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=aspnet-WallStreetCompact-E55E9634-0CC7-4F1B-B2A7-FB71238526D3;Trusted_Connection=True;MultipleActiveResultSets=true");
            this.db= new ApplicationDbContext(optionsBuilder.Options);
        }

        public async Task CreateNewsAsync(News news)
        {
            await this.db.News.AddAsync(news);
            await this.db.SaveChangesAsync();
        }

        public List<News> GetAllNews()
        {
            return this.db.News.ToList();
        }
    }
}
