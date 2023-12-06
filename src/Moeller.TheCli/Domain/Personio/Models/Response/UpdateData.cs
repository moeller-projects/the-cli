using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public class UpdateData
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    }
}
