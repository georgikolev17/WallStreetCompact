using WallStreetCompact.Models;

namespace WallStreetCompact.Services
{
    public interface ICompanyOverviewService
    {
        Task CreateCompanyOverviewAsync(CompanyOverview companyOverview);
    }
}
