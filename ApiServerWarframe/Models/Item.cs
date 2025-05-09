using Newtonsoft.Json;

namespace ApiServerWarframe.Models
{
    public class ItemsPayload
    {
        [JsonProperty("items")]
        public List<Item> Items { get; set; } = new List<Item>();
    }

    public class Item
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("url_name")]
        public string UrlName { get; set; } = string.Empty;

        [JsonProperty("thumb")]
        public string Thumb { get; set; } = string.Empty;

        [JsonProperty("item_name")]
        public string ItemName { get; set; } = string.Empty;
    }
}
