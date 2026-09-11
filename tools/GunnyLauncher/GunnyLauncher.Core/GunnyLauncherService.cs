using System.Diagnostics;

namespace GunnyLauncher.Core;

public sealed class GunnyLauncherService
{
    private readonly Uri _gameBase;
    private readonly string _runtimeRoot;
    private readonly HttpMessageHandler? _handler;

    public GunnyLauncherService(Uri gameBase, string runtimeRoot, HttpMessageHandler? handler = null)
    {
        _gameBase = gameBase ?? throw new ArgumentNullException(nameof(gameBase));
        _runtimeRoot = string.IsNullOrWhiteSpace(runtimeRoot)
            ? throw new ArgumentException("Runtime root is required.", nameof(runtimeRoot))
            : runtimeRoot;
        _handler = handler;
    }

    public async Task<ProcessStartInfo> BuildStartInfoAsync(string username, string password, CancellationToken cancellationToken)
    {
        var login = new GunnyLoginClient(_gameBase, _handler);
        var launch = await login.AuthenticateAsync(username, password, cancellationToken);
        var arguments = RuffleLaunchCommand.BuildArguments(launch, _gameBase);
        return RuffleProcessCommand.BuildStartInfo(_runtimeRoot, arguments);
    }
}
