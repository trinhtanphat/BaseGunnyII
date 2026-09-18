using GunnyLauncher.App;

static void Require(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

var tempRoot = Path.Combine(Path.GetTempPath(), "GunnyLauncherAppTests", Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(tempRoot);
var settingsPath = Path.Combine(tempRoot, "launcher.json");
var store = new LauncherSettingsStore(settingsPath);
store.Save(new LauncherSettings("http://127.0.0.1/Gunny/", "newuser"));
var loaded = store.Load();
Require(loaded.ServerUrl == "http://127.0.0.1/Gunny/", "server setting mismatch");
Require(loaded.Username == "newuser", "username setting mismatch");
var json = File.ReadAllText(settingsPath);
Require(!json.Contains("password", StringComparison.OrdinalIgnoreCase), "settings persisted password");
Require(!json.Contains("captcha", StringComparison.OrdinalIgnoreCase), "settings persisted captcha");

var v30Profile = Path.Combine(tempRoot, "v30.profile.json");
var v389Profile = Path.Combine(tempRoot, "v389.profile.json");
File.WriteAllText(v30Profile, "{\"Profile\":\"v30\",\"DefaultServerUrl\":\"http://103.9.156.181:8083/gunny/\"}");
File.WriteAllText(v389Profile, "{\"Profile\":\"v389\",\"DefaultServerUrl\":\"http://103.9.156.181/Gunny/\"}");
var v30Path = Path.Combine(tempRoot, "v30.json");
var v389Path = Path.Combine(tempRoot, "v389.json");
var v30Store = new LauncherSettingsStore(v30Path, v30Profile);
var v389Store = new LauncherSettingsStore(v389Path, v389Profile);
Require(v30Store.Load().ServerUrl == "http://103.9.156.181:8083/gunny/", "v30 profile default mismatch");
Require(v389Store.Load().ServerUrl == "http://103.9.156.181/Gunny/", "v389 profile default mismatch");
v30Store.Save(new LauncherSettings("http://103.9.156.181:8083/gunny/", "v30user"));
v389Store.Save(new LauncherSettings("http://103.9.156.181/Gunny/", "v389user"));
Require(v30Store.Load().Username == "v30user", "v30 profile save mismatch");
Require(v389Store.Load().Username == "v389user", "v389 profile save mismatch");
Require(File.ReadAllText(v30Path) != File.ReadAllText(v389Path), "profile settings collided");

File.WriteAllText(v389Path, "{\"ServerUrl\":\"http://103.9.156.181:8083/gunny/\",\"Username\":\"stale-v389-user\"}");
var repairedV389 = v389Store.Load();
Require(repairedV389.ServerUrl == "http://103.9.156.181/Gunny/", "v389 profile did not repair stale v30 server URL");
Require(repairedV389.Username == "stale-v389-user", "v389 stale-server repair lost remembered username");
Require(!v389Store.IsServerCompatible("http://103.9.156.181:8083/gunny/"), "v389 accepted a v30 server URL");
Require(v389Store.IsServerCompatible("http://103.9.156.181/Gunny/"), "v389 rejected its own server URL");

File.WriteAllText(v30Path, "{\"ServerUrl\":\"http://103.9.156.181/Gunny/\",\"Username\":\"stale-v30-user\"}");
var repairedV30 = v30Store.Load();
Require(repairedV30.ServerUrl == "http://103.9.156.181:8083/gunny/", "v30 profile did not repair stale v389 server URL");
Require(repairedV30.Username == "stale-v30-user", "v30 stale-server repair lost remembered username");
Require(!v30Store.IsServerCompatible("http://103.9.156.181/Gunny/"), "v30 accepted a v389 server URL");
Require(v30Store.IsServerCompatible("http://103.9.156.181:8083/gunny/"), "v30 rejected its own server URL");

var rejectedSave = false;
try
{
    v389Store.Save(new LauncherSettings("http://103.9.156.181:8083/gunny/", "wrong-runtime"));
}
catch (InvalidOperationException)
{
    rejectedSave = true;
}
Require(rejectedSave, "v389 profile persisted a cross-profile server URL");

ApplicationConfiguration.Initialize();
using var form = new Form1();
Require(form.Controls.Find("authTabs", true).Length == 1, "auth tabs missing");
Require(form.Controls.Find("loginTab", true).Length == 1, "login tab missing");
Require(form.Controls.Find("registerTab", true).Length == 1, "register tab missing");
Require(form.Controls.Find("captchaImage", true).Length == 1, "captcha image missing");
Require(form.Controls.Find("registerButton", true).Length == 1, "register button missing");

Console.WriteLine("GUNNY_LAUNCHER_UI_SMOKE=PASS");
Console.WriteLine("GUNNY_LAUNCHER_PROFILE_ISOLATION=PASS");
