using System.Text.Json;
using WallStreetCompact.Models;

namespace WallStreetCompact.Services
{
    public class DataSeeder : IDataSeeder
    {
        private readonly INewsService newsService;

        public DataSeeder(INewsService newsService)
        {
            this.newsService=newsService;
        }

        public Dictionary<string, string> ReadFiles()
        {
            Dictionary<string, string> endResult = new();
            string path = Path.Combine(Environment.CurrentDirectory, @"Data", @"PolygonNewsData");
            string[] files = Directory.GetFiles(path);
            foreach (string filename in files)
            {
                //Console.WriteLine(filename.Split("_")[1].Split(".")[0]);
                endResult.Add(filename.Split("_")[1].Split(".")[0], File.ReadAllText(filename));
            }
            return endResult;
        }

        public async Task SeedNews()
        {
            var tempFiles = ReadFiles();
            var files = ParseNewsRequest(tempFiles, null);

            foreach (var file in files)
            {
                await newsService.CreateNewsAsync(file);
            }
        }

        public static List<News> ParseNewsRequest(Dictionary<string, string> responseBody, ILogger logger)
        {
            List<News> endResult = new();
            List<List<ParseOnlyNewsData>> endTempResult = new();
            try
            {
                foreach (KeyValuePair<string, string> elem in responseBody)
                {
                    // Creates new stock instance and specifies the ticker
                    dynamic jsonData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(elem.Value);
                    List<Dictionary<string, JsonElement>> parsedData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(jsonData["results"]);
                    endTempResult.Add(parsedData.Select(stock => new ParseOnlyNewsData(
                        tickerlist: Array.ConvertAll(stock["tickers"].EnumerateArray().ToArray(), title => title.GetString() ?? throw new NullReferenceException()),
                        title: stock["title"].GetString() ?? throw new InvalidOperationException(),
                        articleUrl: stock["article_url"].GetString() ?? throw new InvalidOperationException(),
                        //This should never be null so it's never a problem
                        date: stock["published_utc"].GetString().Split("T")[0] ?? throw new InvalidOperationException(),
                        time: stock["published_utc"].GetString().Split("T")[1] ?? throw new InvalidOperationException()
                    )).ToList());
                }

                var newTempList = endTempResult.SelectMany(x => x).ToList();
                Console.WriteLine(newTempList.Count);
                foreach (var tempresult in newTempList)
                {
                    foreach (var ticker in tempresult.TickerList)
                    {
                        if (StockList.SList.Contains(ticker))
                        {
                            endResult.Add(new News
                            {
                                Ticker = ticker,
                                Title = tempresult.Title,
                                ArticleUrl = tempresult.Article_url,
                                Date = DateTime.Parse(tempresult.Date),
                                Time = tempresult.Time,
                            });
                        }
                    }
                }

            }
            catch (Exception e)
            {
                logger.LogInformation("Exception thrown : {Exception}", e.ToString());
            }

            return endResult;
        }

        public class ParseOnlyNewsData
        {
            /// <summary>
            /// Since the API returns a list of Tickers for each article this is a temporary class used ONLY during data
            /// processing and not anywhere else.
            /// The only difference is that instead of a Ticker we have a TickerList which is an array
            /// </summary>
            public string[] TickerList { get; set; }
            public string Title { get; set; }
            public string Article_url { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }

            public ParseOnlyNewsData(string[] tickerlist, string title, string articleUrl, string date, string time)
            {
                TickerList = tickerlist ?? throw new ArgumentNullException(nameof(tickerlist));
                Title = title ?? throw new ArgumentNullException(nameof(title));
                Article_url = articleUrl ?? throw new ArgumentNullException(nameof(articleUrl));
                Date = date;
                Time = time;
            }
        }
    }

