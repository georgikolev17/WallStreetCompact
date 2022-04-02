namespace WallStreetCompact.Models
{
    public class CompanyOverview
    {
        public int Id { get; set; }
        public string Ticker { get; set; }

        public string AssetType { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string CIK { get; set; }
        
        public string Exchange { get; set; }
        
        public string Currency { get; set; }
        
        public string Country { get; set; }
        
        public string Sector { get; set; }
        
        public string Industry { get; set; }
        
        public string Address { get; set; }
        
        public string FiscalYearEnd { get; set; }
        
        public string LatestQuarter { get; set; }
        
        public string MarketCapitalization { get; set; }
        
        public string EBITDA { get; set; }
        
        public string PERatio { get; set; }
        
        public string PEGRatio { get; set; }
        
        public string BookValue { get; set; }
        
        public string DividendPerShare { get; set; }

        public string DividendYield { get; set; }

        public string FiftyTwoWeekHigh { get; set; }
        
        public string FiftyTwoWeekLow { get; set; }
        
        public string FiftyDayMovingAverage { get; set; }

        public string TwoHundredDayMovingAverage { get; set; }

        public decimal AnalystTargetPrice { get; set; }

        public int StockId { get; set; }
        public virtual Stock Stock { get; set; }
    }
}
