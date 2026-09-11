namespace GunnyLauncher.Core;

public static class GunnyProtocolContract
{
    public const string LoginEndpoint = "createLogin.ashx";
    public const string LoginGameEndpoint = "LoginGame.aspx";
    public const string CaptchaEndpoint = "auth/ValidateCode.aspx";
    public const string RegisterEndpoint = "auth/register.ashx";

    public static IReadOnlyList<string> RegistrationFields { get; } =
        new[] { "username", "password", "repassword", "email", "sex", "code" };
}
