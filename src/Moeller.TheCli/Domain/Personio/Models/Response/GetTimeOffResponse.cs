using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public class GetTimeOffResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "date")]
        public object Data { get; set; }
#warning TODO
    }
}
