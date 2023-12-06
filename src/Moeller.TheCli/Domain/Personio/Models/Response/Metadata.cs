using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public class Metadata
    {
        [JsonProperty(PropertyName = "total_elements")]
        public int TotalElements { get; set; }

        [JsonProperty(PropertyName = "current_page")]
        public int CurrentPage { get; set; }

        [JsonProperty(PropertyName = "total_pages")]
        public int TotalPages { get; set; }
    }
}
