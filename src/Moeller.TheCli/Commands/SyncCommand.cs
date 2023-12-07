using System.Globalization;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Moeller.TheCli.Domain;
using Moeller.TheCli.Domain.Personio;
using Moeller.TheCli.Domain.Personio.Configuration;
using Moeller.TheCli.Domain.Personio.Models;
using Moeller.TheCli.Domain.Personio.Models.Request;
using Moeller.TheCli.Domain.Personio.Models.Response;
using Moeller.TheCli.Infrastructure;
using Moeller.TheCli.Infrastructure.Extensions;
using Toggl;
using Toggl.QueryObjects;

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
        await CheckForExistingAttendances(console, From.Value.ToDateTime(TimeOnly.MinValue), To.Value.ToDateTime(TimeOnly.MinValue));
        await SyncTimeEntriesToPersonio(console, timeEntries);
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
        var attendances = timeEntries
            .Where(e => !string.IsNullOrWhiteSpace(e.Start) && !string.IsNullOrWhiteSpace(e.Stop) && e.Duration.GetValueOrDefault() > 0)
            .Select(e =>
            {
                var start = DateTime.TryParseExact(e.Start, TogglSettings.DATE_TIME_FORMAT, null, DateTimeStyles.AssumeLocal, out var parsedStart) ? parsedStart : DateTime.MinValue;
                var stop = DateTime.TryParseExact(e.Stop, TogglSettings.DATE_TIME_FORMAT, null, DateTimeStyles.AssumeLocal, out var parsedStop) ? parsedStop : DateTime.MinValue;
                return new Attendance
                {
                    EmployeeId = _Settings.PersonioSettings.EmployeeId,
                    Comment = e.Description,
                    Date = DateOnly.FromDateTime(start),
                    StartTime = new TimeSpan(start.Hour, start.Minute, start.Second),
                    EndTime = new TimeSpan(stop.Hour, stop.Minute, stop.Second),
                    Break = 0
                };
            }).ToArray();
        await personioClient.CreateAttendancesAsync(new AddAttendancesRequest() {SkipApproval = true, Attendances = attendances});
    }
    
    private async ValueTask CheckForExistingAttendances(IConsole console, DateTime from, DateTime to)
    {
        var personioClient = await GetPersonioClient(console);
        await personioClient.GetAttendancesAsync(new GetAttendencesRequest()
        {
            StartDate = from,
            EndDate = to,
            Limit = 200,
            Offset = 0,
            EmployeeIds = new []{_Settings.PersonioSettings.EmployeeId}
        });
    }
}