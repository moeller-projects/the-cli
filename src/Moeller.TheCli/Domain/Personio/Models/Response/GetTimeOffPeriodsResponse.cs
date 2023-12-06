using Moeller.TheCli.Domain.Personio.Models.Attributes;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public class GetTimeOffPeriodsResponse : BasePagedListResponse<TimeOffPeriodAttributes, TimeOffPeriod>
    {
        protected override Func<TimeOffPeriodAttributes, TimeOffPeriod> Converter => x => x.ToTimeOffPeriod();
    }
}
