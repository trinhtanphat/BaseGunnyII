namespace GunnyLauncher.Core;

public sealed record GunnyServerProfile(
    bool IsLegacyV30,
    string LoadingPath,
    string RuffleBasePath,
    int SocketPort)
{
    public static GunnyServerProfile Resolve(Uri gameBase)
    {
        ArgumentNullException.ThrowIfNull(gameBase);
        return gameBase.Port == 8083
            ? new GunnyServerProfile(true, "Loading.swf", string.Empty, 9300)
            : new GunnyServerProfile(false, "flash/Loading.swf", "flash/", 9200);
    }

    public Uri BuildRegisterBase(Uri gameBase)
    {
        ArgumentNullException.ThrowIfNull(gameBase);
        if (!IsLegacyV30) return EnsureTrailingSlash(gameBase);
        var builder = new UriBuilder(gameBase)
        {
            Path = "/Register/",
            Query = string.Empty,
            Fragment = string.Empty
        };
        return builder.Uri;
    }

    private static Uri EnsureTrailingSlash(Uri value) =>
        value.AbsoluteUri.EndsWith('/') ? value : new Uri(value.AbsoluteUri + "/");
}
