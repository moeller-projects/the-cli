using System.Net;
using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public class DeleteResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "statusCode")]
        public HttpStatusCode StatusCode { get; set; }

        [JsonProperty(PropertyName = "error")]
        public ErrorObject Error { get; set; }

        [JsonProperty(PropertyName = "data")]
        public DeleteData Data { get; set; }

        public static DeleteResponse FromData<TItem>(DeleteData data, HttpStatusCode httpStatusCode)
            => new DeleteResponse { Data = data, Success = true, StatusCode = httpStatusCode };

        public static DeleteResponse FromError<TItem>(DeleteResponse deleteResponse)
            => new DeleteResponse() { Error = deleteResponse.Error, StatusCode = deleteResponse.StatusCode };
    }
}
