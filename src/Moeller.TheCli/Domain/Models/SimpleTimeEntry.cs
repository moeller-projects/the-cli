using System.Globalization;
using TogglAPI.NetStandard.Model;

namespace Moeller.TheCli.Domain.Models;

public class SimpleTimeEntry
{
    public DateTime Start { get; set; }
    public DateTime Stop { get; set; }
    
    public static implicit operator SimpleTimeEntry(ModelsTimeEntry timeEntry)
    {
        var start = DateTime.Parse(timeEntry.Start, null, DateTimeStyles.AssumeLocal); //.TryParseExact(timeEntry.Start, TogglSettings.DATE_TIME_FORMAT, null, DateTimeStyles.AssumeLocal, out var parsedStart) ? parsedStart : DateTime.MinValue;
        var stop = DateTime.Parse(timeEntry.Stop, null, DateTimeStyles.AssumeLocal); //.TryParseExact(timeEntry.Stop, TogglSettings.DATE_TIME_FORMAT, null, DateTimeStyles.AssumeLocal, out var parsedStop) ? parsedStop : DateTime.MinValue;
        return new SimpleTimeEntry 
        { 
            Start = start, 
            Stop = stop 
        };
    }
}