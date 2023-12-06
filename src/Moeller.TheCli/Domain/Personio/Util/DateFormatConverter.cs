using Moeller.TheCli.Domain.Personio.Common;
using Newtonsoft.Json.Converters;

namespace Moeller.TheCli.Domain.Personio.Util
{
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter() : this(Constants.DATE_FORMAT) { }
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
