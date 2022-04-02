using System;
using Newtonsoft.Json;

namespace WallStreetCompact.Models
{
    public class News
    {
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Ticker { get; set; }

        public string Title { get; set; }

        public string ArticleUrl { get; set; }

        public DateTime Date { get; set; }

        public string Time { get; set; }

        public decimal Sentiment { get; set; }
    }
}
