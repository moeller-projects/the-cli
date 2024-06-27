using CliFx.Infrastructure;
using Moeller.TheCli.Domain;
using Moeller.TheCli.Domain.Personio;
using Moeller.TheCli.Domain.Personio.Configuration;
using Moeller.TheCli.Infrastructure;
using Moeller.TheCli.Infrastructure.Extensions;
using TogglAPI.NetStandard.Client;

namespace Moeller.TheCli.Commands;

public abstract class CommandBase(ConfigurationProvider provider)
{
    private PersonioClient? _PersonioClient;

    protected readonly Settings Settings = provider.Get();

    protected async Task SetupTogglClient(IConsole console)
    {
        if (
            string.IsNullOrWhiteSpace(Settings?.TogglSettings?.Username)
            || string.IsNullOrWhiteSpace(Settings?.TogglSettings?.Password)
            )
        {
            await console.RespondWithFailureAsync("Toggl Username/Password doesn't seem to be deposited, please run `the init` again");
            Environment.Exit(-1);
        }
        
        Configuration.Default.BasePath = "https://api.track.toggl.com/api/v9";
        Configuration.Default.Username = Settings?.TogglSettings?.Username;
        Configuration.Default.Password = Settings?.TogglSettings?.Password;
    }

    protected async Task<PersonioClient> GetPersonioClient(IConsole console)
    {
        if (string.IsNullOrWhiteSpace(Settings?.PersonioSettings?.ClientId) || string.IsNullOrWhiteSpace(Settings.PersonioSettings.ClientSecret))
        {
            await console.RespondWithFailureAsync("Personio Credentials doesn't seem to be deposited, please run `the init` again");
            Environment.Exit(-1);
        }

        if (_PersonioClient is not null)
            return _PersonioClient;
        
        _PersonioClient = new PersonioClient(new PersonioClientOptions(){ClientId = Settings.PersonioSettings.ClientId, ClientSecret = Settings.PersonioSettings.ClientSecret});
        await _PersonioClient.AuthAsync();
        if (!_PersonioClient.IsReady)
        {
            await console.RespondWithFailureAsync("Personio Credentials seem to be invalid, please run `the init` again");
            Environment.Exit(-1);
        }

        return _PersonioClient;
    }
}