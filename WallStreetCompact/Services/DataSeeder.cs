using System.Text.Json;
using WallStreetCompact.Models;
using NumSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using WallStreetCompact.Models;
using System.Globalization;

namespace WallStreetCompact.Services
{
    public class DataSeeder : IDataSeeder
    {
        private readonly INewsService newsService;
        private readonly IStocksService stocksService;
        private readonly ICompanyOverviewService companyOverviewService;

        public DataSeeder(INewsService newsService, IStocksService stocksService, ICompanyOverviewService companyOverviewService)
        {
            this.newsService = newsService;
            this.stocksService=stocksService;
            this.companyOverviewService=companyOverviewService;
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
            var files = Init();

            foreach (var file in files)
            {
                await newsService.CreateNewsAsync(file);
            }
        }

        public async Task SeedStocks()
        {
            var stockFiles = ReadStockFiles();
            var tempStocks = ProcessStockData(stockFiles);
            foreach (var stocks in tempStocks.Values)
            {
                foreach (var stock in stocks)
                {
                    await this.stocksService.CreateStockAsync(stock);
                }
            }
        }

        public async Task SeedOverviewCompanies()
        {
            var companyOvewviewFiles = readCompanyOverviewFiles();
            var companyOverview = ProcessCompanyOverviewData(companyOvewviewFiles);

            foreach (var c in companyOverview)
            {
                await this.companyOverviewService.CreateCompanyOverviewAsync(c);
            }
        }

