using System.Globalization;
using CliFx;
using CliFx.Attributes;
using ConsoleTableExt;
using Humanizer;
using Moeller.TheCli.Domain;
using Moeller.TheCli.Infrastructure;
using TogglAPI.NetStandard.Api;
using TogglAPI.NetStandard.Model;
using IConsole = CliFx.Infrastructure.IConsole;

namespace Moeller.TheCli.Commands;

[Command("report yesterday", Description = "gets the time entries for yesterday")]
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

[Command("report today", Description = "gets the time entries for a today")]
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

[Command("report", Description = "gets the time entries for a specified date range")]
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
                DateTime? start = DateTime.TryParse(e.Start, out var parsedStart) ? parsedStart : null;
                DateTime? stop = DateTime.TryParse(e.Stop, out var parsedStop) ? parsedStop : null;
                return new
                {
                    Description = e.Description,
                    Start = start,
                    Stop = stop,
                    Duration = e.Duration is null or < 0 ? "running..." : TimeSpan.FromSeconds(e.Duration.GetValueOrDefault()).Humanize(),
                    Tags = e.Tags is not null && e.Tags.Any() ? string.Join(", ", e.Tags) : null
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
                .WithFormat(ConsoleTableBuilderFormat.Minimal)
                .AddRow("TOTAL", null, null, totalDuration.Humanize(), null)
                .ExportAndWriteLine();

            await console.Output.WriteLineAsync();
        }
        
    }

    private async ValueTask<List<ModelsTimeEntry>> GetTimeEntries(IConsole console, DateTime from, DateTime to)
    {
        using (_ = new Spinner(console, "querying time entries from Toggl"))
        {
            SetupTogglClient(console);
            var timeEntries = await new TimeEntriesApi().GetTimeEntriesAsync(null, null, from.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd"));
            return timeEntries;
        }
    }
}