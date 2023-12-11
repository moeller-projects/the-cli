using System.Globalization;
using CliFx;
using CliFx.Attributes;
using ConsoleTableExt;
using Moeller.TheCli.Domain;
using Moeller.TheCli.Infrastructure;
using Toggl;
using Toggl.QueryObjects;
using IConsole = CliFx.Infrastructure.IConsole;
using Task = System.Threading.Tasks.Task;

namespace Moeller.TheCli.Commands;

[Command("report yesterday", Description = "todo")]
public class ReportYesterdayCommand : ICommand
{
    private readonly ReportCommand _Command;

    public ReportYesterdayCommand(ReportCommand command)
    {
        _Command = command;
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        _Command.Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        return _Command.ExecuteAsync(console);
    }
}

[Command("report today", Description = "todo")]
public class ReportTodayCommand : ICommand
{
    private readonly ReportCommand _Command;

    public ReportTodayCommand(ReportCommand command)
    {
        _Command = command;
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        _Command.Date = DateOnly.FromDateTime(DateTime.Today);
        return _Command.ExecuteAsync(console);
    }
}

[Command("report", Description = "todo")]
public class ReportCommand : CommandBase, ICommand
{
    [CommandParameter(0, Name = "Date", IsRequired = false)]
    public DateOnly? Date { get; set; }

    [CommandOption("from", 'f', Description = "From")]
    public DateOnly? From { get; set; }

    [CommandOption("to", 't', Description = "To")]
    public DateOnly? To { get; set; }

    public ReportCommand(ConfigurationProvider provider) : base(provider)
    {}

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

        var timeEntries = await GetTimeEntries(console, From.Value.ToDateTime(TimeOnly.MinValue), To.Value.ToDateTime(TimeOnly.MinValue));

        var tables = timeEntries
            .Select(e =>
            {
                DateTime? start = DateTime.TryParseExact(e.Start, TogglSettings.DATE_TIME_FORMAT, null, DateTimeStyles.None, out var parsedStart) ? parsedStart : null;
                DateTime? stop = DateTime.TryParseExact(e.Stop, TogglSettings.DATE_TIME_FORMAT, null, DateTimeStyles.None, out var parsedStop) ? parsedStop : null;
                return new
                {
                    Description = e.Description,
                    Start = start,
                    Stop = stop,
                    Duration = e.Duration is null or < 0 ? "running..." : TimeSpan.FromSeconds(e.Duration.GetValueOrDefault()).ToString("hh\\hmm\\m"),
                    Tags = e.TagNames is not null && e.TagNames.Any() ? string.Join(", ", e.TagNames) : null
                };
            }).GroupBy(g => DateOnly.FromDateTime(g.Start.GetValueOrDefault()));

        foreach (var table in tables)
        {
            var totalDuration = table
                .Where(e => e.Start.HasValue && e.Stop.HasValue)
                .Select(e => e.Stop - e.Start)
                .Aggregate((aggregated, current) => aggregated + current)
                .GetValueOrDefault();

            ConsoleTableBuilder
                .From(table.OrderBy(o => o.Start).ThenBy(o => o.Stop).ToList())
                .WithTitle(table.Key.ToString(), ConsoleColor.Magenta, TextAligntment.Left)
                .WithFormat(ConsoleTableBuilderFormat.Alternative)
                .AddRow("TOTAL", null, null, totalDuration.ToString("hh\\hmm\\m"), null)
                .ExportAndWriteLine();

            await console.Output.WriteLineAsync();
        }
    }

    private async ValueTask<List<TimeEntry>> GetTimeEntries(IConsole console, DateTime from, DateTime to)
    {
        // var spinner = new ConsoleSpinner(console, "Loading Toggl TimeEntries", ConsoleSpinner.PositionSpinner.Left);
        // Task.Run(() => spinner.Turn());
        var togglClient = await GetTogglClient(console);
        var timeEntries = await togglClient.TimeEntry.List(new TimeEntryParams
        {
            StartDate = from,
            EndDate = to
        });
        // spinner.Stop("DONE");
        return timeEntries;
    }
}