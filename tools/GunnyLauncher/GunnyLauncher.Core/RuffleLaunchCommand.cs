namespace GunnyLauncher.Core;

public static class RuffleLaunchCommand
{
    public static IReadOnlyList<string> BuildArguments(GameLaunchInfo launch, Uri gameBase)
    {
        ArgumentNullException.ThrowIfNull(launch);
        ArgumentNullException.ThrowIfNull(gameBase);

        var flashBase = new Uri(gameBase, "flash/");
        var socketHost = gameBase.Host;
        var swf = launch.BuildSwfUri(gameBase);

        return new[]
        {
            "--graphics", "dx12",
            "--width", "1000",
            "--height", "600",
            "--base", flashBase.AbsoluteUri,
            "--socket-allow", $"{socketHost}:9200",
            "--tcp-connections", "deny",
            "--storage", "disk",
            "--open-url-mode", "confirm",
            $"-Peditby={launch.EditBy}",
            swf.AbsoluteUri
        };
    }
}
