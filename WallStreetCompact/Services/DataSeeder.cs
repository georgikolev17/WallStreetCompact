using System.Text.Json;
using WallStreetCompact.Models;
using NumSharp;

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
            var files = Init();

            foreach (var file in files)
            {
                await newsService.CreateNewsAsync(file);
            }
        }
        
        public async Task SeedStocks()
        {
            
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

        public List<News> Init()
        {
            var allData = ReadFiles();
            Console.WriteLine(allData.Keys.Count);
            var ParsedData = ParseNewsRequest(allData, null);
            var y = ParsedData.Select(data => data.Title).ToList();
            string path = Path.Combine(Environment.CurrentDirectory, @"Services/SentimentNPYs", @"CurrentNewsSentiment.npy");
            //Console.WriteLine(path);
            //Console.WriteLine("/home/martin/RiderProjects/Lexonic/LexonicWebApplication/Data/SentimentNPYs/CurrentNewsSentiment.npy");
            var xNdArray = np.load(path);

            for (int i = 0; i < (xNdArray.Shape[0] & ParsedData.Count); i++)
            {
                ParsedData[i].Sentiment = (decimal) xNdArray.ToArray<double>()[i];
            }
            return ParsedData;
        }
    }

    public static class StockList
    {
        public static List<string> SList = new()
        {
            "GOOGL", "TSLA", "APPL"
        };
    }
}
