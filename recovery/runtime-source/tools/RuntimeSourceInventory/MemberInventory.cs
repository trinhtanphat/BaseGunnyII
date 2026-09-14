using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RuntimeSourceInventory;

public sealed record RuntimeMemberVariant(string Service, string Assembly, string File, string BodyHash);
public sealed record MemberInventory(
    string Signature,
    bool CanonicalExists,
    string? CanonicalBodyHash,
    IReadOnlyList<RuntimeMemberVariant> RuntimeVariants,
    IReadOnlyList<string> RuntimeVariantBodyHashes,
    string Classification);

public static partial class InventoryEngine
{
    static IReadOnlyList<MemberInventory> BuildMemberInventory(InventoryInput input)
    {
        var canonical = ParseCanonicalMembers(input.CanonicalRoot);
        var runtime = ParseRecoveredMembers(input.RecoveredRoot);
        var rows = new List<MemberInventory>();
        foreach (var signature in runtime.Keys.OrderBy(x => x, StringComparer.Ordinal))
        {
            var variants = runtime[signature].OrderBy(x => x.Service).ThenBy(x => x.Assembly).ThenBy(x => x.File).ToList();
            var hashes = variants.Select(x => x.BodyHash).Distinct(StringComparer.Ordinal).OrderBy(x => x).ToList();
            canonical.TryGetValue(signature, out var canonicalHash);
            var canonicalExists = canonicalHash is not null;
            var classification = hashes.Count > 1
                ? "variant-conflict"
                : !canonicalExists
                    ? "runtime-only"
                    : string.Equals(canonicalHash, hashes[0], StringComparison.Ordinal)
                        ? "canonical-present"
                        : "same-signature/body-different";
            rows.Add(new MemberInventory(signature, canonicalExists, canonicalHash, variants, hashes, classification));
        }
        return rows;
    }

    static Dictionary<string, string> ParseCanonicalMembers(string root)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var file in CanonicalSourceFiles(root))
            foreach (var member in ParseMembers(file))
                result[member.Signature] = member.BodyHash;
        return result;
    }

    static Dictionary<string, List<RuntimeMemberVariant>> ParseRecoveredMembers(string root)
    {
        var result = new Dictionary<string, List<RuntimeMemberVariant>>(StringComparer.Ordinal);
        foreach (var file in SourceFiles(root))
        {
            var rel = Path.GetRelativePath(root, file);
            var parts = rel.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var service = parts.Length > 0 ? parts[0] : "unknown";
            var assembly = parts.Length > 1 ? parts[1] : "unknown";
            foreach (var member in ParseMembers(file))
            {
                if (!result.TryGetValue(member.Signature, out var list))
                    result[member.Signature] = list = [];
                list.Add(new RuntimeMemberVariant(service, assembly, rel, member.BodyHash));
            }
        }
        return result;
    }

    static IEnumerable<(string Signature, string BodyHash)> ParseMembers(string file)
    {
        var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file), new CSharpParseOptions(LanguageVersion.Preview));
        var root = tree.GetRoot();
        foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
            yield return (MemberPrefix(method) + MethodName(method) + Params(method.ParameterList), HashNormalized(method));
        foreach (var ctor in root.DescendantNodes().OfType<ConstructorDeclarationSyntax>())
            yield return (MemberPrefix(ctor) + ".ctor" + Params(ctor.ParameterList), HashNormalized(ctor));
        foreach (var op in root.DescendantNodes().OfType<OperatorDeclarationSyntax>())
            yield return (MemberPrefix(op) + "operator" + op.OperatorToken.Text + Params(op.ParameterList), HashNormalized(op));
        foreach (var op in root.DescendantNodes().OfType<ConversionOperatorDeclarationSyntax>())
            yield return (MemberPrefix(op) + "operator" + op.ImplicitOrExplicitKeyword.Text + Normalize(op.Type.ToString()) + Params(op.ParameterList), HashNormalized(op));
    }

    static string MemberPrefix(SyntaxNode node)
    {
        var type = node.Ancestors().OfType<BaseTypeDeclarationSyntax>().FirstOrDefault();
        return type is null ? "<global>." : FullTypeName(type) + ".";
    }

    static string MethodName(MethodDeclarationSyntax method)
    {
        var explicitPrefix = method.ExplicitInterfaceSpecifier is null
            ? ""
            : Normalize(method.ExplicitInterfaceSpecifier.Name.ToString()) + ".";
        var arity = method.TypeParameterList?.Parameters.Count ?? 0;
        return explicitPrefix + method.Identifier.ValueText + (arity > 0 ? "`" + arity : "");
    }

    static string Params(ParameterListSyntax list)
    {
        var items = list.Parameters.Select(p => string.Concat(p.Modifiers.Select(m => m.Text)) + Normalize(p.Type?.ToString() ?? "?"));
        return "(" + string.Join(",", items) + ")";
    }
}
