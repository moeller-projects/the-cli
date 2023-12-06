using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models
{
    public class TokenData
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}
