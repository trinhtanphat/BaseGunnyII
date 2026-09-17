using System.Net;
using System.Text.RegularExpressions;

namespace GunnyLauncher.Core;

public sealed class GunnyRegistrationClient
{
    private readonly Uri _baseUri;
    private readonly GunnyServerProfile _profile;
    private readonly HttpClient _client;

    public GunnyRegistrationClient(Uri baseUri, HttpMessageHandler? handler = null)
    {
        ArgumentNullException.ThrowIfNull(baseUri);
        _baseUri = EnsureTrailingSlash(baseUri);
        _profile = GunnyServerProfile.Resolve(_baseUri);
        _client = new HttpClient(handler ?? CreateHandler(), disposeHandler: handler is null)
        {
            Timeout = TimeSpan.FromSeconds(20)
        };
    }

    public async Task<RegistrationCaptcha> GetCaptchaAsync(CancellationToken cancellationToken)
    {
        var uri = _profile.IsLegacyV30
            ? new Uri(_profile.BuildRegisterBase(_baseUri), "Handler.ashx")
            : new Uri(_baseUri, GunnyProtocolContract.CaptchaEndpoint);
        using var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        return new RegistrationCaptcha(bytes, contentType);
    }

    public Task<RegistrationResult> RegisterAsync(
        string username,
        string password,
        string confirmation,
        string email,
        string sex,
        string captchaCode,
        CancellationToken cancellationToken) =>
        _profile.IsLegacyV30
            ? RegisterV30Async(username, password, confirmation, email, sex, captchaCode, cancellationToken)
            : RegisterV389Async(username, password, confirmation, email, sex, captchaCode, cancellationToken);

    private async Task<RegistrationResult> RegisterV389Async(
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
        using var response = await _client.PostAsync(new Uri(_baseUri, GunnyProtocolContract.RegisterEndpoint), form, cancellationToken);
        response.EnsureSuccessStatusCode();
        var text = (await response.Content.ReadAsStringAsync(cancellationToken)).Trim();
        return string.Equals(text, "ok", StringComparison.OrdinalIgnoreCase)
            ? new RegistrationResult(true, "ok")
            : new RegistrationResult(false, text);
    }

    private async Task<RegistrationResult> RegisterV30Async(
        string username,
        string password,
        string confirmation,
        string email,
        string sex,
        string captchaCode,
        CancellationToken cancellationToken)
    {
        var registerBase = _profile.BuildRegisterBase(_baseUri);
        var pageUri = new Uri(registerBase, "Default.aspx");
        using var pageResponse = await _client.GetAsync(pageUri, cancellationToken);
        pageResponse.EnsureSuccessStatusCode();
        var page = await pageResponse.Content.ReadAsStringAsync(cancellationToken);

        var fields = new Dictionary<string, string>
        {
            ["__VIEWSTATE"] = Hidden(page, "__VIEWSTATE"),
            ["TxtName"] = username,
            ["TxtCharName"] = username,
            ["TxtPassword"] = password,
            ["TxtRePassword"] = confirmation,
            ["TxtEmail"] = email,
            ["TxtReEmail"] = email,
            ["DropDownList1"] = string.Equals(sex, "1", StringComparison.Ordinal) ? "0" : "1",
            ["TextCode"] = captchaCode,
            ["Button1.x"] = "1",
            ["Button1.y"] = "1"
        };
        AddHiddenIfPresent(fields, page, "__VIEWSTATEGENERATOR");
        AddHiddenIfPresent(fields, page, "__EVENTVALIDATION");

        using var form = new FormUrlEncodedContent(fields);
        using var response = await _client.PostAsync(pageUri, form, cancellationToken);
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync(cancellationToken);
        if (html.Contains("color='Green'", StringComparison.OrdinalIgnoreCase)
            || html.Contains("color=\"Green\"", StringComparison.OrdinalIgnoreCase))
            return new RegistrationResult(true, "ok");

        var label = ExtractLabel(html);
        return new RegistrationResult(false,
            string.IsNullOrWhiteSpace(label) ? "Gunny 3.0 registration was rejected." : label);
    }

    private static string Hidden(string html, string name)
    {
        var value = TryHidden(html, name);
        if (value is null) throw new InvalidDataException($"Gunny 3.0 register page is missing {name}.");
        return value;
    }

    private static string? TryHidden(string html, string name)
    {
var pattern = "<input\\b[^>]*\\bname\\s*=\\s*[\"']" + Regex.Escape(name) + "[\"'][^>]*\\bvalue\\s*=\\s*[\"'](?<v>[^\"']*)[\"'][^>]*>";
        var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        return match.Success ? WebUtility.HtmlDecode(match.Groups["v"].Value) : null;
    }

    private static void AddHiddenIfPresent(Dictionary<string, string> fields, string html, string name)
    {
        var value = TryHidden(html, name);
        if (value is not null) fields[name] = value;
    }

    private static string ExtractLabel(string html)
    {
var match = Regex.Match(html, "<span[^>]*id=[\"']Label1[\"'][^>]*>(?<v>.*?)</span>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (!match.Success) return string.Empty;
        var text = Regex.Replace(match.Groups["v"].Value, "<[^>]+>", " ");
        return WebUtility.HtmlDecode(Regex.Replace(text, @"\s+", " ")).Trim();
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
