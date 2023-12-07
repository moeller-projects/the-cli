using CliFx;
using JsonFlatFileDataStore;
using Microsoft.Extensions.DependencyInjection;
using Moeller.TheCli.Domain;

namespace Moeller.TheCli;
public static class Program
{
    public static async Task<int> Main(string[] args) =>
        await new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .UseTypeActivator(commandTypes =>
            {
                var configPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "MOELLER/THE_CLI");

                if (!Directory.Exists(configPath))
                    Directory.CreateDirectory(configPath);

                var services = new ServiceCollection();
                services.AddSingleton<IDataStore>(provider => new DataStore(Path.Combine(configPath, "config.json")));
                services.AddSingleton<ConfigurationProvider>();
                
                foreach (var commandType in commandTypes)
                    services.AddTransient(commandType);

                return services.BuildServiceProvider();
            })
            .Build()
            .RunAsync();
}