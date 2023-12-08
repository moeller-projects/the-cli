using Moeller.TheCli.Domain.Personio.Models.Attributes;

namespace Moeller.TheCli.Domain.Personio.Models.Response;

public class GetAttendancesResponse : BasePagedListResponse<AttendanceAttributes, Attendance>
{
    protected override Func<AttendanceAttributes, Attendance> Converter => x => x.ToAttendance();
}