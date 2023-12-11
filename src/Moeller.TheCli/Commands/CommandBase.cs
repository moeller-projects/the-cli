using CliFx.Infrastructure;
using Moeller.TheCli.Domain;
using Moeller.TheCli.Domain.Personio;
using Moeller.TheCli.Domain.Personio.Configuration;
using Moeller.TheCli.Infrastructure;
using Moeller.TheCli.Infrastructure.Extensions;
using Toggl;

namespace Moeller.TheCli.Commands;

public abstract class CommandBase(ConfigurationProvider provider)
{
    private TogglAsync? _TogglClient;
    private PersonioClient? _PersonioClient;

    protected readonly Settings Settings = provider.Get();

    protected async Task<TogglAsync> GetTogglClient(IConsole console)
    {
        if (string.IsNullOrWhiteSpace(Settings?.TogglSettings?.ApiToken))
        {
            await console.RespondWithFailureAsync("Toggl Connection not ready, please check your Api Token and feel free to re-init your cli");
            Environment.Exit(-1);
        }

        return _TogglClient ??= new TogglAsync(Settings?.TogglSettings?.ApiToken);
    }

    protected async Task<PersonioClient> GetPersonioClient(IConsole console)
    {
        if (string.IsNullOrWhiteSpace(Settings?.PersonioSettings?.ClientId) || string.IsNullOrWhiteSpace(Settings.PersonioSettings.ClientSecret))
        {
            await console.RespondWithFailureAsync("Personio Connection not ready, please check your Api Token and feel free to re-init your cli");
            Environment.Exit(-1);
        }

        if (_PersonioClient is not null)
            return _PersonioClient;
        
        _PersonioClient = new PersonioClient(new PersonioClientOptions(){ClientId = Settings.PersonioSettings.ClientId, ClientSecret = Settings.PersonioSettings.ClientSecret});
        await _PersonioClient.AuthAsync();
        if (!_PersonioClient.IsReady)
        {
            await console.RespondWithFailureAsync("Personio Connection not ready, please check your Api Token and feel free to re-init your cli");
            Environment.Exit(-1);
        }

        return _PersonioClient;
    }
}