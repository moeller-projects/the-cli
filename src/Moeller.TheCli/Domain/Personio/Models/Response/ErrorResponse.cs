using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public class ErrorResponse : BaseResponse
    {
        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }
    }

    public class Error
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "error_data")]
        public Dictionary<string, object> ErrorData { get; set; }
        
        [JsonProperty(PropertyName = "detailed_message")]
        public JArray? DetailedMessage { get; set; }
    }
}