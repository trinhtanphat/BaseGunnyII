namespace GunnyLauncher.Core;

public sealed record RuffleAndroidLaunchDescriptor(
    string Action,
    string MimeType,
    string PackageName,
    Uri GameUri);

public static class RuffleAndroidContract
{
    public const string PackageName = "rs.ruffle";
    public const string ActionView = "android.intent.action.VIEW";
    public const string FlashMimeType = "application/x-shockwave-flash";

    public static Uri BuildGameUri(GameLaunchInfo launch, Uri gameBase)
    {
        ArgumentNullException.ThrowIfNull(launch);
        ArgumentNullException.ThrowIfNull(gameBase);
        return launch.BuildSwfUri(gameBase);
    }

    public static RuffleAndroidLaunchDescriptor CreateLaunchDescriptor(GameLaunchInfo launch, Uri gameBase) =>
        new(ActionView, FlashMimeType, PackageName, BuildGameUri(launch, gameBase));
}
