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
        public string MarketCapitalization { get; set; }
        public string EBITDA { get; set; }
        public string DividendPerShare { get; set; }
        public string DividendYield { get; set; }
        public string RevenuePerShareTTM { get; set; }
        public string ProfitMargin { get; set; }
        public string AnalystTargetPrice { get; set; }
        public string EVToRevenue { get; set; }
        public string EVToEBITDA { get; set; }
        public string FiftyTwoWeekHigh { get; set; }
        public string FiftyTwoWeekLow { get; set; }
        public string FiftyDayMovingAverage { get; set; }
        public string TwoHundredDayMovingAverage { get; set; }
        public string SharesOutstanding { get; set; }
    public CompanyOverview(string ticker, string assetType, string name, string description, string cik,
            string exchange, string currency, string country, string sector, string industry, string address,
            string marketCapitalization, string ebitda, string dividendPerShare, string dividendYield,
            string revenuePerShareTtm, string profitMargin, string analystTargetPrice,
            string evToRevenue, string evToEbitda, string fiftyTwoWeekHigh, string fiftyTwoWeekLow,
            string fiftyDayMovingAverage, string twoHundredDayMovingAverage, string sharesOutstanding)
        {
            Ticker = ticker ?? throw new ArgumentNullException(nameof(ticker));
            AssetType = assetType ?? throw new ArgumentNullException(nameof(assetType));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            CIK = cik ?? throw new ArgumentNullException(nameof(cik));
            Exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            Country = country ?? throw new ArgumentNullException(nameof(country));
            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
            Industry = industry ?? throw new ArgumentNullException(nameof(industry));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            MarketCapitalization =
                marketCapitalization ?? throw new ArgumentNullException(nameof(marketCapitalization));
            EBITDA = ebitda ?? throw new ArgumentNullException(nameof(ebitda));
            DividendPerShare = dividendPerShare ?? throw new ArgumentNullException(nameof(dividendPerShare));
            DividendYield = dividendYield ?? throw new ArgumentNullException(nameof(dividendYield));
            RevenuePerShareTTM =
                revenuePerShareTtm ?? throw new ArgumentNullException(nameof(revenuePerShareTtm));
            ProfitMargin = profitMargin ?? throw new ArgumentNullException(nameof(profitMargin));
            AnalystTargetPrice =
                analystTargetPrice ?? throw new ArgumentNullException(nameof(analystTargetPrice));
            EVToRevenue = evToRevenue ?? throw new ArgumentNullException(nameof(evToRevenue));
            EVToEBITDA = evToEbitda ?? throw new ArgumentNullException(nameof(evToEbitda));
            FiftyTwoWeekHigh = fiftyTwoWeekHigh ?? throw new ArgumentNullException(nameof(fiftyTwoWeekHigh));
            FiftyTwoWeekLow = fiftyTwoWeekLow ?? throw new ArgumentNullException(nameof(fiftyTwoWeekLow));
            FiftyDayMovingAverage = fiftyDayMovingAverage ??
                                    throw new ArgumentNullException(nameof(fiftyDayMovingAverage));
            TwoHundredDayMovingAverage = twoHundredDayMovingAverage ??
                                         throw new ArgumentNullException(nameof(twoHundredDayMovingAverage));
            SharesOutstanding = sharesOutstanding ?? throw new ArgumentNullException(nameof(sharesOutstanding));
        }

        public CompanyOverview(string ticker)
        {
            Ticker = ticker;
        }
    }
}