    public static class StockList
    {
        public static List<string> SList = new()
        {
            "GOOGL", "MMM", "AOS", "ABT", "ABBV", "ABMD", "ACN", "ATVI", "ADBE", "AAP", "AES", "AFL", "A",
            "AIG", "APD", "AKAM", "ALK", "ALB", "ARE", "ALGN", "ALLE", "LNT", "ALL",
            "GOOG", "MO", "AMZN", "AMCR", "AMD", "AEE", "AAL", "AEP", "AXP", "AMT" ,"AWK", "AMP",
            "ABC", "AME", "AMGN", "APH", "ADI", "ANSS", "ANTM", "AON", "APA" ,"AAPL", "AMAT",
            "APTV", "ADM", "ANET", "AJG" ,"AIZ", "T" ,"ATO", "ADSK", "ADP", "AZO", "AVB", "AVY",
            "BKR", "BLL", "BAC", "BBWI", "BAX" ,"BDX", "BRK.B", "BBY", "BIO", "TECH", "BIIB",
            "BLK", "BK", "BA", "BKNG" ,"BWA", "BXP", "BSX", "BMY", "AVGO", "BR", "BRO", "BF.B",
            "CHRW", "CDNS", "CZR" ,"CPB", "COF", "CAH", "KMX" ,"CCL" ,"CARR", "CTLT" ,"CAT",
            "CBOE", "CBRE", "CDW", "CE", "CNC", "CNP", "CDAY", "CERN" ,"CF" ,"CRL", "SCHW",
            "CHTR", "CVX", "CMG", "CB", "CHD", "CI", "CINF", "CTAS", "CSCO", "C", "CFG", "CTXS",
            "CLX" ,"CME", "CMS", "KO", "CTSH", "CL" ,"CMCSA" ,"CMA" ,"CAG" ,"COP", "ED" ,"STZ",
            "COO", "CPRT", "GLW", "CTVA" ,"COST", "CTRA", "CCI", "CSX", "CMI", "CVS", "DHI",
            "DHR" ,"DRI", "DVA", "DE", "DAL", "XRAY", "DVN", "DXCM", "FANG", "DLR", "DFS",
            "DISCA" ,"DISCK" ,"DISH" ,"DG" ,"DLTR" ,"D", "DPZ", "DOV", "DOW", "DTE", "DUK",
            "DRE", "DD", "DXC" ,"EMN", "ETN", "EBAY", "ECL" ,"EIX" ,"EW" ,"EA", "EMR", "ENPH",
            "ETR" ,"EOG", "EPAM", "EFX", "EQIX" ,"EQR", "ESS", "EL" ,"ETSY", "EVRG", "ES", "RE",
            "EXC", "EXPE", "EXPD", "EXR", "XOM", "FFIV" ,"FDS" ,"FAST" ,"FRT", "FDX", "FIS",
            "FITB", "FE", "FRC", "FISV", "FLT" ,"FMC" ,"F", "FTNT", "FTV", "FBHS", "FOXA", "FOX",
            "BEN", "FCX", "GPS" ,"GRMN" ,"IT" ,"GNRC", "GD" ,"GE" ,"GIS", "GM", "GPC", "GILD",
            "GL", "GPN", "GS", "GWW", "HAL", "HIG", "HAS", "HCA", "PEAK", "HSIC", "HSY", "HES",
            "HPE", "HLT" ,"HOLX", "HD" ,"HON" ,"HRL", "HST", "HWM", "HPQ", "HUM", "HBAN", "HII",
            "IEX" ,"IDXX", "INFO", "ITW", "ILMN", "INCY" ,"IR" ,"INTC", "ICE", "IBM", "IP",
            "IPG" ,"IFF", "INTU", "ISRG" ,"IVZ" ,"IPGP", "IQV" ,"IRM" ,"JKHY" ,"J" ,"JBHT",
            "SJM", "JNJ" ,"JCI" ,"JPM", "JNPR" ,"K" ,"KEY", "KEYS", "KMB", "KIM", "KMI", "KLAC",
            "KHC", "KR", "LHX", "LH", "LRCX", "LW" ,"LVS", "LDOS", "LEN" ,"LLY", "LNC", "LIN",
            "LYV", "LKQ", "LMT", "L" ,"LOW" ,"LUMN", "LYB", "MTB", "MRO" ,"MPC", "MKTX", "MAR", 
            "MMC" ,"MLM", "MAS", "MA", "MTCH", "MKC", "MCD", "MCK", "MDT", "MRK", "FB", "MET",
            "MTD", "MGM", "MCHP", "MU", "MSFT", "MAA", "MRNA", "MHK", "TAP", "MDLZ", "MPWR",
            "MNST", "MCO", "MS", "MOS", "MSI", "MSCI", "NDAQ", "NTAP", "NFLX", "NWL", "NEM",
            "NWSA" ,"NWS" ,"NEE", "NLSN", "NKE" ,"NI", "NSC", "NTRS", "NOC" ,"NLOK" ,"NCLH",
            "NRG", "NUE", "NVDA" ,"NVR" ,"NXPI" ,"ORLY", "OXY" ,"ODFL", "OMC" ,"OKE" ,"ORCL",
            "OGN", "OTIS" ,"PCAR", "PKG", "PH", "PAYX", "PAYC", "PYPL", "PENN" ,"PNR", "PBC",
            "PEP", "PKI" ,"PFE", "PM", "PSX", "PNW", "PXD", "PNC", "POOL," ,"PPG", "PPL", "PFG",
            "PG", "PGR", "PLD", "PRU", "PEG", "PTC", "PSA", "PHM", "PVH", "QRVO", "PWR", "QCOM",
            "DGX" ,"RL" ,"RJF" ,"RTX" ,"O" ,"REG" ,"REGN" ,"RF" ,"RSG" ,"RMD" ,"RHI", "ROK",
            "ROL" ,"ROP" ,"ROST", "RCL" ,"SPGI" ,"CRM" ,"SBAC" ,"SLB", "STX" ,"SEE", "SRE",
            "NOW" ,"SHW" ,"SBNY" ,"SPG" ,"SWKS", "SNA", "SEDG", "SO", "LUV", "SWK", "SBUX",
            "STT", "STE" ,"SYK" ,"SIVB", "SYF" ,"SNPS", "SYY", "TMUS", "TROW", "TTWO", "TPR",
            "TGT" ,"TEL", "TDY" ,"TFX", "TER" ,"TSLA", "TXN", "TXT", "TMO" ,"TJX", "TSCO" ,"TT",
            "TDG", "TRV" ,"TRMB", "TFC", "TWTR" ,"TYL" ,"TSN", "UDR", "ULTA", "USB" ,"UAA", "UA",
            "UNP" ,"UAL", "UNH" ,"UPS" ,"URI", "UHS" ,"VLO" ,"VTR" ,"VRSN" ,"VRSK" ,"VZ", "VRTX",
            "VFC", "VIAC", "VTRS" ,"V" ,"VNO" ,"VMC" ,"WRB" ,"WAB", "WMT", "WBA" ,"DIS" ,"WM",
            "WAT" ,"WEC" ,"WFC", "WELL" ,"WST", "WDC", "WRK" ,"WY" ,"WHR" ,"WMB" ,"WTW", "WYNN",
            "XEL" ,"XLNX" ,"XYL"
        };
    }
}
