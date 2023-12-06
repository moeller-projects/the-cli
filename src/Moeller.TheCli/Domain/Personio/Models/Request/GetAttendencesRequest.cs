namespace Moeller.TheCli.Domain.Personio.Models.Request
{
    public class GetAttendencesRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? UpdatedFrom { get; set; }
        public DateTime? UpdatedTo { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public IEnumerable<int> EmployeeIds { get; set; }
    }
}
