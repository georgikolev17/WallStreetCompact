using Microsoft.AspNetCore.Identity;

namespace WallStreetCompact.Models
{
    public class Portfolio
    {
        public Portfolio()
        {
            this.Stocks = new HashSet<Stock>();
        }

        public int UserId { get; set; }
        public virtual IdentityUser User { get; set; }

        public int Id { get; set; }

        public virtual ICollection<Stock> Stocks { get; set; }


    }
}
