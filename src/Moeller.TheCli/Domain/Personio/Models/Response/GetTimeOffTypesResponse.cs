using Moeller.TheCli.Domain.Personio.Models.Attributes;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public class GetTimeOffTypesResponse : BasePagedListResponse<TimeOffTypeAttributes, TimeOffType>
    {
        protected override Func<TimeOffTypeAttributes, TimeOffType> Converter => x => x.ToTimeOffType();
    }
}
