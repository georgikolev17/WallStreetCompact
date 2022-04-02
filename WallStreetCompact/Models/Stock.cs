using System;
using System.ComponentModel.DataAnnotations;

namespace WallStreetCompact.Models
{
    public class Stock
    {
        [Required]
        public int Id { get; set; }

        public string Ticker { get; set; }

        public decimal Open { get; set; }
        
        public decimal Close { get; set; }
        
        public decimal High { get; set; }
        
        public decimal Low { get; set; }
        
        public Int64 Volume { get; set; }
        
        public Int64 NumberOfTransactions { get; set; }
        
        public string Date { get; set; }

        [Required]
        public int PortfolioId { get; set; }

        public virtual Portfolio Portfolio { get; set; }
    }
}
