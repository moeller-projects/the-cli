using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using ConsoleTableExt;
using Moeller.TheCli.Domain;
using Moeller.TheCli.Infrastructure;
using Toggl;
using Toggl.QueryObjects;

namespace Moeller.TheCli.Commands;

[Command("report yesterday", Description = "todo")]
public class ReportYesterdayCommand : ICommand
{
    private readonly ReportCommand _ReportCommand;

    public ReportYesterdayCommand(ReportCommand reportCommand)
    {
        _ReportCommand = reportCommand;
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        _ReportCommand.Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        return _ReportCommand.ExecuteAsync(console);
    }
}

[Command("report today", Description = "todo")]
public class ReportTodayCommand : ICommand
{
    private readonly ReportCommand _ReportCommand;

    public ReportTodayCommand(ReportCommand reportCommand)
    {
        _ReportCommand = reportCommand;
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        _ReportCommand.Date = DateOnly.FromDateTime(DateTime.Today);
        return _ReportCommand.ExecuteAsync(console);
    }
}


[Command("report", Description = "todo")]
public class ReportCommand : ICommand
{
    [CommandParameter(0, Name = "Date", IsRequired = false)] public DateOnly? Date { get; set; }
    [CommandOption("from", 'f', Description = "From")] public DateOnly? From { get; set; }
    [CommandOption("to", 't', Description = "To")] public DateOnly? To { get; set; }
    
    
    private readonly Settings _Settings;
    
    public ReportCommand(ConfigurationProvider provider)
    {
        _Settings = provider.Get();
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        if (!Date.HasValue && !From.HasValue && !To.HasValue)
        {
            await console.Error.WriteLineAsync("You have to specify at least one Date Argument");
            return;
        }

        if (Date.HasValue)
        {
            From = Date.Value;
            To = Date.Value.AddDays(1);
        }
        
        var timeEntries = await GetTimeEntries(From.Value.ToDateTime(TimeOnly.MinValue), To.Value.ToDateTime(TimeOnly.MinValue));

        var table = timeEntries.OrderBy(e => e.Id).Select(e =>
        {
            return new
            {
                Description = e.Description,
                Start = e.Start,
                Stop = e.Stop,
                Duration = e.Duration is null or < 0 ? "running..." : TimeSpan.FromSeconds(e.Duration.GetValueOrDefault()).ToString("hh\\hmm\\m"),
                Tags = e.TagNames is not null && e.TagNames.Any() ? string.Join(", ", e.TagNames) : null
            };
        }).ToList();
        table.Add(new
        {
            Description = "SUM",
            Start = string.Empty,
            Stop = string.Empty,
            Duration = TimeSpan.FromSeconds(timeEntries.Sum(e => e.Duration is not null && e.Duration.Value >= 0 ? e.Duration.GetValueOrDefault() : 0)).ToString("hh\\hmm\\m"),
            Tags = string.Empty
        });
        
        ConsoleTableBuilder
            .From(table)
            .WithFormat(ConsoleTableBuilderFormat.Minimal)
            .ExportAndWriteLine();
    }
    
    private async ValueTask<List<TimeEntry>> GetTimeEntries(DateTime from, DateTime to)
    {
        var togglClient = new TogglAsync(_Settings?.TogglSettings?.ApiToken);
        var timeEntries = await togglClient.TimeEntry.List(new TimeEntryParams
        {
            StartDate = from,
            EndDate = to
        });

        return timeEntries;
    }
}