namespace Moeller.TheCli.Domain.Personio.Models.Request
{
    public class GetAbsencePeriodsRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime UpdatedFrom { get; set; }
        public DateTime UpdatedTo { get; set; }
        public IEnumerable<int>? EmployeeIds { get; set; }
        public IEnumerable<string>? AbsenceTypes { get; set; }
        public IEnumerable<string>? AbsencePeriods { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}
