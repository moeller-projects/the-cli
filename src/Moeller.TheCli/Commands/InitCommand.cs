using System.Net;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Moeller.TheCli.Domain;
using Moeller.TheCli.Domain.Personio;
using Moeller.TheCli.Domain.Personio.Configuration;
using Moeller.TheCli.Domain.Personio.Models.Request;
using Moeller.TheCli.Infrastructure;
using Moeller.TheCli.Infrastructure.Extensions;
using Sharprompt;
using TogglAPI.NetStandard.Api;
using TogglAPI.NetStandard.Client;
using IConsole = CliFx.Infrastructure.IConsole;
using Task = System.Threading.Tasks.Task;

namespace Moeller.TheCli.Commands;

[Command("init", Description = "initializes the application")]
public class InitCommand(ConfigurationProvider provider) : ICommand
{
    private readonly ConfigurationProvider _Provider = provider ?? throw new ArgumentNullException(nameof(provider));

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
            await console.RespondWithSuccessfulAsync();
        }
    }

    private static async ValueTask<PersonioSettings?> InitPersonioSettings(IConsole console)
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
        var username = Prompt.Input<string>("Please enter your Toggl Username");
        var password = Prompt.Password("Please enter your Toggl Password");
        
        var spi = new ConsoleSpinner(console, "Initialising TogglClient");
        Task.Run(() => spi.Turn());
        Configuration.Default.BasePath = "https://api.track.toggl.com/api/v9";
        Configuration.Default.Username = username?.Trim();
        Configuration.Default.Password = password?.Trim();
        var workspaceApi = new WorkspacesApi();
        spi.Stop();
        var workspaces = await workspaceApi.GetWorkspacesAsync();
        var currentWorkspace = Prompt.Select("Select your target WorkSpace", workspaces, textSelector: workspace => workspace.Name);
        var togglSettings = new TogglSettings()
        {
            Username = username?.Trim(),
            Password = password?.Trim(),
            DefaultWorkSpace = currentWorkspace.Id.GetValueOrDefault()
        };
        return togglSettings;
    }
}