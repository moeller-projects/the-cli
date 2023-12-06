namespace Moeller.TheCli.Domain.Personio.Util
{
    public class PagedList<T>
    {
        public List<T> Data { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int TotalElements { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
