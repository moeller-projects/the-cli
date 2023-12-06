using System.Net;
using Moeller.TheCli.Domain.Personio.Util;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public class PagedListResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public PagedList<T> PagedList { get; set; }
        public Error Error { get; set; }
        public string Raw { get; set; }
    }
}
