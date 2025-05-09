using Newtonsoft.Json;

namespace ApiServerWarframe.Models
{
    public class ItemDetailPayload
    {
        [JsonProperty("item")]
        public ItemDetail Item { get; set; } = new ItemDetail();
    }

    public class ItemDetail
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("items_in_set")]
        public List<ItemSet> ItemsInSet { get; set; } = new List<ItemSet>();
    }

    public class ItemSet
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("url_name")]
        public string UrlName { get; set; } = string.Empty;

        [JsonProperty("icon")]
        public string UrlIcon { get; set; } = string.Empty;

        [JsonProperty("sub_icon", NullValueHandling = NullValueHandling.Ignore)]
        public string? UrlSubIcon { get; set; }

        [JsonProperty("thumb")]
        public string UrlThumb { get; set; } = string.Empty;

        [JsonProperty("mod_max_rank", NullValueHandling = NullValueHandling.Ignore)]
        public int? ModMaxRank { get; set; }

        [JsonProperty("subtypes", NullValueHandling = NullValueHandling.Ignore)]
        public string[]? Subtypes { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; } = [];

        [JsonProperty("ducats")]
        public int Ducats { get; set; }

        [JsonProperty("rarity")]
        public string Rarity { get; set; } = string.Empty;

        [JsonProperty("trading_tax")]
        public int TradingTax { get; set; }
    }
}
