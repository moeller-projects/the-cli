using Moeller.TheCli.Domain.Personio.Util;
using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public abstract class BasePagedListResponse<TResult> : BaseResponse
    {
        public abstract PagedList<TResult> ToPagedList();
    }
    public abstract class BasePagedListResponse<TAttributes, TResult> : BasePagedListResponse<TResult>
    {
        [JsonProperty(PropertyName = "data")]
        public TypeAndAttributesObject<TAttributes>[] Data { get; set; }

        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty(PropertyName = "offset")]
        public int Offset { get; set; }

        [JsonProperty(PropertyName = "limit")]
        public int Limit { get; set; }

        protected abstract Func<TAttributes, TResult> Converter { get; }
        public override PagedList<TResult> ToPagedList()
        {
            var pagedList = new PagedList<TResult>();
            pagedList.Data = Data?.Select(x => Converter(x.Attributes))?.ToList();
            pagedList.Offset = Offset;
            pagedList.Limit = Limit;
            pagedList.TotalElements = Metadata?.TotalElements ?? 0;
            pagedList.CurrentPage = Metadata?.CurrentPage ?? 0;
            pagedList.TotalPages = Metadata?.TotalPages ?? 0;
            return pagedList;
        }
    }
}
