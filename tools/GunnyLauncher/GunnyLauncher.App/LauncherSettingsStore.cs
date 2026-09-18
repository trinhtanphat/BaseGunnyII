using System.Text.Json;
using GunnyLauncher.Core;

namespace GunnyLauncher.App;

public sealed class LauncherSettingsStore
{
    private readonly string _path;
    private readonly LauncherSettings _defaultSettings;
    private readonly GunnyServerProfile _defaultServerProfile;

    public LauncherSettingsStore(string? path = null, string? profilePath = null)
    {
        profilePath ??= Path.Combine(AppContext.BaseDirectory, "launcher.profile.json");
        var profile = LauncherProfile.Load(profilePath);
        _defaultSettings = new LauncherSettings(profile.DefaultServerUrl, string.Empty);
        _defaultServerProfile = GunnyServerProfile.Resolve(new Uri(profile.DefaultServerUrl));

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
            var saved = JsonSerializer.Deserialize<LauncherSettings>(File.ReadAllText(_path));
            if (saved is null) return _defaultSettings;

            if (!IsServerCompatible(saved.ServerUrl))
                return new LauncherSettings(_defaultSettings.ServerUrl, saved.Username ?? string.Empty);

            return saved;
        }
        catch (JsonException)
        {
            return _defaultSettings;
        }
    }

    public bool IsServerCompatible(string? serverUrl)
    {
        if (!Uri.TryCreate(serverUrl, UriKind.Absolute, out var candidate)) return false;
        return GunnyServerProfile.Resolve(candidate).IsLegacyV30 == _defaultServerProfile.IsLegacyV30;
    }

    public void Save(LauncherSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        if (!IsServerCompatible(settings.ServerUrl))
            throw new InvalidOperationException("Server URL does not match the active launcher profile.");

        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        File.WriteAllText(_path, JsonSerializer.Serialize(settings));
    }
}
