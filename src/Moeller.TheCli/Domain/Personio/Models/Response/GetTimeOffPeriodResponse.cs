using Moeller.TheCli.Domain.Personio.Models.Attributes;
using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public class GetTimeOffPeriodResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "data")]
        public TypeAndAttributesObject<TimeOffPeriodAttributes> Data { get; set; }
    }
}
