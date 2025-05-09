using Newtonsoft.Json;

namespace ApiServerWarframe.Models
{
    public class StatisticsPayload
    {
        [JsonProperty("statistics_closed")]
        public StatisticsData ClosedStatistics { get; set; } = new StatisticsData();

        [JsonProperty("statistics_live")]
        public StatisticsData LiveStatistics { get; set; } = new StatisticsData();
    }

    public class StatisticsData
    {
        [JsonProperty("48hours")]
        public List<Statistic> Last48Hours { get; set; } = new List<Statistic>();

        [JsonProperty("90days")]
        public List<Statistic> Last90Days { get; set; } = new List<Statistic>();
    }

    public class Statistic
    {
        [JsonProperty("subtype", NullValueHandling = NullValueHandling.Ignore)]
        public string? Subtype { get; set; }

        [JsonProperty("mod_rank", NullValueHandling = NullValueHandling.Ignore)]
        public double? ModRank { get; set; }

        [JsonProperty("datetime")]
        public DateTime Datetime { get; set; }

        [JsonProperty("volume")]
        public int Volume { get; set; }

        [JsonProperty("min_price")]
        public double MinPrice { get; set; }

        [JsonProperty("max_price")]
        public double MaxPrice { get; set; }

        [JsonProperty("open_price")]
        public double OpenPrice { get; set; }

        [JsonProperty("closed_price")]
        public double ClosedPrice { get; set; }

        [JsonProperty("avg_price")]
        public double AvgPrice { get; set; }

        [JsonProperty("wa_price")]
        public double WeightedAvgPrice { get; set; }

        [JsonProperty("median")]
        public double Median { get; set; }

        [JsonProperty("moving_avg", NullValueHandling = NullValueHandling.Ignore)]
        public double? MovingAvg { get; set; }

        [JsonProperty("donch_top")]
        public double DonchianTop { get; set; }

        [JsonProperty("donch_bot")]
        public double DonchianBottom { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
    }
}
