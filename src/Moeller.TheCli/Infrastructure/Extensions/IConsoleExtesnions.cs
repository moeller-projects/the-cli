using CliFx.Infrastructure;
using Sharprompt;

namespace Moeller.TheCli.Infrastructure.Extensions;

public static class IConsoleExtesnions
{
    public static Task RespondWithSuccessfulAsync(this IConsole console)
    {
        using (console.WithForegroundColor(ConsoleColor.Green))
        {
            return console.Output.WriteLineAsync($"{Prompt.Symbols.Done} Successful!");
        }
    }
    
    public static Task RespondWithFailureAsync(this IConsole console, string message, Exception? exception = null)
    {
        using (console.WithForegroundColor(ConsoleColor.Red))
        {
            return console.Output.WriteLineAsync($"{Prompt.Symbols.Error} Error: {message}");
        }
    }
}