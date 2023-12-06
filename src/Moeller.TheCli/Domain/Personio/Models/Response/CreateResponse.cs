using System.Net;
using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public class CreateResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "statusCode")]
        public HttpStatusCode StatusCode { get; set; }

        [JsonProperty(PropertyName = "error")]
        public ErrorObject Error { get; set; }

        public static CreateResponse<TItem> FromData<TItem>(TItem item, HttpStatusCode httpStatusCode)
            => new CreateResponse<TItem> { Data = item, Success = true, StatusCode = httpStatusCode };

        public static CreateResponse<TItem> FromError<TItem>(CreateResponse createResponse)
            => new CreateResponse<TItem>() { Error = createResponse.Error, StatusCode = createResponse.StatusCode };
    }

    public class CreateResponse<T> : CreateResponse
    {
        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }
}
