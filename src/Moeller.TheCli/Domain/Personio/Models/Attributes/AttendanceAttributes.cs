using Moeller.TheCli.Domain.Personio.Util;
using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Attributes;

public class AttendanceAttributes
{
    [JsonProperty("employee")] public int Employee { get; set; }

    [JsonProperty("date"), JsonConverter(typeof(DateOnlyConverter))] public DateOnly Date { get; set; }

    [JsonProperty("start_time"), JsonConverter(typeof(TimeSpanConverter))] public TimeSpan StartTime { get; set; }

    [JsonProperty("end_time"), JsonConverter(typeof(TimeSpanConverter))] public TimeSpan EndTime { get; set; }

    [JsonProperty("break")] public int Break { get; set; }

    [JsonProperty("comment")] public string Comment { get; set; }

    [JsonProperty("updated_at")] public DateTime UpdatedAt { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("project")] public int? Project { get; set; }

    [JsonProperty("is_holiday")] public bool IsHoliday { get; set; }

    [JsonProperty("is_on_time_off")] public bool IsOnTimeOff { get; set; }

    public static implicit operator Attendance(AttendanceAttributes x)
        => x != null
            ? new Attendance()
            {
                EmployeeId = x.Employee,
                Date = x.Date,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Break = x.Break,
                Comment = x.Comment,
                ProjectId = x.Project
            }
            : null;

    public Attendance ToAttendance() => (Attendance) this;
}