        public static List<News> ParseNewsRequest(Dictionary<string, string> responseBody)
        {
            List<News> endResult = new();
            List<List<ParseOnlyNewsData>> endTempResult = new();
            try
            {
                foreach (KeyValuePair<string, string> elem in responseBody)
                {
                    // Creates new stock instance and specifies the ticker
                    dynamic jsonData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(elem.Value);
                    List<Dictionary<string, JsonElement>> parsedData =
                        JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(jsonData["results"]);
                    endTempResult.Add(parsedData.Select(stock => new ParseOnlyNewsData(
                        tickerlist: Array.ConvertAll(stock["tickers"].EnumerateArray().ToArray(),
                            title => title.GetString() ?? throw new NullReferenceException()),
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
                //logger.LogInformation("Exception thrown : {Exception}", e.ToString());
                Console.WriteLine("Nigeriec");
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

        public List<News> Init()
        {
            var allData = ReadFiles();
            Console.WriteLine(allData.Keys.Count);
            var ParsedData = ParseNewsRequest(allData);
            var y = ParsedData.Select(data => data.Title).ToList();
            string path = Path.Combine(Environment.CurrentDirectory, @"Services/SentimentNPYs",
                @"CurrentNewsSentiment.npy");
            //Console.WriteLine(path);
            //Console.WriteLine("/home/martin/RiderProjects/Lexonic/LexonicWebApplication/Data/SentimentNPYs/CurrentNewsSentiment.npy");
            var xNdArray = np.load(path);

            for (int i = 0; i < (xNdArray.Shape[0] & ParsedData.Count); i++)
            {
                ParsedData[i].Sentiment = (decimal) xNdArray.ToArray<double>()[i];
            }

            return ParsedData;
        }

        public static Dictionary<DateOnly, string> ReadStockFiles()
        {
            Dictionary<DateOnly, string> endResult = new();
            string path = Path.Combine(Environment.CurrentDirectory, @"Data", @"PolygonStockData");
            string[] files = Directory.GetFiles(path);
            foreach (string filename in files)
            {
                var date = filename.Split("_")[1].Split(".")[0].Split('-').Select(x => int.Parse(x)).ToArray();
                DateTime dt = DateTime.ParseExact($"{date[0]:D2}/{date[1]:D2}/{date[2]}", "MM/dd/yyyy", CultureInfo.InvariantCulture);
                //Console.WriteLine(filename.Split("_")[1].Split(".")[0]);
                endResult.Add(DateOnly.FromDateTime(dt), File.ReadAllText(filename));
            }

            return endResult;
        }

        public static Dictionary<DateOnly, List<Stock>> ProcessStockData(Dictionary<DateOnly, string> responseBody)
        {
            Dictionary<DateOnly, List<Stock>> globalHistoricData = new();
            try
            {
                if (responseBody.Count == 0)
                {
                    throw new ArgumentNullException(nameof(responseBody), $"{nameof(responseBody)} is empty");
                }

                Dictionary<DateOnly, List<Stock>> historicData = new();
                foreach (KeyValuePair<DateOnly, string> elem in responseBody)
                {
                    dynamic jsonData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(elem.Value);
                    List<Dictionary<string, JsonElement>> parsedData =
                        JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(jsonData["results"]);
                    List<Stock> timestepData = parsedData
                        .Where(stock => StockList.SList.Contains(stock["T"].ToString())).Select(stock => new Stock()
                        {
                            High = stock["h"].GetDecimal(),
                            Low = stock["l"].GetDecimal(),
                            Open = stock["o"].GetDecimal(),
                            Close = stock["c"].GetDecimal(),
                            Ticker = stock["T"].GetString(),
                            Volume = Int64.Parse(stock["v"].GetDecimal().ToString()),
                            NumberOfTransactions = stock["n"].GetInt64(),
                            Date = elem.Key.ToString()
                        })
                        .ToList();

                    historicData.Add(elem.Key, timestepData);
                }

                globalHistoricData = historicData;
            }
            catch (Exception e)
            {
                //logger.LogInformation("Exception thrown : {Exception}", e.ToString());
                Console.WriteLine("Exception thrown : {0}", e.ToString());
            }

            return globalHistoricData;
        }

        // Utility function for processing the Date to a format that the API will take
        public static string FormatDate(DateOnly _dateOnly)
        {
            string dateString = String.Empty;
            if (_dateOnly.ToString().Split("/")[0].Length == 1)
            {
                if (_dateOnly.ToString().Split("/")[1].Length == 1)
                {
                    dateString = _dateOnly.ToString().Split("/")[2] + "-0" + _dateOnly.ToString().Split("/")[0] + "-0" +
                                 _dateOnly.ToString().Split("/")[1];
                }
                else
                {
                    dateString = _dateOnly.ToString().Split("/")[2] + "-0" + _dateOnly.ToString().Split("/")[0] + "-" +
                                 _dateOnly.ToString().Split("/")[1];
                }
            }
            else
            {
                if (_dateOnly.ToString().Split("/")[1].Length == 1)
                {
                    dateString = _dateOnly.ToString().Split("/")[2] + "-" + _dateOnly.ToString().Split("/")[0] + "-0" +
                                 _dateOnly.ToString().Split("/")[1];

                }
                else
                {
                    dateString = _dateOnly.ToString().Split("/")[2] + "-" + _dateOnly.ToString().Split("/")[0] + "-" +
                                 _dateOnly.ToString().Split("/")[1];
                }
            }

            return dateString;
        }

        public static List<string> readCompanyOverviewFiles()
        {
            List<string> endResult = new List<string>();

            string path = Path.Combine(Environment.CurrentDirectory, @"Data", @"CompanyOverview");
            string[] files = { "AAPL.json", "TSLA.json", "GOOGL.json" };
            foreach (string filename in files)
            {
                endResult.Add(File.ReadAllText(Path.Combine(path, filename)));
            }

            return endResult;
        }

        public static List<CompanyOverview> ProcessCompanyOverviewData(List<string> dList)
        {
            List<CompanyOverview> parsedData = new();
            foreach (string response in dList)
            {
                var Data = JsonSerializer.Deserialize<Dictionary<string, string>>(response) ??
                           throw new InvalidOperationException();
                string tempTicker = Data["Symbol"];

                string AssetType = Data["AssetType"];
                string tempName = Data["Name"];
                string tempDescription = Data["Description"];
                string tempCIK = Data["CIK"];
                string tempExchange = Data["Exchange"];
                string tempCurrency = Data["Currency"];
                string tempCountry = Data["Country"];
                string tempSector = Data["Sector"];
                string tempIndustry = Data["Industry"];
                string tempAddress = Data["Address"];
                string tempMarketCapitalization = Data["MarketCapitalization"];
                string tempEBITDA = Data["EBITDA"];
                string tempDividendPerShare = Data["DividendPerShare"];
                string tempDividendYield = Data["DividendYield"];
                string tempRevenuePerShareTTM = Data["RevenuePerShareTTM"];
                string tempProfitMargin = Data["ProfitMargin"];
                string tempAnalystTargetPrice = Data["AnalystTargetPrice"];
                string tempEVToRevenue = Data["EVToRevenue"];
                string tempEVToEBITDA = Data["EVToEBITDA"];
                string tempFiftyTwoWeekHigh = Data["52WeekHigh"];
                string tempFiftyTwoWeekLow = Data["52WeekLow"];
                string tempFiftyDayMovingAverage = Data["50DayMovingAverage"];
                string tempTwoHundredDayMovingAverage = Data["200DayMovingAverage"];
                string tempSharesOutstanding = Data["SharesOutstanding"];

                CompanyOverview currentStock = new(
                    tempTicker,
                    AssetType,
                    tempName,
                    tempDescription,
                    tempCIK,
                    tempExchange,
                    tempCurrency,
                    tempCountry,
                    tempSector,
                    tempIndustry,
                    tempAddress,
                    tempMarketCapitalization,
                    tempEBITDA,
                    tempDividendPerShare,
                    tempDividendYield,
                    tempRevenuePerShareTTM,
                    tempProfitMargin,
                    tempAnalystTargetPrice,
                    tempEVToRevenue,
                    tempEVToEBITDA,
                    tempFiftyTwoWeekHigh,
                    tempFiftyTwoWeekLow,
                    tempFiftyDayMovingAverage,
                    tempTwoHundredDayMovingAverage,
                    tempSharesOutstanding
                );
                parsedData.Add(currentStock);
            }

            return parsedData;
        }
    }
}

public static class StockList
    {
        public static List<string> SList = new()
        {
            "GOOGL", "TSLA", "AAPL"
        };
    }

public class StockData
{
    public string Ticker { get; set; }
    public decimal Open { get; set; }
    public decimal Close { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public Int64 Volume { get; set; }
    public Int64 NumberOfTransactions { get; set; }
    public string Date { get; set; }
}
