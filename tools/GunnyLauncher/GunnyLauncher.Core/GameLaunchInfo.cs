namespace GunnyLauncher.Core;

public sealed record GameLaunchInfo(string User, string Key, string EditBy)
{
    public static GameLaunchInfo ParseRedirect(Uri redirect)
    {
        ArgumentNullException.ThrowIfNull(redirect);
        var query = ParseQuery(redirect.Query);
        if (!query.TryGetValue("user", out var user) || string.IsNullOrWhiteSpace(user))
            throw new InvalidDataException("Login redirect is missing user.");
        if (!query.TryGetValue("key", out var key) || string.IsNullOrWhiteSpace(key))
            throw new InvalidDataException("Login redirect is missing key.");
        query.TryGetValue("editby", out var editBy);
        return new GameLaunchInfo(user, key, editBy ?? string.Empty);
    }

    public Uri BuildSwfUri(Uri gunnyBaseUri)
    {
        ArgumentNullException.ThrowIfNull(gunnyBaseUri);
        var swf = new Uri(gunnyBaseUri, "flash/Loading.swf");
        var config = new Uri(gunnyBaseUri, "config.xml");
        var query = "user=" + Uri.EscapeDataString(User)
            + "&key=" + Uri.EscapeDataString(Key)
            + "&config=" + Uri.EscapeDataString(config.AbsoluteUri);
        return new UriBuilder(swf) { Query = query }.Uri;
    }

    private static Dictionary<string, string> ParseQuery(string query)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var text = query.StartsWith('?') ? query[1..] : query;
        foreach (var pair in text.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var split = pair.IndexOf('=');
            var rawKey = split < 0 ? pair : pair[..split];
            var rawValue = split < 0 ? string.Empty : pair[(split + 1)..];
            var key = Decode(rawKey);
            var value = Decode(rawValue);
            result[key] = value;
        }
        return result;
    }

    private static string Decode(string value) =>
        Uri.UnescapeDataString(value.Replace('+', ' '));
}
