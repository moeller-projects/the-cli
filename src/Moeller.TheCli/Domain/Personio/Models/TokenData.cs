using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models
{
    public class TokenData
    {
        [JsonProperty(PropertyName = "token")]
        public required string Token { get; set; }
    }
}
