using System.Diagnostics;
using JsonFlatFileDataStore;
using Moeller.TheCli.Infrastructure;

namespace Moeller.TheCli.Domain;

public class ConfigurationProvider
{
    private readonly IDataStore _Store;

    public ConfigurationProvider(IDataStore store)
    {
        _Store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public Settings Get()
    {
        try
        {
            return _Store.GetItem<Settings>(nameof(Settings));
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return null;
        }
    }

    public Task<bool> SetAsync(Settings settings)
    {
        return _Store.ReplaceItemAsync(nameof(Settings), settings, true);
    }
}