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

    public TogglSettings? TogglSettings { get; set; }
    public PersonioSettings? PersonioSettings { get; set; }
}

public class TogglSettings
{
    public const string DATE_TIME_FORMAT = "MM/dd/yyyy HH:mm:ss";

    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ApiToken { get; init; }
    public long DefaultWorkSpace { get; init; }
}

public class PersonioSettings
{
    public string? EmployeeName { get; init; }
    public string? ClientId { get; init; }
    public string? ClientSecret { get; init; }
    public int EmployeeId { get; init; }
}