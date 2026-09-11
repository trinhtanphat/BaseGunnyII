namespace GunnyLauncher.App;

public sealed record LauncherSettings(string ServerUrl, string Username)
{
    public static LauncherSettings Default { get; } =
        new("http://103.9.156.182/Gunny/", string.Empty);
}
