using System.Text.Json;

namespace GunnyLauncher.App;

public sealed class LauncherSettingsStore
{
    private readonly string _path;

    public LauncherSettingsStore(string? path = null)
    {
        _path = path ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BaseGunnyII",
            "launcher.json");
    }

    public LauncherSettings Load()
    {
        if (!File.Exists(_path)) return LauncherSettings.Default;
        try
        {
            return JsonSerializer.Deserialize<LauncherSettings>(File.ReadAllText(_path))
                ?? LauncherSettings.Default;
        }
        catch (JsonException)
        {
            return LauncherSettings.Default;
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
