using System.Net;
using GunnyLauncher.Core;

static void Require(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

var redirect = new Uri("http://103.9.156.182/Gunny/Default.aspx?user=test%20user&key=abc-123&editby=Trminhpc");
var launch = GameLaunchInfo.ParseRedirect(redirect);
Require(launch.User == "test user", "decoded user mismatch");
Require(launch.Key == "abc-123", "key mismatch");
Require(launch.EditBy == "Trminhpc", "editby mismatch");

var swf = launch.BuildSwfUri(new Uri("http://103.9.156.182/Gunny/"));
Require(swf.AbsoluteUri.Contains("/Gunny/flash/Loading.swf?", StringComparison.Ordinal), "SWF path mismatch");
Require(swf.Query.Contains("user=test%20user", StringComparison.Ordinal), "user query missing");
Require(swf.Query.Contains("key=abc-123", StringComparison.Ordinal), "key query missing");
Require(swf.Query.Contains("config=http%3A%2F%2F103.9.156.182%2FGunny%2Fconfig.xml", StringComparison.OrdinalIgnoreCase), "config query missing");

var handler = new SequenceHandler();
var client = new GunnyLoginClient(new Uri("http://103.9.156.182/Gunny/"), handler);
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
var ruffleArgs = RuffleLaunchCommand.BuildArguments(launch, new Uri("http://103.9.156.182/Gunny/"));
var argLine = string.Join("|", ruffleArgs);
Require(argLine.Contains("--socket-allow|103.9.156.182:9200", StringComparison.Ordinal), "socket allowlist missing");
Require(argLine.Contains("--graphics|dx12", StringComparison.Ordinal), "DX12 graphics override missing");
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

var serviceHandler = new SequenceHandler();
var service = new GunnyLauncherService(new Uri("http://103.9.156.182/Gunny/"), runtimeRoot, serviceHandler);
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

sealed record CapturedRequest(HttpMethod Method, Uri Uri, string? Body);
