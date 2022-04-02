/*using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
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
            var files = ParseNewsRequest(tempFiles);

            foreach (var file in files)
            {
                await newsService.CreateNewsAsync(file);
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
                //logger.LogInformation("Exception thrown : {Exception}", e.ToString());
                Console.WriteLine("Nigeriec");
            }

            return endResult;
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
        public static Dictionary<DateOnly, string> ReadStockFiles()
        {
            Dictionary<DateOnly, string> endResult = new();
            string path = Path.Combine(Environment.CurrentDirectory, @"Data",@"PolygonStockData");
            string[] files = Directory.GetFiles(path);
            foreach (string filename in files)
            {
                DateTime dt = DateTime.Parse(filename.Split("_")[1].Split(".")[0]);
                //Console.WriteLine(filename.Split("_")[1].Split(".")[0]);
                endResult.Add(DateOnly.FromDateTime(dt), File.ReadAllText(filename));
            }
            return endResult;
        }
        public static Dictionary<DateOnly, List<StockData>> ProcessStockData(Dictionary<DateOnly, string> responseBody)
        {
            Dictionary<DateOnly, List<StockData>> globalHistoricData = new();
            try
            {
                if (responseBody.Count == 0)
                {
                    // TODO: Find another solution instead of throwing exceptions
                    throw new ArgumentNullException(nameof(responseBody), $"{nameof(responseBody)} is empty");
                }
                Dictionary<DateOnly, List<StockData>> historicData = new();
                foreach (KeyValuePair<DateOnly, string> elem in responseBody)
                {
                    dynamic jsonData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(elem.Value);
                    List<Dictionary<string, JsonElement>> parsedData =
                        JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(jsonData["results"]);
                    List<StockData> timestepData = parsedData
                        .Where(stock => StockList.SList.Contains(stock["T"].ToString())).Select(stock => new StockData()
                        {
                            High = stock["h"].GetDecimal(),
                            Low = stock["l"].GetDecimal(),
                            Open = stock["o"].GetDecimal(),
                            Close = stock["c"].GetDecimal(),
                            Ticker = stock["T"].GetString(),
                            Volume = Int64.Parse(stock["v"].GetDecimal().ToString()),
                            NumberOfTransactions = stock["n"].GetInt64(),
                            Date = FormatDate(elem.Key)
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
                    dateString = _dateOnly.ToString().Split("/")[2] + "-0" + _dateOnly.ToString().Split("/")[0] + "-0" + _dateOnly.ToString().Split("/")[1];
                }
                else
                {
                    dateString = _dateOnly.ToString().Split("/")[2] + "-0" + _dateOnly.ToString().Split("/")[0] + "-" + _dateOnly.ToString().Split("/")[1];
                }
            }
            else
            {
                if (_dateOnly.ToString().Split("/")[1].Length == 1)
                {
                    dateString = _dateOnly.ToString().Split("/")[2] + "-" + _dateOnly.ToString().Split("/")[0] + "-0" + _dateOnly.ToString().Split("/")[1];

                }
                else
                {
                    dateString = _dateOnly.ToString().Split("/")[2] + "-" + _dateOnly.ToString().Split("/")[0] + "-" + _dateOnly.ToString().Split("/")[1];
                }
            }
            return dateString;
        }
    }

    public static class StockList
    {
        public static List<string> SList = new()
        {
            "GOOGL", "APPL", "TSLA"
        };
    }
}
*/
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

namespace WallStreetCompact.Services
{
    public class DataSeeder : IDataSeeder
    {
        private readonly INewsService newsService;

        public DataSeeder(INewsService newsService)
        {
            this.newsService = newsService;
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

        public static Dictionary<DateOnly, string> ReadStockFiles()
        {
            Dictionary<DateOnly, string> endResult = new();
            string path = Path.Combine(Environment.CurrentDirectory, @"Data", @"PolygonStockData");
            string[] files = Directory.GetFiles(path);
            foreach (string filename in files)
            {
                DateTime dt = DateTime.Parse(filename.Split("_")[1].Split(".")[0]);
                //Console.WriteLine(filename.Split("_")[1].Split(".")[0]);
                endResult.Add(DateOnly.FromDateTime(dt), File.ReadAllText(filename));
            }

            return endResult;
        }

        public static Dictionary<DateOnly, List<StockData>> ProcessStockData(Dictionary<DateOnly, string> responseBody)
        {
            Dictionary<DateOnly, List<StockData>> globalHistoricData = new();
            try
            {
                if (responseBody.Count == 0)
                {
                    // TODO: Find another solution instead of throwing exceptions
                    throw new ArgumentNullException(nameof(responseBody), $"{nameof(responseBody)} is empty");
                }

                Dictionary<DateOnly, List<StockData>> historicData = new();
                foreach (KeyValuePair<DateOnly, string> elem in responseBody)
                {
                    dynamic jsonData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(elem.Value);
                    List<Dictionary<string, JsonElement>> parsedData =
                        JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(jsonData["results"]);
                    List<StockData> timestepData = parsedData
                        .Where(stock => StockList.SList.Contains(stock["T"].ToString())).Select(stock => new StockData()
                        {
                            High = stock["h"].GetDecimal(),
                            Low = stock["l"].GetDecimal(),
                            Open = stock["o"].GetDecimal(),
                            Close = stock["c"].GetDecimal(),
                            Ticker = stock["T"].GetString(),
                            Volume = Int64.Parse(stock["v"].GetDecimal().ToString()),
                            NumberOfTransactions = stock["n"].GetInt64(),
                            Date = FormatDate(elem.Key)
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
    }
}

public static class StockList
    {
        public static List<string> SList = new()
        {
            "GOOGL", "TSLA", "APPL"
        };
    }
