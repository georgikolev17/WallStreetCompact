using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WallStreetCompact.Models;
using WallStreetCompact.Services;
using WallStreetCompact.ViewModels;

namespace WallStreetCompact.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDataSeeder dataSeeder;
        private readonly IStocksService stocksService;
        private readonly INewsService newsService;
        private readonly ICompanyOverviewService companyOverviewService;

        public HomeController(ILogger<HomeController> logger, IDataSeeder dataSeeder, IStocksService stocksService, INewsService newsService, ICompanyOverviewService companyOverviewService)
        {
            _logger = logger;
            this.dataSeeder=dataSeeder;
            this.stocksService=stocksService;
            this.newsService=newsService;
            this.companyOverviewService=companyOverviewService;
        }

        public async Task<IActionResult> Index()
        {

            // await dataSeeder.SeedNews();
            // await dataSeeder.SeedStocks();
            // await dataSeeder.SeedOverviewCompanies();

            if (this.User.Identity.IsAuthenticated)
            {
                var portfolioModel = new PortfolioViewModel()
                {
                    Stocks = this.stocksService.GetAllStocks(),
                    News = this.newsService.GetAllNews(),
                    CompanyOverviews = this.companyOverviewService.GetAllCompanyOverviews(),
                };

                return View("HomePage", portfolioModel);
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}