using System.Security.Cryptography;

namespace RuntimeSourceInventory;

public sealed record RuntimeFileInventory(
    string Service,
    string Assembly,
    string File,
    string ContentHash);

public static partial class InventoryEngine
{
    static IReadOnlyList<RuntimeFileInventory> BuildFileInventory(string recoveredRoot)
    {
        var rows = new List<RuntimeFileInventory>();
        foreach (var file in SourceFiles(recoveredRoot))
        {
            var rel = Path.GetRelativePath(recoveredRoot, file);
            var parts = rel.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var service = parts.Length > 0 ? parts[0] : "unknown";
            var assembly = parts.Length > 1 ? parts[1] : "unknown";
            using var stream = File.OpenRead(file);
            var hash = Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
            rows.Add(new RuntimeFileInventory(service, assembly, rel, hash));
        }
        return rows.OrderBy(x => x.Service).ThenBy(x => x.Assembly).ThenBy(x => x.File).ToList();
    }
}
