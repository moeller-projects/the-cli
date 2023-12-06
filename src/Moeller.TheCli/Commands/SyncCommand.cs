using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Moeller.TheCli.Domain;

namespace Moeller.TheCli.Commands;

[Command("sync yesterday", Description = "todo")]
public class SyncYesterdayCommand : SyncCommandBase, ICommand
{
    public SyncYesterdayCommand(ConfigurationProvider provider): base(provider)
    {
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var yesterday = DateTime.Today.AddDays(-1);
        var from = yesterday.Date;
        var to = yesterday.Date.AddDays(1);

        var timeEntries = await GetTimeEntries(from, to);

        await console.Output.WriteLineAsync($"Total Hours worked: {TimeSpan.FromSeconds(timeEntries.Where(e => e.Duration > 0).Sum(e => e.Duration.GetValueOrDefault())):g}");
    }
}

[Command("sync today", Description = "todo")]
public class SyncTodayCommand : SyncCommandBase, ICommand
{
    public SyncTodayCommand(ConfigurationProvider provider): base(provider)
    {
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var today = DateTime.Today;
        var from = today.Date;
        var to = today.Date.AddDays(1);

        var timeEntries = await GetTimeEntries(from, to);

        await console.Output.WriteLineAsync($"Total Hours worked: {TimeSpan.FromSeconds(timeEntries.Where(e => e.Duration > 0).Sum(e => e.Duration.GetValueOrDefault())):g}");
        await SyncTimeEntriesToPersonio(timeEntries);
    }
}

[Command("sync", Description = "Sync Toggl Entries to Personio TimeTracking")]
public class SyncCommand : SyncCommandBase, ICommand
{
    [CommandParameter(0, Name = "Date", IsRequired = false)] public DateOnly? Date { get; init; }
    [CommandOption("from", 'f', Description = "From")] public DateOnly? From { get; set; }
    [CommandOption("to", 't', Description = "To")] public DateOnly? To { get; set; }

    public SyncCommand(ConfigurationProvider provider): base(provider)
    {
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
        
        var timeEntries = await GetTimeEntries(From.Value.ToDateTime(TimeOnly.MinValue), To.Value.ToDateTime(TimeOnly.MinValue));
    }
}