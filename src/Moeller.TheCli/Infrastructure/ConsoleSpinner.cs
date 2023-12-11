using CliFx.Infrastructure;

namespace Moeller.TheCli.Infrastructure;

/// <summary>
/// Animation of a spinning spinner near the text
/// </summary>
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

    /// <summary>
    /// The position of the spinner in relation to the text
    /// </summary>
    public enum PositionSpinner
    {
        Left,
        Right
    }

    /// <summary>
    /// Initializing the animation spinner
    /// </summary>
    /// <param name="entry">*Text near the spinner</param>
    /// <param name="position">Position spinner (Left / Right). Default = Left</param>
    public ConsoleSpinner(IConsole console, string entry, PositionSpinner position = PositionSpinner.Left)
    {
        _Console = console;
        this.Text = entry;
        this.LeftBrackets = " ";
        this.RightBrackets = " ";
        this.Position = position;

        this.IsLoading = true;

        Console.SetCursorPosition(GetPositionText(), Console.CursorTop);
        Console.Write(Text);

        Console.SetCursorPosition(GetPositionSpinner(), Console.CursorTop);
    }

    /// <summary>
    /// Initializing the animation spinner
    /// </summary>
    /// <param name="entry">*Text near the spinner</param>
    /// <param name="delay">Animation delay rate (ms). Default = 700</param>
    /// <param name="position">Position spinner (Left / Right). Default = Left</param>
    public ConsoleSpinner(string entry, int delay = 700, PositionSpinner position = PositionSpinner.Left)
    {
        this.Text = entry;
        this.LeftBrackets = " ";
        this.RightBrackets = " ";
        this.Delay = delay;
        this.Position = position;

        this.IsLoading = true;

        Console.SetCursorPosition(GetPositionText(), _Console.CursorTop);
        _Console.Output.Write(Text);

        Console.SetCursorPosition(GetPositionSpinner(), _Console.CursorTop);
    }

    /// <summary>
    /// Initializing the animation spinner
    /// </summary>
    /// <param name="entry">*Text near the spinner</param>
    /// <param name="leftBrackets">Left bracket from the spinner. Default = " "</param>
    /// <param name="rightBrackets">Right bracket from the spinner. Default = " "</param>
    /// <param name="position">Position spinner (Left / Right). Default = Left</param>
    public ConsoleSpinner(string entry, string leftBrackets = " ", string rightBrackets = " ", PositionSpinner position = PositionSpinner.Left)
    {
        this.Text = entry;
        this.LeftBrackets = leftBrackets;
        this.RightBrackets = rightBrackets;
        this.Position = position;
        this.IsLoading = true;

        Console.SetCursorPosition(GetPositionText(), _Console.CursorTop);
        _Console.Output.Write(Text);

        Console.SetCursorPosition(GetPositionSpinner(), _Console.CursorTop);
    }

    /// <summary>
    /// Initializing the animation spinner
    /// </summary>
    /// <param name="entry">*Text near the spinner</param>
    /// <param name="leftBrackets">Left bracket from the spinner. Default = " "</param>
    /// <param name="rightBrackets">Right bracket from the spinner. Default = " "</param>
    /// <param name="delay">Animation delay rate (ms). Default = 700</param>
    /// <param name="position">Position spinner (Left / Right). Default = Left</param>
    public ConsoleSpinner(string entry, string leftBrackets = " ", string rightBrackets = " ", int delay = 700, PositionSpinner position = PositionSpinner.Left)
    {
        this.Text = entry;
        this.LeftBrackets = leftBrackets;
        this.RightBrackets = rightBrackets;
        this.Delay = delay;
        this.Position = position;
        this.IsLoading = true;

        Console.SetCursorPosition(GetPositionText(), _Console.CursorTop);
        _Console.Output.Write(Text);

        Console.SetCursorPosition(GetPositionSpinner(), _Console.CursorTop);
    }

    /// <summary>
    /// Starting the spinner animation
    /// </summary>
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