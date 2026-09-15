using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Cryptography;
using System.Text;

namespace RuntimeSourceInventory;

public sealed record InventoryInput(string CanonicalRoot, string RecoveredRoot);
public sealed record RuntimeTypeVariant(string Service, string Assembly, string File, string BodyHash);
public sealed record TypeInventory(
    string FullName,
    bool CanonicalExists,
    string? CanonicalBodyHash,
    IReadOnlyList<RuntimeTypeVariant> RuntimeVariants,
    IReadOnlyList<string> RuntimeVariantBodyHashes,
    string Classification,
    bool AutoPromotionAllowed);

public sealed record InventoryResult(IReadOnlyList<TypeInventory> Types, IReadOnlyList<MemberInventory> Members, IReadOnlyList<RuntimeFileInventory> Files);

public static partial class InventoryEngine
{
    public static InventoryResult Analyze(InventoryInput input)
    {
        var canonical = ParseCanonical(input.CanonicalRoot);
        var runtime = ParseRecovered(input.RecoveredRoot);
        var names = runtime.Keys.OrderBy(x => x, StringComparer.Ordinal);
        var rows = new List<TypeInventory>();
        foreach (var name in names)
        {
            var variants = runtime[name].OrderBy(x => x.Service).ThenBy(x => x.Assembly).ThenBy(x => x.File).ToList();
            var hashes = variants.Select(x => x.BodyHash).Distinct(StringComparer.Ordinal).OrderBy(x => x).ToList();
            canonical.TryGetValue(name, out var canonicalHash);
            var canonicalExists = canonicalHash is not null;
            var classification = hashes.Count > 1
                ? "variant-conflict"
                : !canonicalExists
                    ? "runtime-only"
                    : string.Equals(canonicalHash, hashes[0], StringComparison.Ordinal)
                        ? "canonical-present"
                        : "same-signature/body-different";
            rows.Add(new TypeInventory(
                name,
                canonicalExists,
                canonicalHash,
                variants,
                hashes,
                classification,
                classification == "runtime-only" && hashes.Count == 1));
        }
        return new InventoryResult(rows, BuildMemberInventory(input), BuildFileInventory(input.RecoveredRoot));
    }

    static Dictionary<string, string> ParseCanonical(string root)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var file in CanonicalSourceFiles(root))
        {
            foreach (var type in ParseTypes(file))
                result[type.FullName] = type.BodyHash;
        }
        return result;
    }

    static Dictionary<string, List<RuntimeTypeVariant>> ParseRecovered(string root)
    {
        var result = new Dictionary<string, List<RuntimeTypeVariant>>(StringComparer.Ordinal);
        foreach (var file in SourceFiles(root))
        {
            var rel = Path.GetRelativePath(root, file);
            var parts = rel.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var service = parts.Length > 0 ? parts[0] : "unknown";
            var assembly = parts.Length > 1 ? parts[1] : "unknown";
            foreach (var type in ParseTypes(file))
            {
                if (!result.TryGetValue(type.FullName, out var list))
                    result[type.FullName] = list = [];
                list.Add(new RuntimeTypeVariant(service, assembly, rel, type.BodyHash));
            }
        }
        return result;
    }

    static IEnumerable<string> CanonicalSourceFiles(string root) =>
        SourceFiles(root).Where(p => !Path.GetRelativePath(root, p)
            .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)[0]
            .Equals("recovery", StringComparison.OrdinalIgnoreCase));

    static IEnumerable<string> SourceFiles(string root) =>
        Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(p => !HasIgnoredSegment(root, p));
    static bool HasIgnoredSegment(string root, string path)
    {
        var rel = Path.GetRelativePath(root, path);
        var parts = rel.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return parts.Any(p => p is "bin" or "obj" or ".git");
    }

    static IEnumerable<(string FullName, string BodyHash)> ParseTypes(string file)
    {
        var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file), new CSharpParseOptions(LanguageVersion.Preview));
        foreach (var type in tree.GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
            yield return (FullTypeName(type), HashNormalized(type));
    }

    static string FullTypeName(BaseTypeDeclarationSyntax type)
    {
        var namespaces = type.Ancestors().OfType<BaseNamespaceDeclarationSyntax>()
            .Reverse().Select(x => Normalize(x.Name.ToString()));
        var parents = type.Ancestors().OfType<BaseTypeDeclarationSyntax>()
            .Reverse().Select(x => x.Identifier.ValueText);
        return string.Join(".", namespaces.Concat(parents).Append(type.Identifier.ValueText));
    }

    static string HashNormalized(SyntaxNode node)
    {
        var normalized = node.WithoutTrivia().NormalizeWhitespace().ToFullString();
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalized))).ToLowerInvariant();
    }

    static string Normalize(string value) => new(value.Where(c => !char.IsWhiteSpace(c)).ToArray());
}
