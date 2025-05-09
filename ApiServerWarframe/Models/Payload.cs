using Newtonsoft.Json;

namespace ApiServerWarframe.Models
{
    public class ApiResponse<T>
    {
        [JsonProperty("payload")]
        public T? Payload { get; set; }
    }
}
