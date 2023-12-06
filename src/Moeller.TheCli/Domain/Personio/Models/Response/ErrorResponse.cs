using Newtonsoft.Json;

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
    }
}