namespace GunnyLauncher.Core;

public static class RuffleLaunchCommand
{
    public static IReadOnlyList<string> BuildArguments(GameLaunchInfo launch, Uri gameBase)
    {
        ArgumentNullException.ThrowIfNull(launch);
        ArgumentNullException.ThrowIfNull(gameBase);

        var profile = GunnyServerProfile.Resolve(gameBase);
        var ruffleBase = new Uri(gameBase, profile.RuffleBasePath);
        var socketHost = gameBase.Host;
        var swf = launch.BuildSwfUri(gameBase);
        var saveDirectory = BuildSaveDirectory(profile);

        return new[]
        {
            "--graphics", "gl",
            "--no-avm2-optimizer",
            "--width", "1000",
            "--height", "600",
            "--base", ruffleBase.AbsoluteUri,
            "--socket-allow", $"{socketHost}:{profile.SocketPort}",
            "--tcp-connections", "allow",
            "--storage", "disk",
            "--save-directory", saveDirectory,
            "--open-url-mode", "confirm",
            $"-Peditby={launch.EditBy}",
            swf.AbsoluteUri
        };
    }

    private static string BuildSaveDirectory(GunnyServerProfile profile)
    {
        var profileName = profile.IsLegacyV30 ? "v30" : "v389";
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BaseGunnyII",
            "Ruffle",
            profileName,
            "SharedObjects");
    }
}
