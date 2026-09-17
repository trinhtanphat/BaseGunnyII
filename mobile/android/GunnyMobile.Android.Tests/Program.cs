using GunnyLauncher.Core;

static void Require(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

var gameBase = new Uri("http://103.9.156.181/Gunny/");
var launch = GameLaunchInfo.ParseRedirect(
    new Uri("http://103.9.156.181/Gunny/Default.aspx?user=test%20user&key=abc-123&editby=Trminhpc"));

Require(RuffleAndroidContract.PackageName == "rs.ruffle", "Ruffle package mismatch");
var uri = RuffleAndroidContract.BuildGameUri(launch, gameBase);
Require(uri.AbsolutePath.EndsWith("/flash/Loading.swf", StringComparison.Ordinal), "Android SWF path mismatch");
Require(uri.Query.Contains("key=abc-123", StringComparison.Ordinal), "signed key missing");
Require(uri.Query.Contains("user=test%20user", StringComparison.Ordinal), "signed user missing");

var descriptor = RuffleAndroidContract.CreateLaunchDescriptor(launch, gameBase);
Require(descriptor.Action == "android.intent.action.VIEW", "Android action mismatch");
Require(descriptor.MimeType == "application/x-shockwave-flash", "Android MIME mismatch");
Require(descriptor.PackageName == "rs.ruffle", "descriptor package mismatch");
Require(descriptor.GameUri == uri, "descriptor SWF URI mismatch");
Console.WriteLine("GUNNY_ANDROID_CONTRACT_SMOKE=PASS");
