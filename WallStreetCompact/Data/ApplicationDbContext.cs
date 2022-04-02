using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WallStreetCompact.Models;

namespace WallStreetCompact.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CompanyOverview> CompanyOverviews { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<News> News { get; set; }
    }
}