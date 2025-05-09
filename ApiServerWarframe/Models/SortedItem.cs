using Newtonsoft.Json;

namespace ApiServerWarframe.Models
{
    public class SortedItem
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("url_name")]
        public string UrlName { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("liquidity", NullValueHandling = NullValueHandling.Ignore)]
        public int? Liquidity { get; set; }

        [JsonProperty("spread", NullValueHandling = NullValueHandling.Ignore)]
        public int? Spread { get; set; }

        [JsonProperty("rank", NullValueHandling = NullValueHandling.Ignore)]
        public double? Rank { get; set; }

        [JsonProperty("subtype", NullValueHandling = NullValueHandling.Ignore)]
        public string? Subtype { get; set; }

        [JsonProperty("buy_price", NullValueHandling = NullValueHandling.Ignore)]
        public int? BuyPrice { get; set; }

        [JsonProperty("sell_price", NullValueHandling = NullValueHandling.Ignore)]
        public int? SellPrice { get; set; }

        [JsonProperty("reason", NullValueHandling = NullValueHandling.Ignore)]
        public string? Reason { get; set; }

        [JsonProperty("trading_tax", NullValueHandling = NullValueHandling.Ignore)]
        public int TradingTax { get; set; }

        //[JsonIgnore]
        //public string CTradingTax
        //{
        //    get
        //    {
        //        if (TradingTax >= 1_000_000)
        //            return $"{TradingTax / 1_000_000.0:G}M"; // Выводит все значимые цифры
        //        else if (TradingTax >= 1_000)
        //            return $"{TradingTax / 1_000.0:G}K";     // Выводит все значимые цифры
        //        else
        //            return TradingTax.ToString();
        //    }
        //}

        [JsonProperty("90days_trend",NullValueHandling = NullValueHandling.Ignore)]
        public double DaysTrend { get; set; }

        [JsonProperty("48hours_trend", NullValueHandling = NullValueHandling.Ignore)]
        public double HoursTrend { get; set; }
    }
}
