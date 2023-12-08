using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Response;

public class BatchResponse
{
    [JsonProperty(PropertyName = "id")]
    public IReadOnlyCollection<long>? Ids { get; set; }
    
    [JsonProperty(PropertyName = "message")]
    public string Message { get; set; }
}