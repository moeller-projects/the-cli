namespace Moeller.TheCli.Infrastructure;

public class Settings
{
    public Settings()
    {
        
    }

    public Settings(TogglSettings? togglSettings, PersonioSettings? personioSettings)
    {
        TogglSettings = togglSettings;
        PersonioSettings = personioSettings;
    }

    public TogglSettings? TogglSettings { get; }
    public PersonioSettings? PersonioSettings { get; }
}

public class TogglSettings
{
    public string? ApiToken { get; init; }
    public int DefaultWorkSpace { get; init; }
}

public class PersonioSettings
{
    public string? EmployeeName { get; init; }
    public string? ClientId { get; init; }
    public string? ClientSecret { get; init; }
    public int EmployeeId { get; init; }
}