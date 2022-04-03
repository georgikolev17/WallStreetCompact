namespace WallStreetCompact.Models
{
    public class Prediction
    {
        public int Id { get; set; }

        public double Price { get; set; }

        public int CompanyOverviewId { get; set; }

        public virtual CompanyOverview CompanyOverview { get; set; }
    }
}
