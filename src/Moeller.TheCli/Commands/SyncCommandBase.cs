using Moeller.TheCli.Domain;
using Moeller.TheCli.Domain.Personio;
using Moeller.TheCli.Domain.Personio.Configuration;
using Moeller.TheCli.Domain.Personio.Models;
using Moeller.TheCli.Domain.Personio.Models.Request;
using Moeller.TheCli.Infrastructure;
using Toggl;
using Toggl.QueryObjects;

namespace Moeller.TheCli.Commands;

public abstract class SyncCommandBase
{
    private readonly Settings _Settings;

    protected SyncCommandBase(ConfigurationProvider configurationProvider)
    {
        _Settings = configurationProvider?.Get() ?? throw new ArgumentNullException(nameof(configurationProvider));
    }
    
    protected async ValueTask<List<TimeEntry>> GetTimeEntries(DateTime from, DateTime to)
    {
        var togglClient = new TogglAsync(_Settings?.TogglSettings?.ApiToken);
        var timeEntries = await togglClient.TimeEntry.List(new TimeEntryParams
        {
            StartDate = from,
            EndDate = to
        });

        return timeEntries;
    }
    
    protected async ValueTask SyncTimeEntriesToPersonio(IEnumerable<TimeEntry> timeEntries)
    {
        var personioClient = new PersonioClient(new PersonioClientOptions(){ClientId = _Settings.PersonioSettings.ClientId, ClientSecret = _Settings.PersonioSettings.ClientSecret});
        await personioClient.AuthAsync();

        var attendances = timeEntries
            .Where(e => !string.IsNullOrWhiteSpace(e.Start) && !string.IsNullOrWhiteSpace(e.Stop) && e.Duration.GetValueOrDefault() > 0)
            .Select(e =>
            {
                var start = DateTime.Parse(e.Start);
                var stop = DateTime.Parse(e.Stop);
                return new Attendance
                {
                    EmployeeId = _Settings.PersonioSettings.EmployeeId,
                    Comment = e.Description,
                    Date = start.Date,
                    StartTime = new TimeSpan(start.Hour, start.Minute, start.Second),
                    EndTime = new TimeSpan(stop.Hour, stop.Minute, stop.Second),
                    Break = 0
                };
            });
        await personioClient.CreateAttendancesAsync(new AddAttendancesRequest() {SkipApproval = true, Attendances = attendances});
    }
}