using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WallStreetCompact.Models
{
    public class Portfolio
    {
        public int UserId { get; set; }
        public virtual IdentityUser User { get; set; }

        public int Id { get; set; }
    }
}
