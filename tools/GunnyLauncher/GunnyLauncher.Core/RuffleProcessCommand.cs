using System.Diagnostics;

namespace GunnyLauncher.Core;

public static class RuffleProcessCommand
{
    public static ProcessStartInfo BuildStartInfo(string runtimeRoot, IReadOnlyList<string> arguments)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(runtimeRoot);
        ArgumentNullException.ThrowIfNull(arguments);

        var info = new ProcessStartInfo
        {
            FileName = Path.Combine(runtimeRoot, "runtime", "ruffle.exe"),
            WorkingDirectory = runtimeRoot,
            UseShellExecute = false
        };

        foreach (var argument in arguments)
            info.ArgumentList.Add(argument);

        return info;
    }
}
