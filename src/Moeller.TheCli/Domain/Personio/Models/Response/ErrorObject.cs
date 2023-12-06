using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public class ErrorObject
    {
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
