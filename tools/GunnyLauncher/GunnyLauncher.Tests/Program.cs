using System.Net;
using GunnyLauncher.Core;

static void Require(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

var gameBase = new Uri("http://103.9.156.182/Gunny/");
Require(GunnyProtocolContract.LoginEndpoint == "createLogin.ashx", "login endpoint contract mismatch");
Require(GunnyProtocolContract.LoginGameEndpoint == "LoginGame.aspx", "login game endpoint contract mismatch");
Require(GunnyProtocolContract.CaptchaEndpoint == "auth/ValidateCode.aspx", "captcha endpoint contract mismatch");
Require(GunnyProtocolContract.RegisterEndpoint == "auth/register.ashx", "register endpoint contract mismatch");
Require(GunnyProtocolContract.RegistrationFields.SequenceEqual(new[] { "username", "password", "repassword", "email", "sex", "code" }), "registration fields contract mismatch");
var redirect = new Uri("http://103.9.156.182/Gunny/Default.aspx?user=test%20user&key=abc-123&editby=Trminhpc");
var launch = GameLaunchInfo.ParseRedirect(redirect);
Require(launch.User == "test user", "decoded user mismatch");
Require(launch.Key == "abc-123", "key mismatch");
Require(launch.EditBy == "Trminhpc", "editby mismatch");

var swf = launch.BuildSwfUri(gameBase);
Require(swf.AbsoluteUri.Contains("/Gunny/flash/Loading.swf?", StringComparison.Ordinal), "SWF path mismatch");
Require(swf.Query.Contains("user=test%20user", StringComparison.Ordinal), "user query missing");
Require(swf.Query.Contains("key=abc-123", StringComparison.Ordinal), "key query missing");
Require(swf.Query.Contains("config=http%3A%2F%2F103.9.156.182%2FGunny%2Fconfig.xml", StringComparison.OrdinalIgnoreCase), "config query missing");

var handler = new SequenceHandler();
var client = new GunnyLoginClient(gameBase, handler);
var authenticated = await client.AuthenticateAsync("test user", "secret pass", CancellationToken.None);
Require(authenticated.User == "test user", "authenticated user mismatch");
Require(authenticated.Key == "server-guid", "authenticated key mismatch");
Require(handler.Requests.Count == 2, "expected exactly two HTTP requests");
Require(handler.Requests[0].Method == HttpMethod.Post, "first request must be POST");
Require(handler.Requests[0].Uri.AbsolutePath.EndsWith("/Gunny/createLogin.ashx", StringComparison.OrdinalIgnoreCase), "login endpoint mismatch");
var posted = handler.Requests[0].Body ?? string.Empty;
Require(posted.Contains("username=test+user", StringComparison.Ordinal), "username form value missing");
Require(posted.Contains("password=secret+pass", StringComparison.Ordinal), "password form value missing");
Require(handler.Requests[1].Method == HttpMethod.Get, "second request must be GET");
Require(handler.Requests[1].Uri.AbsolutePath.EndsWith("/Gunny/LoginGame.aspx", StringComparison.OrdinalIgnoreCase), "LoginGame endpoint mismatch");

var registrationHandler = new RegistrationSequenceHandler();
var registration = new GunnyRegistrationClient(gameBase, registrationHandler);
var captcha = await registration.GetCaptchaAsync(CancellationToken.None);
Require(captcha.ImageBytes.SequenceEqual(new byte[] { 1, 2, 3 }), "captcha bytes mismatch");
Require(captcha.ContentType == "image/png", "captcha content type mismatch");
var registrationResult = await registration.RegisterAsync("newuser", "test123", "test123", "u@example.com", "1", "A1B2", CancellationToken.None);
Require(registrationResult.Success, "registration should accept ok");
Require(registrationHandler.Requests.Count == 2, "registration must use captcha then register sequence");
Require(registrationHandler.Requests[0].Method == HttpMethod.Get, "captcha request must be GET");
Require(registrationHandler.Requests[0].Uri.AbsolutePath.EndsWith("/Gunny/auth/ValidateCode.aspx", StringComparison.OrdinalIgnoreCase), "captcha endpoint mismatch");
Require(registrationHandler.Requests[1].Method == HttpMethod.Post, "registration request must be POST");
Require(registrationHandler.Requests[1].Uri.AbsolutePath.EndsWith("/Gunny/auth/register.ashx", StringComparison.OrdinalIgnoreCase), "registration endpoint mismatch");
var registrationPost = registrationHandler.Requests[1].Body ?? string.Empty;
Require(registrationPost.Contains("username=newuser", StringComparison.Ordinal), "registration username missing");
Require(registrationPost.Contains("password=test123", StringComparison.Ordinal), "registration password form value missing");
Require(registrationPost.Contains("repassword=test123", StringComparison.Ordinal), "registration confirmation missing");
Require(registrationPost.Contains("email=u%40example.com", StringComparison.Ordinal), "registration email missing");
Require(registrationPost.Contains("sex=1", StringComparison.Ordinal), "registration sex missing");
Require(registrationPost.Contains("code=A1B2", StringComparison.Ordinal), "registration captcha missing");
Require(!registrationPost.Contains("validateCode=", StringComparison.Ordinal), "legacy register endpoint must receive code, not validateCode");
Console.WriteLine("GUNNY_REGISTER_SMOKE=PASS");

var ruffleArgs = RuffleLaunchCommand.BuildArguments(launch, gameBase);
var argLine = string.Join("|", ruffleArgs);
Require(argLine.Contains("--socket-allow|103.9.156.182:9200", StringComparison.Ordinal), "socket allowlist missing");
Require(argLine.Contains("--tcp-connections|deny", StringComparison.Ordinal), "default TCP deny missing");
Require(argLine.Contains("--base|http://103.9.156.182/Gunny/flash/", StringComparison.OrdinalIgnoreCase), "Ruffle base missing");
Require(argLine.Contains("-Peditby=Trminhpc", StringComparison.Ordinal), "editby flashvar missing");
Require(ruffleArgs[^1].Contains("Loading.swf?", StringComparison.Ordinal), "SWF URL must be final argument");

var runtimeRoot = Path.Combine(Path.GetTempPath(), "GunnyLauncherSmoke");
var startInfo = RuffleProcessCommand.BuildStartInfo(runtimeRoot, ruffleArgs);
Require(startInfo.FileName.EndsWith(Path.Combine("runtime", "ruffle.exe"), StringComparison.OrdinalIgnoreCase), "Ruffle executable path mismatch");
Require(!startInfo.UseShellExecute, "Ruffle must not launch through shell");
Require(startInfo.ArgumentList.Count == ruffleArgs.Count, "Ruffle argument count mismatch");
Require(startInfo.ArgumentList[^1] == ruffleArgs[^1], "SWF must remain final process argument");

var metadataHandler = new SequenceHandler();
var metadataService = new GunnyLauncherService(gameBase, runtimeRoot, metadataHandler);
var launchOnly = await metadataService.AuthenticateAsync("test user", "secret pass", CancellationToken.None);
Require(launchOnly.Key == "server-guid", "metadata auth key mismatch");

var serviceHandler = new SequenceHandler();
var service = new GunnyLauncherService(gameBase, runtimeRoot, serviceHandler);
var serviceInfo = await service.BuildStartInfoAsync("test user", "secret pass", CancellationToken.None);
Require(serviceInfo.FileName.EndsWith(Path.Combine("runtime", "ruffle.exe"), StringComparison.OrdinalIgnoreCase), "service runtime path mismatch");
Require(serviceInfo.ArgumentList[^1].Contains("user=test%20user", StringComparison.Ordinal), "service SWF user missing");
Require(serviceHandler.Requests.Count == 2, "service must use complete two-step login flow");

Console.WriteLine("GUNNY_LOGIN_SMOKE=PASS");

sealed class SequenceHandler : HttpMessageHandler
{
    public List<CapturedRequest> Requests { get; } = new();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
        Requests.Add(new CapturedRequest(request.Method, request.RequestUri!, body));
        if (Requests.Count == 1)
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") };
        if (Requests.Count == 2)
        {
            var response = new HttpResponseMessage(HttpStatusCode.Redirect);
            response.Headers.Location = new Uri("http://103.9.156.182/Gunny/Default.aspx?user=test%20user&key=server-guid&editby=Trminhpc");
            return response;
        }
        throw new InvalidOperationException("Unexpected HTTP request.");
    }
}

sealed class RegistrationSequenceHandler : HttpMessageHandler
{
    public List<CapturedRequest> Requests { get; } = new();

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var body = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);
        Requests.Add(new CapturedRequest(request.Method, request.RequestUri!, body));

        if (Requests.Count == 1)
        {
            var content = new ByteArrayContent(new byte[] { 1, 2, 3 });
            content.Headers.ContentType = new("image/png");
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
        }

        if (Requests.Count == 2)
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") };

        throw new InvalidOperationException("Unexpected registration HTTP request.");
    }
}

sealed record CapturedRequest(HttpMethod Method, Uri Uri, string? Body);
