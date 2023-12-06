namespace Moeller.TheCli.Domain.Personio.Models.Request
{
    public class GetEmployeesRequest
    {
        /// <summary>
        /// Minvalue = 1
        /// </summary>
        public int Limit { get; set; } = 10;

        /// <summary>
        /// Minvalue = 0
        /// </summary>
        public int Offset { get; set; }
        public string Email { get; set; }
        public DateTime? UpdatedSince { get; set; }
        public IEnumerable<string> Attributes { get; set; }
    }
}
