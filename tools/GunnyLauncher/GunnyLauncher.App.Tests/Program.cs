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

ApplicationConfiguration.Initialize();
using var form = new Form1();
Require(form.Controls.Find("authTabs", true).Length == 1, "auth tabs missing");
Require(form.Controls.Find("loginTab", true).Length == 1, "login tab missing");
Require(form.Controls.Find("registerTab", true).Length == 1, "register tab missing");
Require(form.Controls.Find("captchaImage", true).Length == 1, "captcha image missing");
Require(form.Controls.Find("registerButton", true).Length == 1, "register button missing");

Console.WriteLine("GUNNY_LAUNCHER_UI_SMOKE=PASS");
