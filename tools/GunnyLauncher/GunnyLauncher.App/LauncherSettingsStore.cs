using System.Text.Json;
using GunnyLauncher.Core;

namespace GunnyLauncher.App;

public sealed class LauncherSettingsStore
{
    private readonly string _path;
    private readonly LauncherSettings _defaultSettings;

    public LauncherSettingsStore(string? path = null, string? profilePath = null)
    {
        profilePath ??= Path.Combine(AppContext.BaseDirectory, "launcher.profile.json");
        var profile = LauncherProfile.Load(profilePath);
        _defaultSettings = new LauncherSettings(profile.DefaultServerUrl, string.Empty);

        _path = !string.IsNullOrWhiteSpace(path)
            ? path
            : Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BaseGunnyII",
                "profiles",
                profile.Profile,
                "launcher.json");
    }

    public LauncherSettings Load()
    {
        if (!File.Exists(_path)) return _defaultSettings;
        try
        {
            return JsonSerializer.Deserialize<LauncherSettings>(File.ReadAllText(_path))
                ?? _defaultSettings;
        }
        catch (JsonException)
        {
            return _defaultSettings;
        }
    }

    public void Save(LauncherSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        File.WriteAllText(_path, JsonSerializer.Serialize(settings));
    }
}
