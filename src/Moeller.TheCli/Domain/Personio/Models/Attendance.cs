using Moeller.TheCli.Domain.Personio.Util;
using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models
{
    public class Attendance
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }
        
        [JsonProperty(PropertyName = "employee")]
        public int EmployeeId { get; set; }

        [JsonProperty(PropertyName = "date"), JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly Date { get; set; }

        [JsonProperty(PropertyName = "start_time"), JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan StartTime { get; set; }

        [JsonProperty(PropertyName = "end_time"), JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan EndTime { get; set; }

        [JsonProperty(PropertyName = "break")]
        public decimal Break { get; set; }

        [JsonProperty(PropertyName = "project_id")]
        public int? ProjectId { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }
    }
}
