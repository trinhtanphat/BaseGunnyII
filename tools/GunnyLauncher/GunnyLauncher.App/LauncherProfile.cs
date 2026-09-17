using System.Text.Json;

namespace GunnyLauncher.App;

public sealed record LauncherProfile(string Profile, string DefaultServerUrl)
{
    public static LauncherProfile Default { get; } =
        new("v389", LauncherSettings.Default.ServerUrl);

    public static LauncherProfile Load(string path)
    {
        if (!File.Exists(path)) return Default;
        try
        {
            var profile = JsonSerializer.Deserialize<LauncherProfile>(File.ReadAllText(path));
            if (profile is null || string.IsNullOrWhiteSpace(profile.Profile)) return Default;
            if (!Uri.TryCreate(profile.DefaultServerUrl, UriKind.Absolute, out _)) return Default;
            return profile with { Profile = Sanitize(profile.Profile) };
        }
        catch (JsonException)
        {
            return Default;
        }
    }

    private static string Sanitize(string value)
    {
        var chars = value.Trim().Select(c => char.IsLetterOrDigit(c) || c is '-' or '_' ? c : '_').ToArray();
        return chars.Length == 0 ? Default.Profile : new string(chars);
    }
}
