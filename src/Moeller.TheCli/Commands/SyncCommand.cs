using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Moeller.TheCli.Domain;
using Moeller.TheCli.Domain.Models;
using Moeller.TheCli.Domain.Personio.Models;
using Moeller.TheCli.Domain.Personio.Models.Request;
using Moeller.TheCli.Infrastructure;
using Sharprompt;
using TogglAPI.NetStandard.Api;
using TogglAPI.NetStandard.Model;
using Task = System.Threading.Tasks.Task;

namespace Moeller.TheCli.Commands;

[Command("sync yesterday", Description = "synchronizes the time entries for yesterday to Personio")]
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

[Command("sync today", Description = "synchronizes the time entries for today to Personio")]
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

[Command("sync", Description = "synchronizes the time entries for a specific date range to Personio")]
public class SyncCommand : CommandBase, ICommand
{
    [CommandParameter(0, Name = "Date", IsRequired = false)] public DateOnly? Date { get; set; }
    [CommandOption("from", 'f', Description = "From")] public DateOnly? From { get; set; }
    [CommandOption("to", 't', Description = "To")] public DateOnly? To { get; set; }
    
    public SyncCommand(ConfigurationProvider provider) : base(provider)
    {}
    
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
        
        var timeEntries = await GetTimeEntries(console, From.GetValueOrDefault().ToDateTime(TimeOnly.MinValue), To.GetValueOrDefault().ToDateTime(TimeOnly.MinValue));
        await DeleteExistingTimeEntries(console, From.GetValueOrDefault().ToDateTime(TimeOnly.MinValue), To.GetValueOrDefault().ToDateTime(TimeOnly.MinValue));
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
            EmployeeIds = new []{Settings.PersonioSettings.EmployeeId}
        });

        if (existingAttendances?.PagedList.TotalElements > 0)
        {
            await console.Output.WriteLineAsync($"Found {existingAttendances?.PagedList.TotalElements} existing Attendances in Personio");
            if (!Prompt.Confirm("These will be deleted before syncing", true))
                return;
            
            var spi = new ConsoleSpinner(console, "Deleting existing Personio Attendances");
            Task.Run(() => spi.Turn());
            var deleteTasks = existingAttendances.PagedList.Data.Select(async a => await personioClient.DeleteAttendancesAsync(a.Id, true)).ToArray();
            Task.WaitAll(deleteTasks);
            spi.Stop("DONE");
        }
    }

    private async ValueTask<List<ModelsTimeEntry>> GetTimeEntries(IConsole console, DateTime from, DateTime to)
    {
        var spi = new ConsoleSpinner(console, "Query Time Entries from Toggl");
        Task.Run(() => spi.Turn());
        SetupTogglClient(console);
        
        var timeEntries = await new TimeEntriesApi().GetTimeEntriesAsync(null, null, from.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd"));

        spi.Stop("DONE");
        return timeEntries;
    }

    private async ValueTask SyncTimeEntriesToPersonio(IConsole console, IEnumerable<ModelsTimeEntry> timeEntries)
    {
        var personioClient = await GetPersonioClient(console);
        var attendances = ConvertToAttendances(timeEntries);
        
        var spi = new ConsoleSpinner(console, $"Syncing {timeEntries.Count()} TimeEntries as Attendances to Personio");
        Task.Run(() => spi.Turn());
        await personioClient.CreateAttendancesAsync(new AddAttendancesRequest() {SkipApproval = true, Attendances = attendances});
        spi.Stop("DONE");
    }

    private IEnumerable<Attendance> ConvertToAttendances(IEnumerable<ModelsTimeEntry> timeEntries)
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
            EmployeeId = Settings.PersonioSettings.EmployeeId,
            Date = e.Min(ste => DateOnly.FromDateTime(ste.Start)),
            StartTime = e.Min(m => new TimeSpan(m.Start.Hour, m.Start.Minute, m.Start.Second)),
            EndTime = e.Max(m => new TimeSpan(m.Stop.Hour, m.Stop.Minute, m.Stop.Second)),
            Break = 0
        });
    }
}