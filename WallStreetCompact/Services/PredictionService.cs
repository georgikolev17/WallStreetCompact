using Microsoft.EntityFrameworkCore;
using WallStreetCompact.Data;
using WallStreetCompact.Models;

namespace WallStreetCompact.Services
{
    public class PredictionService : IPredictionService
    {
        private readonly ApplicationDbContext db;
        public PredictionService()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=aspnet-WallStreetCompact-E55E9634-0CC7-4F1B-B2A7-FB71238526D3;Trusted_Connection=True;MultipleActiveResultSets=true");
            this.db= new ApplicationDbContext(optionsBuilder.Options);
        }

        public async Task CreatePredictionAsync(double price, int id)
        {
            await this.db.AddAsync(new Prediction
            {
                Price = price,
                CompanyOverviewId = id,
            });
            await this.db.SaveChangesAsync();
        }

        public List<Prediction> GetAllPredictions()
        {
            return this.db.Predictions.ToList();
        }
    }
}
