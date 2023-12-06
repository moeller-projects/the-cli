using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public class BaseResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
    }
}
