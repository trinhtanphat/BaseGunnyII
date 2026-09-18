using System.Net;
using GunnyLauncher.Core;

static void Require(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

var gameBase = new Uri("http://103.9.156.181/Gunny/");
Require(GunnyProtocolContract.LoginEndpoint == "createLogin.ashx", "login endpoint contract mismatch");
Require(GunnyProtocolContract.LoginGameEndpoint == "LoginGame.aspx", "login game endpoint contract mismatch");
Require(GunnyProtocolContract.CaptchaEndpoint == "auth/ValidateCode.aspx", "captcha endpoint contract mismatch");
Require(GunnyProtocolContract.RegisterEndpoint == "auth/register.ashx", "register endpoint contract mismatch");
Require(GunnyProtocolContract.RegistrationFields.SequenceEqual(new[] { "username", "password", "repassword", "email", "sex", "code" }), "registration fields contract mismatch");
var redirect = new Uri("http://103.9.156.181/Gunny/Default.aspx?user=test%20user&key=abc-123&editby=Trminhpc");
var launch = GameLaunchInfo.ParseRedirect(redirect);
Require(launch.User == "test user", "decoded user mismatch");
Require(launch.Key == "abc-123", "key mismatch");
Require(launch.EditBy == "Trminhpc", "editby mismatch");

var swf = launch.BuildSwfUri(gameBase);
Require(swf.AbsoluteUri.Contains("/Gunny/flash/Loading.swf?", StringComparison.Ordinal), "SWF path mismatch");
Require(swf.Query.Contains("user=test%20user", StringComparison.Ordinal), "user query missing");
Require(swf.Query.Contains("key=abc-123", StringComparison.Ordinal), "key query missing");
Require(swf.Query.Contains("config=http%3A%2F%2F103.9.156.181%2FGunny%2Fconfig.xml", StringComparison.OrdinalIgnoreCase), "config query missing");

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
Require(argLine.Contains("--socket-allow|103.9.156.181:9200", StringComparison.Ordinal), "socket allowlist missing");
Require(argLine.Contains("--graphics|gl", StringComparison.Ordinal), "OpenGL graphics override required to avoid the Intel DX12 wgpu OOM path");
Require(argLine.Contains("--no-avm2-optimizer", StringComparison.Ordinal), "AVM2 optimizer must be disabled for legacy Alchemy module");
Require(argLine.Contains("--tcp-connections|allow", StringComparison.Ordinal), "game TCP connections must be enabled for the Road server");
Require(argLine.Contains("--base|http://103.9.156.181/Gunny/flash/", StringComparison.OrdinalIgnoreCase), "Ruffle base missing");
var v389SaveIndex = ruffleArgs.ToList().FindIndex(x => x == "--save-directory");
Require(v389SaveIndex >= 0 && v389SaveIndex + 1 < ruffleArgs.Count, "v389 Ruffle save-directory missing");
var v389SaveDirectory = ruffleArgs[v389SaveIndex + 1];
Require(v389SaveDirectory.EndsWith(Path.Combine("BaseGunnyII", "Ruffle", "v389", "SharedObjects"), StringComparison.OrdinalIgnoreCase), "v389 Ruffle storage must be profile isolated");
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

var v30Base = new Uri("http://103.9.156.181:8083/gunny/");
var v30Profile = GunnyServerProfile.Resolve(v30Base);
Require(v30Profile.IsLegacyV30, "port 8083 must select Gunny 3.0 profile");
Require(v30Profile.SocketPort == 9300, "Gunny 3.0 socket port mismatch");
var v30Launch = new GameLaunchInfo("v30 user", "v30-key", string.Empty);
var v30Swf = v30Launch.BuildSwfUri(v30Base);
Require(v30Swf.AbsolutePath.EndsWith("/gunny/Loading.swf", StringComparison.OrdinalIgnoreCase), "Gunny 3.0 SWF must load from webroot");
Require(v30Swf.Query.Contains("config=http%3A%2F%2F103.9.156.181%3A8083%2Fgunny%2Fconfig.xml", StringComparison.OrdinalIgnoreCase), "Gunny 3.0 config query missing");
var v30Args = RuffleLaunchCommand.BuildArguments(v30Launch, v30Base);
var v30ArgLine = string.Join("|", v30Args);
Require(v30ArgLine.Contains("--socket-allow|103.9.156.181:9300", StringComparison.Ordinal), "Gunny 3.0 socket allowlist mismatch");
Require(v30ArgLine.Contains("--base|http://103.9.156.181:8083/gunny/", StringComparison.OrdinalIgnoreCase), "Gunny 3.0 Ruffle base mismatch");
var v30SaveIndex = v30Args.ToList().FindIndex(x => x == "--save-directory");
Require(v30SaveIndex >= 0 && v30SaveIndex + 1 < v30Args.Count, "v30 Ruffle save-directory missing");
var v30SaveDirectory = v30Args[v30SaveIndex + 1];
Require(v30SaveDirectory.EndsWith(Path.Combine("BaseGunnyII", "Ruffle", "v30", "SharedObjects"), StringComparison.OrdinalIgnoreCase), "v30 Ruffle storage must be profile isolated");
Require(!string.Equals(v389SaveDirectory, v30SaveDirectory, StringComparison.OrdinalIgnoreCase), "v389 and v30 must never share Ruffle save data");

var v30LoginHandler = new V30LoginHandler();
var v30Client = new GunnyLoginClient(v30Base, v30LoginHandler);
var v30Authenticated = await v30Client.AuthenticateAsync("v30 user", "secret pass", CancellationToken.None);
Require(v30Authenticated.Key == "v30-key", "Gunny 3.0 login redirect key mismatch");
Require(v30LoginHandler.Requests.Count == 1, "Gunny 3.0 login must be one direct POST");
Require(v30LoginHandler.Requests[0].Uri.AbsolutePath.EndsWith("/gunny/LoginGame.aspx", StringComparison.OrdinalIgnoreCase), "Gunny 3.0 LoginGame endpoint mismatch");
var v30Post = v30LoginHandler.Requests[0].Body ?? string.Empty;
Require(v30Post.Contains("txtUserName=v30+user", StringComparison.Ordinal), "Gunny 3.0 username field missing");
Require(v30Post.Contains("txtPassword=", StringComparison.Ordinal), "Gunny 3.0 hashed password field missing");
Require(!v30Post.Contains("secret+pass", StringComparison.Ordinal), "Gunny 3.0 password must be MD5 hashed before POST");

var v30RegistrationHandler = new V30RegistrationHandler();
var v30Registration = new GunnyRegistrationClient(v30Base, v30RegistrationHandler);
var v30Captcha = await v30Registration.GetCaptchaAsync(CancellationToken.None);
Require(v30Captcha.ContentType == "image/gif", "Gunny 3.0 captcha content type mismatch");
var v30RegistrationResult = await v30Registration.RegisterAsync("newuser", "test123", "test123", "u@example.com", "1", "A1B2C", CancellationToken.None);
Require(v30RegistrationResult.Success, "Gunny 3.0 registration should accept success page");
Require(v30RegistrationHandler.Requests.Count == 3, "Gunny 3.0 registration must use captcha, form GET, form POST");
Require(v30RegistrationHandler.Requests[0].Uri.AbsolutePath.EndsWith("/Register/Handler.ashx", StringComparison.OrdinalIgnoreCase), "Gunny 3.0 captcha endpoint mismatch");
var v30RegisterPost = v30RegistrationHandler.Requests[2].Body ?? string.Empty;
Require(v30RegisterPost.Contains("TxtName=newuser", StringComparison.Ordinal), "Gunny 3.0 register username missing");
Require(v30RegisterPost.Contains("TxtCharName=newuser", StringComparison.Ordinal), "Gunny 3.0 register character name missing");
Require(v30RegisterPost.Contains("TextCode=A1B2C", StringComparison.Ordinal), "Gunny 3.0 register captcha missing");
Require(v30RegisterPost.Contains("Button1.x=1", StringComparison.Ordinal), "Gunny 3.0 ImageButton submit field missing");
Console.WriteLine("GUNNY_V30_SMOKE=PASS");

Console.WriteLine("GUNNY_LOGIN_SMOKE=PASS");

sealed class V30LoginHandler : HttpMessageHandler
{
    public List<CapturedRequest> Requests { get; } = new();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
        Requests.Add(new CapturedRequest(request.Method, request.RequestUri!, body));
        var response = new HttpResponseMessage(HttpStatusCode.Redirect);
        response.Headers.Location = new Uri("http://103.9.156.181:8083/gunny/Default.aspx?user=v30%20user&key=v30-key&site=&sitename=");
        return response;
    }
}

sealed class V30RegistrationHandler : HttpMessageHandler
{
    public List<CapturedRequest> Requests { get; } = new();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
        Requests.Add(new CapturedRequest(request.Method, request.RequestUri!, body));
        if (Requests.Count == 1)
        {
            var content = new ByteArrayContent(new byte[] { 4, 5, 6 });
            content.Headers.ContentType = new("image/gif");
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
        }
        if (Requests.Count == 2)
        {
            const string page = "<input type=\"hidden\" name=\"__VIEWSTATE\" value=\"state&amp;1\" /><input type=\"hidden\" name=\"__VIEWSTATEGENERATOR\" value=\"gen\" /><input type=\"hidden\" name=\"__EVENTVALIDATION\" value=\"event\" />";
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(page) };
        }
        if (Requests.Count == 3)
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("<span id=\"Label1\"><font color='Green'>Dang ky thanh cong!</font></span>") };
        throw new InvalidOperationException("Unexpected Gunny 3.0 registration request.");
    }
}

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
            response.Headers.Location = new Uri("http://103.9.156.181/Gunny/Default.aspx?user=test%20user&key=server-guid&editby=Trminhpc");
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
