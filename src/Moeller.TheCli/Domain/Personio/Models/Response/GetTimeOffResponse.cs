using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public class GetTimeOffResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "date")]
        public object Data { get; set; }
#warning TODO
    }

    public class CreateAttendancesResponse : ErrorResponse
    {

    [JsonProperty(PropertyName = "data")] public CreatedAttendances Data { get; set; }
#warning TODO
    }

    public class CreatedAttendances
    {
        public IReadOnlyCollection<id> Id { get; set; }
    }
}
