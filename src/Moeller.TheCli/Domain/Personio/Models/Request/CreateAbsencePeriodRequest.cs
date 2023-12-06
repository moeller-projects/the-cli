using Moeller.TheCli.Domain.Personio.Util;
using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Request
{
    public class CreateAbsencePeriodRequest
    {
        [JsonProperty(PropertyName = "employee_id")]
        public int EmployeeId { get; set; }

        [JsonProperty(PropertyName = "time_off_type_id")]
        public int TimeOffTypeId { get; set; }

        [JsonProperty(PropertyName = "start_date"), JsonConverter(typeof(DateFormatConverter))]
        public DateTime StartDate { get; set; }

        [JsonProperty(PropertyName = "end_date"), JsonConverter(typeof(DateFormatConverter))]
        public DateTime EndDate { get; set; }

        [JsonProperty(PropertyName = "start_date"), JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan StartTime { get; set; }

        [JsonProperty(PropertyName = "end_date"), JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan EndTime { get; set; }

        [JsonProperty(PropertyName = "half_day_start")]
        public bool HalfDayStart { get; set; }

        [JsonProperty(PropertyName = "half_day_end")]
        public bool HalfDayEnd { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }

        [JsonProperty(PropertyName = "skip_approval")]
        public bool SkipApproval { get; set; } = true;
    }
}
