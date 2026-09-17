using System.Net;
using System.Security.Cryptography;
using System.Text;

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

        var profile = GunnyServerProfile.Resolve(_baseUri);
        return profile.IsLegacyV30
            ? await AuthenticateV30Async(client, username, password, cancellationToken)
            : await AuthenticateV389Async(client, username, password, cancellationToken);
    }

    private async Task<GameLaunchInfo> AuthenticateV389Async(
        HttpClient client,
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        using var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["username"] = username,
            ["password"] = password
        });
        using var loginResponse = await client.PostAsync(new Uri(_baseUri, GunnyProtocolContract.LoginEndpoint), form, cancellationToken);
        loginResponse.EnsureSuccessStatusCode();
        var loginText = (await loginResponse.Content.ReadAsStringAsync(cancellationToken)).Trim();
        if (!string.Equals(loginText, "ok", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Username or password was rejected by the Gunny server.");

        var loginGameUri = new Uri(_baseUri, GunnyProtocolContract.LoginGameEndpoint);
        using var gameResponse = await client.GetAsync(loginGameUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        return ParseGameRedirect(gameResponse, loginGameUri);
    }

    private async Task<GameLaunchInfo> AuthenticateV30Async(
        HttpClient client,
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        using var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["txtUserName"] = username,
            ["txtPassword"] = Md5Lower(password),
            ["txtSite"] = string.Empty
        });
        var loginGameUri = new Uri(_baseUri, GunnyProtocolContract.LoginGameEndpoint);
        using var response = await client.PostAsync(loginGameUri, form, cancellationToken);
        if ((int)response.StatusCode >= 300 && (int)response.StatusCode <= 399 && response.Headers.Location is not null)
            return ParseGameRedirect(response, loginGameUri);

        var body = (await response.Content.ReadAsStringAsync(cancellationToken)).Trim();
        response.EnsureSuccessStatusCode();
        var message = string.IsNullOrWhiteSpace(body) ? "Gunny 3.0 rejected the login." : body;
        throw new InvalidOperationException(message.Length > 300 ? message[..300] : message);
    }

    private static GameLaunchInfo ParseGameRedirect(HttpResponseMessage response, Uri requestUri)
    {
        if ((int)response.StatusCode < 300 || (int)response.StatusCode > 399 || response.Headers.Location is null)
            throw new InvalidDataException("LoginGame did not return the expected game redirect.");
        var location = response.Headers.Location.IsAbsoluteUri
            ? response.Headers.Location
            : new Uri(requestUri, response.Headers.Location);
        return GameLaunchInfo.ParseRedirect(location);
    }

    private static string Md5Lower(string value) =>
        Convert.ToHexString(MD5.HashData(Encoding.ASCII.GetBytes(value))).ToLowerInvariant();

    private static HttpClientHandler CreateHandler() => new()
    {
        AllowAutoRedirect = false,
        CookieContainer = new CookieContainer(),
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    };

    private static Uri EnsureTrailingSlash(Uri value) =>
        value.AbsoluteUri.EndsWith('/') ? value : new Uri(value.AbsoluteUri + "/");
}
