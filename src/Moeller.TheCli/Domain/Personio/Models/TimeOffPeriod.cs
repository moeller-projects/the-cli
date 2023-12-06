using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models
{
    public class TimeOffPeriod
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }

        [JsonProperty(PropertyName = "start_date")]
        public DateTimeOffset StartDate { get; set; }

        [JsonProperty(PropertyName = "end_date")]
        public DateTimeOffset EndDate { get; set; }

        [JsonProperty(PropertyName = "days_count")]
        public decimal DaysCount { get; set; }

        [JsonProperty(PropertyName = "half_day_start")]
        public bool HalfDayStart { get; set; }

        [JsonProperty(PropertyName = "half_day_end")]
        public bool HalfDayEnd { get; set; }

        //[JsonProperty(PropertyName = "time_off_type")]
        //public TimeOffType TimeOffType { get; set; }
        [JsonProperty(PropertyName = "time_off_type_id")]
        public int TimeOffTypeId { get; set; }

        //[JsonProperty(PropertyName = "employee")]
        //public Employee Employee { get; set; }
        [JsonProperty(PropertyName = "employee_id")]
        public int EmployeeId { get; set; }

        [JsonProperty(PropertyName = "created_by")]
        public string CreatedBy { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty(PropertyName = "updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}