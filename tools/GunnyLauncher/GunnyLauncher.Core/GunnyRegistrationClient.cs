using System.Net;

namespace GunnyLauncher.Core;

public sealed class GunnyRegistrationClient
{
    private readonly Uri _baseUri;
    private readonly HttpClient _client;

    public GunnyRegistrationClient(Uri baseUri, HttpMessageHandler? handler = null)
    {
        ArgumentNullException.ThrowIfNull(baseUri);
        _baseUri = EnsureTrailingSlash(baseUri);
        _client = new HttpClient(handler ?? CreateHandler(), disposeHandler: handler is null)
        {
            Timeout = TimeSpan.FromSeconds(20)
        };
    }

    public async Task<RegistrationCaptcha> GetCaptchaAsync(CancellationToken cancellationToken)
    {
        using var response = await _client.GetAsync(
            new Uri(_baseUri, GunnyProtocolContract.CaptchaEndpoint),
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        return new RegistrationCaptcha(bytes, contentType);
    }

    public async Task<RegistrationResult> RegisterAsync(
        string username,
        string password,
        string confirmation,
        string email,
        string sex,
        string captchaCode,
        CancellationToken cancellationToken)
    {
        using var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["username"] = username,
            ["password"] = password,
            ["repassword"] = confirmation,
            ["email"] = email,
            ["sex"] = sex,
            ["code"] = captchaCode
        });

        using var response = await _client.PostAsync(
            new Uri(_baseUri, GunnyProtocolContract.RegisterEndpoint),
            form,
            cancellationToken);
        response.EnsureSuccessStatusCode();
        var text = (await response.Content.ReadAsStringAsync(cancellationToken)).Trim();
        return string.Equals(text, "ok", StringComparison.OrdinalIgnoreCase)
            ? new RegistrationResult(true, "ok")
            : new RegistrationResult(false, text);
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
