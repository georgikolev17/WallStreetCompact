using WallStreetCompact.Models;

namespace WallStreetCompact.ViewModels
{
    public class PortfolioViewModel
    {
        public PortfolioViewModel()
        {
            this.Stocks = new List<Stock>();
            this.News = new List<News>();
            this.CompanyOverviews = new List<CompanyOverview>();
        }

        public ICollection<Stock> Stocks { get; set; }
        public ICollection<News> News { get; set; }
        public ICollection<CompanyOverview> CompanyOverviews { get; set; }
    }
}
