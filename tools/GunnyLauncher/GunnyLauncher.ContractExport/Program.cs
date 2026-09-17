using System.Text.Json;
using GunnyLauncher.Core;

var outputPath = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.GetFullPath(Path.Combine("mobile", "contracts", "gunny-launch-contract.json"));

var gameBase = new Uri("http://103.9.156.181/Gunny/");
var redirect = new Uri("http://103.9.156.181/Gunny/Default.aspx?user=fixture&key=fixture-key&editby=Trminhpc");
var launch = GameLaunchInfo.ParseRedirect(redirect);
var swf = launch.BuildSwfUri(gameBase);

var contract = new
{
    schemaVersion = 1,
    gameBase = gameBase.AbsoluteUri,
    login = new
    {
        method = "POST",
        endpoint = GunnyProtocolContract.LoginEndpoint,
        redirectMethod = "GET",
        redirectEndpoint = GunnyProtocolContract.LoginGameEndpoint
    },
    registration = new
    {
        captchaMethod = "GET",
        captchaEndpoint = GunnyProtocolContract.CaptchaEndpoint,
        registerMethod = "POST",
        registerEndpoint = GunnyProtocolContract.RegisterEndpoint,
        fields = GunnyProtocolContract.RegistrationFields
    },
    sample = new
    {
        redirect = redirect.AbsoluteUri,
        user = launch.User,
        key = launch.Key,
        editBy = launch.EditBy,
        swf = swf.AbsoluteUri
    },
    android = new
    {
        packageName = RuffleAndroidContract.PackageName
    },
    gameSocket = new
    {
        host = "103.9.156.181",
        port = 9200
    }
};

var options = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = null
};
var json = JsonSerializer.Serialize(contract, options) + Environment.NewLine;
Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
File.WriteAllText(outputPath, json, new System.Text.UTF8Encoding(false));
Console.WriteLine($"GUNNY_CONTRACT_EXPORT={outputPath}");
