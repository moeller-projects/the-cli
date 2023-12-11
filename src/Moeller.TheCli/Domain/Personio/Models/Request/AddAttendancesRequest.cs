using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Request
{
    public class AddAttendancesRequest
    {
        /// <summary>
        /// Optional, default value is true. If set to false, the approval status 
        /// of the attendance period will be "pending" if an approval rule is set 
        /// for the attendances type. The respective approval flow will be triggered.
        /// </summary>
        [JsonProperty(PropertyName = "skip_approval")]
        public bool SkipApproval { get; set; } = true;

        /// <summary>
        /// array of objects
        /// </summary>
        [JsonProperty(PropertyName = "attendances")]
        public IEnumerable<Attendance>? Attendances { get; set; }
    }
}
