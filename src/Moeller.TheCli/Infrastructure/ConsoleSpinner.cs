using CliFx.Infrastructure;

namespace Moeller.TheCli.Infrastructure;

public class ConsoleSpinner
{
    private const string TURN_BUF = "/-\\|";

    private int _counter = 0;
    private bool IsLoading;
    private readonly IConsole _Console;
    private string Text;
    private string LeftBrackets;
    private string RightBrackets;
    private int Delay;
    private PositionSpinner Position;

    public enum PositionSpinner
    {
        Left,
        Right
    }

    public ConsoleSpinner(IConsole console, string entry, string leftBrackets = "[", string rightBrackets = "]", int delay = 700, PositionSpinner position = PositionSpinner.Left)
    {
        _Console = console;
        Text = entry;
        LeftBrackets = leftBrackets;
        RightBrackets = rightBrackets;
        Delay = delay;
        Position = position;
        IsLoading = true;

        Console.SetCursorPosition(GetPositionText(), _Console.CursorTop);
        _Console.Output.Write(Text);

        Console.SetCursorPosition(GetPositionSpinner(), _Console.CursorTop);
    }

    public void Turn()
    {
        Console.CursorVisible = false;

        while (IsLoading)
        {
            _Console.Output.Write($"{LeftBrackets}{TURN_BUF[_counter++ % 4]}{RightBrackets}");
            Console.SetCursorPosition(_Console.CursorLeft - 3, _Console.CursorTop);
            Thread.Sleep(Delay);
        }
    }

    private int GetPositionText() => Position switch
    {
        PositionSpinner.Left => 4,
        PositionSpinner.Right => 0,
        _ => throw new ArgumentOutOfRangeException()
    };

    private int GetPositionSpinner() => Position switch
    {
        PositionSpinner.Left => 0,
        PositionSpinner.Right => _Console.CursorLeft + 1,
        _ => throw new ArgumentOutOfRangeException()
    };

    /// <summary>
    /// Stopping the spinner animation
    /// </summary>
    /// <param name="textInBrackets">The text that will replace the spinner. Default = OK</param>
    /// <param name="colorText">The color of the brackets and the text inside them. Default = Green</param>
    public void Stop(string textInBrackets = "OK", ConsoleColor colorText = ConsoleColor.Green)
    {
        IsLoading = false;

        Console.SetCursorPosition(GetPositionSpinner(), _Console.CursorTop);

        _Console.ForegroundColor = colorText;
        _Console.Output.Write($"{LeftBrackets}{textInBrackets}{RightBrackets} ");
        _Console.ResetColor();
        _Console.Output.Write($"{Text}");

        Console.SetCursorPosition(0, _Console.CursorTop + 1);
        Console.CursorVisible = true;
    }
}

public class Spinner : IDisposable
{
    private ConsoleSpinner _Spinner { get; init; }

    public Spinner(IConsole console, string? description)
    {
        _Spinner = new ConsoleSpinner(console, description ?? null);
        Task.Run(() => _Spinner.Turn());
    }
    
    public void Dispose()
    {
        _Spinner.Stop("DONE");
    }
}