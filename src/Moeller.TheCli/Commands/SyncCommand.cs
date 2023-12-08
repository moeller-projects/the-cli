using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Moeller.TheCli.Domain;
using Moeller.TheCli.Domain.Personio;
using Moeller.TheCli.Domain.Personio.Configuration;
using Moeller.TheCli.Domain.Personio.Models;
using Moeller.TheCli.Domain.Personio.Models.Request;
using Moeller.TheCli.Infrastructure;
using Moeller.TheCli.Infrastructure.Extensions;
using Sharprompt;
using Toggl;
using Toggl.QueryObjects;
using Task = System.Threading.Tasks.Task;

namespace Moeller.TheCli.Commands;

[Command("sync yesterday", Description = "todo")]
public class SyncYesterdayCommand : ICommand
{
    private readonly SyncCommand _Command;

    public SyncYesterdayCommand(SyncCommand command)
    {
        _Command = command ?? throw new ArgumentNullException(nameof(command));
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        _Command.Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        return _Command.ExecuteAsync(console);
    }
}

[Command("sync today", Description = "todo")]
public class SyncTodayCommand : ICommand
{
    private readonly SyncCommand _Command;

    public SyncTodayCommand(SyncCommand command)
    {
        _Command = command ?? throw new ArgumentNullException(nameof(command));
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        _Command.Date = DateOnly.FromDateTime(DateTime.Today);
        return _Command.ExecuteAsync(console);
    }
}

[Command("sync", Description = "Sync Toggl Entries to Personio TimeTracking")]
public class SyncCommand : ICommand
{
    [CommandParameter(0, Name = "Date", IsRequired = false)] public DateOnly? Date { get; set; }
    [CommandOption("from", 'f', Description = "From")] public DateOnly? From { get; set; }
    [CommandOption("to", 't', Description = "To")] public DateOnly? To { get; set; }

    
    private readonly Settings _Settings;
    private TogglAsync? _TogglClient;
    private PersonioClient? _PersonioClient;

    public SyncCommand(ConfigurationProvider provider)
    {
        _Settings = provider.Get();
    }
    
    public async ValueTask ExecuteAsync(IConsole console)
    {
        if (!Date.HasValue && !From.HasValue && !To.HasValue)
        {
            throw new Exception("You have to specify at least one Date Argument");
        }

        if (Date.HasValue)
        {
            From = Date.Value;
            To = Date.Value.AddDays(1);
        }
        
        var timeEntries = await GetTimeEntries(console, From.Value.ToDateTime(TimeOnly.MinValue), To.Value.ToDateTime(TimeOnly.MinValue));
        await DeleteExistingTimeEntries(console, From.Value.ToDateTime(TimeOnly.MinValue), To.Value.ToDateTime(TimeOnly.MinValue));
        await SyncTimeEntriesToPersonio(console, timeEntries);
    }

    private async Task DeleteExistingTimeEntries(IConsole console, DateTime from, DateTime to)
    {
        var personioClient = await GetPersonioClient(console);
        var existingAttendances = await personioClient.GetAttendancesAsync(new GetAttendencesRequest
        {
            StartDate = from,
            EndDate = to,
            Limit = 200,
            Offset = 0,
            EmployeeIds = new []{_Settings.PersonioSettings.EmployeeId}
        });

        if (existingAttendances?.PagedList.TotalElements > 0)
        {
            var deleteTasks = existingAttendances.PagedList.Data.Select(async a => await _PersonioClient.DeleteAttendancesAsync(a.Id, true)).ToArray();
            Task.WaitAll(deleteTasks);
        }
    }

    private async ValueTask<List<TimeEntry>> GetTimeEntries(IConsole console, DateTime from, DateTime to)
    {
        var togglClient = await GetTogglClient(console);
        var timeEntries = await togglClient.TimeEntry.List(new TimeEntryParams
        {
            StartDate = from,
            EndDate = to
        });

        return timeEntries;
    }

    private async Task<TogglAsync> GetTogglClient(IConsole console)
    {
        if (string.IsNullOrWhiteSpace(_Settings?.TogglSettings?.ApiToken))
        {
            await console.RespondWithFailureAsync("Toggl Connection not ready, please check your Api Token and feel free to re-init your cli");
            Environment.Exit(-1);
        }

        return _TogglClient ??= new TogglAsync(_Settings?.TogglSettings?.ApiToken);
    }
    
    private async Task<PersonioClient> GetPersonioClient(IConsole console)
    {
        if (string.IsNullOrWhiteSpace(_Settings?.PersonioSettings?.ClientId) || string.IsNullOrWhiteSpace(_Settings.PersonioSettings.ClientSecret))
        {
            await console.RespondWithFailureAsync("Personio Connection not ready, please check your Api Token and feel free to re-init your cli");
            Environment.Exit(-1);
        }

        if (_PersonioClient is not null)
            return _PersonioClient;
        
        _PersonioClient = new PersonioClient(new PersonioClientOptions(){ClientId = _Settings.PersonioSettings.ClientId, ClientSecret = _Settings.PersonioSettings.ClientSecret});
        await _PersonioClient.AuthAsync();
        if (!_PersonioClient.IsReady)
        {
            await console.RespondWithFailureAsync("Personio Connection not ready, please check your Api Token and feel free to re-init your cli");
            Environment.Exit(-1);
        }

        return _PersonioClient;
    }

    private async ValueTask SyncTimeEntriesToPersonio(IConsole console, IEnumerable<TimeEntry> timeEntries)
    {
        var personioClient = await GetPersonioClient(console);
        var attendances = ConvertToAttendances(timeEntries);
        await personioClient.CreateAttendancesAsync(new AddAttendancesRequest() {SkipApproval = true, Attendances = attendances});
    }

    private IEnumerable<Attendance> ConvertToAttendances(IEnumerable<TimeEntry> timeEntries)
    {
        var simpleTimeEntries = timeEntries
            .Where(e => !string.IsNullOrWhiteSpace(e.Start) && !string.IsNullOrWhiteSpace(e.Stop) && e.Duration.GetValueOrDefault() > 0)
            .Select(e => (SimpleTimeEntry) e)
            .OrderBy(e => e.Start).ThenBy(e => e.Stop)
            .ToArray();
        
        var groupedTimeEntries = new List<List<SimpleTimeEntry>>();
        var group1 = new List<SimpleTimeEntry>() {simpleTimeEntries[0]};
        groupedTimeEntries.Add(group1);

        var last = simpleTimeEntries[0];
        for (var i = 1; i < simpleTimeEntries.Length; i++)
        {
            var current = simpleTimeEntries[i];
            var timeDiff = current.Start - last.Stop;
            var isNewGroup = timeDiff.TotalMinutes > 5;
            if (isNewGroup)
            {
                groupedTimeEntries.Add(new List<SimpleTimeEntry>());
            }

            groupedTimeEntries.Last().Add(current);
            last = current;
        }

        return groupedTimeEntries.Select(e => new Attendance()
        {
            EmployeeId = _Settings.PersonioSettings.EmployeeId,
            Date = e.Min(ste => DateOnly.FromDateTime(ste.Start)),
            StartTime = e.Min(m => new TimeSpan(m.Start.Hour, m.Start.Minute, m.Start.Second)),
            EndTime = e.Max(m => new TimeSpan(m.Stop.Hour, m.Stop.Minute, m.Stop.Second)),
            Break = 0
        });
    }
}