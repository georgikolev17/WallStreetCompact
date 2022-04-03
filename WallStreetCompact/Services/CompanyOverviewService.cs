using Microsoft.EntityFrameworkCore;
using WallStreetCompact.Data;
using WallStreetCompact.Models;

namespace WallStreetCompact.Services
{
    public class CompanyOverviewService : ICompanyOverviewService
    {
        private readonly ApplicationDbContext db;
        public CompanyOverviewService()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=aspnet-WallStreetCompact-E55E9634-0CC7-4F1B-B2A7-FB71238526D3;Trusted_Connection=True;MultipleActiveResultSets=true");
            this.db= new ApplicationDbContext(optionsBuilder.Options);
        }

        public async Task CreateCompanyOverviewAsync(CompanyOverview companyOverview)
        {
            await this.db.AddAsync(companyOverview);
            await this.db.SaveChangesAsync();
        }
    }
}
