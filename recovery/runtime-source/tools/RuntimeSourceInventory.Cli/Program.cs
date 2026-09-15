using RuntimeSourceInventory;
using System.Text.Json;

if (args.Length != 4)
{
    Console.Error.WriteLine("Usage: RuntimeSourceInventory.Cli <canonicalRoot> <recoveredRoot> <jsonOut> <mdOut>");
    return 2;
}

var canonicalRoot = Path.GetFullPath(args[0]);
var recoveredRoot = Path.GetFullPath(args[1]);
var jsonOut = Path.GetFullPath(args[2]);
var mdOut = Path.GetFullPath(args[3]);
var result = InventoryEngine.Analyze(new InventoryInput(canonicalRoot, recoveredRoot));
var additiveTypes = result.Types.Where(x => x.AutoPromotionAllowed).Select(x => x.FullName).ToArray();
InventoryEngine.ValidateAdditiveTypes(result, additiveTypes);

var typeCounts = result.Types.GroupBy(x => x.Classification)
    .ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);
var memberCounts = result.Members.GroupBy(x => x.Classification)
    .ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);
var duplicateHashGroups = result.Files.GroupBy(x => x.ContentHash).Count(g => g.Count() > 1);
var payload = new
{
    generatedUtc = DateTimeOffset.UtcNow,
    canonicalRoot,
    recoveredRoot,
    summary = new
    {
        types = result.Types.Count,
        members = result.Members.Count,
        files = result.Files.Count,
        additiveTypes = additiveTypes.Length,
        duplicateHashGroups,
        typeCounts,
        memberCounts
    },
    types = result.Types,
    members = result.Members,
    files = result.Files
};
Directory.CreateDirectory(Path.GetDirectoryName(jsonOut)!);
await File.WriteAllTextAsync(jsonOut, JsonSerializer.Serialize(payload,
    new JsonSerializerOptions { WriteIndented = true }));

var md = new List<string>
{
    "# Canonical runtime source semantic diff",
    "",
    $"Generated UTC: `{DateTimeOffset.UtcNow:O}`",
    $"Canonical root: `{canonicalRoot}`",
    $"Recovered root: `{recoveredRoot}`",
    "",
    "## Summary",
    "",
    $"- Runtime types: **{result.Types.Count}**",
    $"- Runtime members: **{result.Members.Count}**",
    $"- Runtime C# files: **{result.Files.Count}**",
    $"- Auto-promotion type candidates: **{additiveTypes.Length}**",
    $"- Duplicate file-content hash groups: **{duplicateHashGroups}**",
    "",
    "### Type classifications"
};
foreach (var row in typeCounts.OrderBy(x => x.Key)) md.Add($"- `{row.Key}`: {row.Value}");
md.Add("");
md.Add("### Member classifications");
foreach (var row in memberCounts.OrderBy(x => x.Key)) md.Add($"- `{row.Key}`: {row.Value}");
md.Add("");
md.Add("### Files by service / assembly");
foreach (var group in result.Files.GroupBy(x => new { x.Service, x.Assembly })
             .OrderBy(g => g.Key.Service).ThenBy(g => g.Key.Assembly))
    md.Add($"- `{group.Key.Service}/{group.Key.Assembly}`: {group.Count()} C# files");
md.Add("");
md.Add("## Auto-promotion type candidates");
md.Add("");
foreach (var type in result.Types.Where(x => x.AutoPromotionAllowed).OrderBy(x => x.FullName))
    md.Add($"- `{type.FullName}` — {string.Join(", ", type.RuntimeVariants.Select(v => v.Service + "/" + v.Assembly).Distinct())}");
md.Add("");
md.Add("## Variant-conflict types");
md.Add("");
foreach (var type in result.Types.Where(x => x.Classification == "variant-conflict").OrderBy(x => x.FullName))
    md.Add($"- `{type.FullName}` — {type.RuntimeVariantBodyHashes.Count} distinct bodies");
md.Add("");
md.Add("## Notes");
md.Add("");
md.Add("`AutoPromotionAllowed` is intentionally type-level only. Member-only deltas and variant conflicts require manual semantic review.");
await File.WriteAllLinesAsync(mdOut, md);

Console.WriteLine($"TYPES={result.Types.Count}");
Console.WriteLine($"MEMBERS={result.Members.Count}");
Console.WriteLine($"FILES={result.Files.Count}");
Console.WriteLine($"ADDITIVE_TYPES={additiveTypes.Length}");
Console.WriteLine($"VARIANT_TYPES={result.Types.Count(x => x.Classification == "variant-conflict")}");
Console.WriteLine($"JSON={jsonOut}");
Console.WriteLine($"MD={mdOut}");
return 0;
