using System.Net;

namespace GunnyLauncher.Core;

public sealed class GunnyLoginClient
{
    private readonly Uri _baseUri;
    private readonly HttpMessageHandler? _handler;

    public GunnyLoginClient(Uri baseUri, HttpMessageHandler? handler = null)
    {
        ArgumentNullException.ThrowIfNull(baseUri);
        _baseUri = EnsureTrailingSlash(baseUri);
        _handler = handler;
    }

    public async Task<GameLaunchInfo> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username is required.", nameof(username));
        if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password is required.", nameof(password));

        var ownedHandler = _handler is null ? CreateHandler() : null;
        using var client = new HttpClient(_handler ?? ownedHandler!, disposeHandler: ownedHandler is not null)
        {
            Timeout = TimeSpan.FromSeconds(20)
        };
        using var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["username"] = username,
            ["password"] = password
        });
        using var loginResponse = await client.PostAsync(new Uri(_baseUri, "createLogin.ashx"), form, cancellationToken);
        loginResponse.EnsureSuccessStatusCode();
        var loginText = (await loginResponse.Content.ReadAsStringAsync(cancellationToken)).Trim();
        if (!string.Equals(loginText, "ok", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Username or password was rejected by the Gunny server.");

        var loginGameUri = new Uri(_baseUri, "LoginGame.aspx");
        using var gameResponse = await client.GetAsync(loginGameUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if ((int)gameResponse.StatusCode < 300 || (int)gameResponse.StatusCode > 399 || gameResponse.Headers.Location is null)
            throw new InvalidDataException("LoginGame did not return the expected game redirect.");

        var location = gameResponse.Headers.Location.IsAbsoluteUri
            ? gameResponse.Headers.Location
            : new Uri(loginGameUri, gameResponse.Headers.Location);
        return GameLaunchInfo.ParseRedirect(location);
    }

    private static HttpClientHandler CreateHandler() => new()
    {
        AllowAutoRedirect = false,
        CookieContainer = new CookieContainer(),
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    };

    private static Uri EnsureTrailingSlash(Uri value) =>
        value.AbsoluteUri.EndsWith('/') ? value : new Uri(value.AbsoluteUri + "/");
}
