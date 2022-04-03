using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WallStreetCompact.Models;
using WallStreetCompact.Services;

namespace WallStreetCompact.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDataSeeder dataSeeder;

        public HomeController(ILogger<HomeController> logger, IDataSeeder dataSeeder)
        {
            _logger = logger;
            this.dataSeeder=dataSeeder;
        }

        public async Task<IActionResult> Index()
        {

            // await dataSeeder.SeedNews();
            // await dataSeeder.SeedStocks();
            await dataSeeder.SeedOverviewCompanies();

            return View();
        }

        public IActionResult Privacy()
        {
            return View("HomePage");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}