using CliFx.Infrastructure;
using Sharprompt;

namespace Moeller.TheCli.Infrastructure.Extensions;

public static class IConsoleExtesnions
{
    public static Task RespondWithSuccessfulAsync(this IConsole console)
    {
        using (console.WithForegroundColor(ConsoleColor.Green))
        {
            return console.Output.WriteLineAsync(Prompt.Symbols.Done.ToString() + " Successful!");
        }
    }
}