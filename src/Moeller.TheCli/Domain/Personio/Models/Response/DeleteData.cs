using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public class DeleteData
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
