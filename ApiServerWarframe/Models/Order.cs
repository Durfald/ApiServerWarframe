using Newtonsoft.Json;

namespace ApiServerWarframe.Models
{
    public class OrdersPayload
    {
        [JsonProperty("orders")]
        public List<Order> Orders { get; set; } = new List<Order>();
    }

    public class Order
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("platinum")]
        public int Platinum { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("order_type")]
        public string OrderType { get; set; } = string.Empty;

        [JsonIgnore]
        public OrderType OrderTypeEnum => (OrderType)Enum.Parse(typeof(OrderType), OrderType, true);

        [JsonProperty("mod_rank", NullValueHandling = NullValueHandling.Ignore)]
        public int? ModRank { get; set; }

        [JsonProperty("subtype", NullValueHandling = NullValueHandling.Ignore)]
        public string? Subtype { get; set; }

        [JsonProperty("creation_date")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("last_update")]
        public DateTime LastUpdate { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; } = string.Empty;

        [JsonProperty("user")]
        public OrderUser User { get; set; } = new OrderUser();
    }

    public enum OrderType
    {
        Buy,
        Sell
    }

    public enum UserStatus
    {
        Offline,
        Online,
        InGame
    }

    public class OrderUser
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("platform")]
        public string Platform { get; set; } = string.Empty;

        [JsonProperty("ingame_name")]
        public string IngameName { get; set; } = string.Empty;

        [JsonProperty("crossplay")]
        public bool Crossplay { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonIgnore]
        public UserStatus StatusEnum => (UserStatus)Enum.Parse(typeof(UserStatus), Status, true);

        [JsonProperty("region")]
        public string Region { get; set; } = string.Empty;

        [JsonProperty("reputation")]
        public int Reputation { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; } = string.Empty;

        [JsonProperty("last_seen")]
        public DateTime LastSeen { get; set; }
    }
}
