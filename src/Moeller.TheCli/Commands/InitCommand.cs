using System.Net;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Moeller.TheCli.Domain;
using Moeller.TheCli.Domain.Personio;
using Moeller.TheCli.Domain.Personio.Configuration;
using Moeller.TheCli.Domain.Personio.Models.Request;
using Moeller.TheCli.Infrastructure;
using Sharprompt;
using Toggl;
using IConsole = CliFx.Infrastructure.IConsole;

namespace Moeller.TheCli.Commands;

[Command("init", Description = "inits the app")]
public class InitCommand : ICommand
{
    private readonly ConfigurationProvider _Provider;

    public InitCommand(ConfigurationProvider provider)
    {
        _Provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var config = _Provider.Get();
        if (config is not null)
        {
            if (!Prompt.Confirm("Tool is already configured. Do you really wanna overwrite it?"))
                return;
        }
        
        var togglSettings = await InitTogglSettings(console);
        var personioSettings = await InitPersonioSettings(console);

        var settings = new Settings(togglSettings, personioSettings);
        
        await _Provider.SetAsync(settings);
        using (console.WithForegroundColor(ConsoleColor.Green))
        {
            await console.Output.WriteLineAsync("> Successful! <");
        }
    }

    private async ValueTask<PersonioSettings?> InitPersonioSettings(IConsole console)
    {
        var clientId = Prompt.Password("Please enter your Personio ClientId");
        var clientSecret = Prompt.Password("Please enter your Personio ClientSecret");
        var personioClient = new PersonioClient(new PersonioClientOptions()
        {
            ClientId = clientId,
            ClientSecret = clientSecret
        });
        var authToken = await personioClient.AuthAsync();
        if (string.IsNullOrWhiteSpace(authToken))
        {
            throw new Exception("ClientCredentials are invalid!");
        }

        var email = Prompt.Input<string>("Please enter your Personio Email");
        var users = await personioClient.GetEmployeesAsync(new GetEmployeesRequest() {Email = email});
        if (users.StatusCode != HttpStatusCode.OK || users.PagedList.TotalElements == 0)
        {
            await console.Output.WriteLineAsync($"no user found for email {email}");
        }

        var selectedUser = Prompt.Select("Select your Employee", users.PagedList.Data);
        return new PersonioSettings()
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            EmployeeId = selectedUser.Id.Value
        };
    }

    private static async Task<TogglSettings?> InitTogglSettings(IConsole console)
    {
        var token = Prompt.Password("Please enter your Toggl API Token");
        var togglClient = new TogglAsync(token);

        var user = await togglClient.User.GetCurrent();
        await console.Output.WriteLineAsync($"Hello {user.FullName}");

        var workspaces = await togglClient.Workspace.List();
        var currentWorkspace = Prompt.Select("Select your target WorkSpace", workspaces);
        var togglSettings = new TogglSettings()
        {
            ApiToken = token,
            DefaultWorkSpace = currentWorkspace.Id.GetValueOrDefault()
        };
        return togglSettings;
    }
}