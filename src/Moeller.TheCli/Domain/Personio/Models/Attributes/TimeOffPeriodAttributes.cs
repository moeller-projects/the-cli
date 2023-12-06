using Moeller.TheCli.Domain.Personio.Models.Response;
using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Attributes
{
    public class TimeOffPeriodAttributes
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

        [JsonProperty(PropertyName = "created_by")]
        public string CreatedBy { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty(PropertyName = "updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty(PropertyName = "time_off_type")]
        public TypeAndAttributesObject<TimeOffTypeAttributes> TimeOffType { get; set; }

        [JsonProperty(PropertyName = "employee")]
        public TypeAndAttributesObject<EmployeeAttributes> Employee { get; set; }

        [JsonProperty(PropertyName = "email")]
        public AttributeObject<string> Email { get; set; }

        public static implicit operator TimeOffPeriod(TimeOffPeriodAttributes x)
            => new TimeOffPeriod()
            {
                Comment = x.Comment,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                DaysCount = x.DaysCount,
                EmployeeId = x.Employee.Attributes.Id,
                EndDate = x.EndDate,
                HalfDayEnd = x.HalfDayEnd,
                HalfDayStart = x.HalfDayStart,
                Id = x.Id,
                StartDate = x.StartDate,
                Status = x.Status,
                TimeOffTypeId = x.TimeOffType.Attributes.Id,
                UpdatedAt = x.UpdatedAt
            };

        public TimeOffPeriod ToTimeOffPeriod() => (TimeOffPeriod)this;

    }
}
