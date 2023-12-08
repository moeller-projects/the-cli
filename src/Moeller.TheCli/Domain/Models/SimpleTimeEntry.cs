using System.Globalization;
using Moeller.TheCli.Infrastructure;
using Toggl;

namespace Moeller.TheCli.Domain.Models;

public class SimpleTimeEntry
{
    public DateTime Start { get; set; }
    public DateTime Stop { get; set; }
    
    public static implicit operator SimpleTimeEntry(TimeEntry timeEntry)
    {
        var start = DateTime.TryParseExact(timeEntry.Start, TogglSettings.DATE_TIME_FORMAT, null, DateTimeStyles.AssumeLocal, out var parsedStart) ? parsedStart : DateTime.MinValue;
        var stop = DateTime.TryParseExact(timeEntry.Stop, TogglSettings.DATE_TIME_FORMAT, null, DateTimeStyles.AssumeLocal, out var parsedStop) ? parsedStop : DateTime.MinValue;
        return new SimpleTimeEntry 
        { 
            Start = start, 
            Stop = stop 
        };
    }
}