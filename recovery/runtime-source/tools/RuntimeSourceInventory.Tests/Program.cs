using RuntimeSourceInventory;

var root = Path.Combine(Path.GetTempPath(), "gunny-runtime-inventory-tests", Guid.NewGuid().ToString("N"));
var canonical = Path.Combine(root, "canonical");
var recovered = Path.Combine(root, "recovered");
Directory.CreateDirectory(canonical);
Directory.CreateDirectory(Path.Combine(recovered, "center", "Demo"));
Directory.CreateDirectory(Path.Combine(recovered, "Fight", "Demo"));

File.WriteAllText(Path.Combine(canonical, "Shared.cs"), "namespace Demo; public class Shared { public int Value() => 1; }");
File.WriteAllText(Path.Combine(recovered, "center", "Demo", "Shared.cs"), "namespace Demo; public class Shared { public int Value() => 1; }");
File.WriteAllText(Path.Combine(recovered, "Fight", "Demo", "Shared.cs"), "namespace Demo; public class Shared { public int Value() => 2; }");

var result = InventoryEngine.Analyze(new InventoryInput(canonical, recovered));
var shared = result.Types.Single(x => x.FullName == "Demo.Shared");
Assert(shared.Classification == "variant-conflict", "same type with divergent runtime bodies must be variant-conflict");
Assert(shared.CanonicalExists, "canonical type should be detected");
Assert(shared.RuntimeVariantBodyHashes.Distinct().Count() == 2, "two runtime body hashes should be preserved");
Directory.CreateDirectory(Path.Combine(recovered, "Road", "Demo"));
File.WriteAllText(Path.Combine(recovered, "Road", "Demo", "NewType.cs"), "namespace Demo; public class NewType { public void Run() {} }");
File.WriteAllText(Path.Combine(recovered, "Fight", "Demo", "NewType.cs"), "namespace Demo; public class NewType { public void Run() {} }");

result = InventoryEngine.Analyze(new InventoryInput(canonical, recovered));
var additive = result.Types.Single(x => x.FullName == "Demo.NewType");
Assert(additive.Classification == "runtime-only", "absent canonical type with one runtime body must be runtime-only");
Assert(additive.AutoPromotionAllowed, "single-body runtime-only type should be eligible for additive promotion");


static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

var valueMember = result.Members.Single(x => x.Signature == "Demo.Shared.Value()");
Assert(valueMember.Classification == "variant-conflict", "same member signature with divergent runtime bodies must be variant-conflict");
Assert(valueMember.CanonicalExists, "canonical member should be detected");
var runMember = result.Members.Single(x => x.Signature == "Demo.NewType.Run()");
Assert(runMember.Classification == "runtime-only", "runtime-only member should be classified separately");

var nestedRecovery = Path.Combine(canonical, "recovery", "runtime-source", "by-service", "Road", "Demo");
Directory.CreateDirectory(nestedRecovery);
File.WriteAllText(Path.Combine(nestedRecovery, "Ghost.cs"), "namespace Demo; public class Ghost { public void Hidden() {} }");
File.WriteAllText(Path.Combine(recovered, "Road", "Demo", "Ghost.cs"), "namespace Demo; public class Ghost { public void Hidden() {} }");
result = InventoryEngine.Analyze(new InventoryInput(canonical, recovered));
Assert(result.Types.All(x => x.FullName != "Demo.Ghost" || !x.CanonicalExists), "canonical scan must ignore recovery subtree");

Directory.Delete(root, recursive: true);
Console.WriteLine("RuntimeSourceInventory.Tests: PASS");

AssertThrows(
    () => InventoryEngine.ValidateAdditiveTypes(result, new[] { "Demo.Shared" }),
    "canonical-present additive proposal must fail closed");
InventoryEngine.ValidateAdditiveTypes(result, new[] { "Demo.NewType" });

static void AssertThrows(Action action, string message)
{
    try { action(); }
    catch (InvalidOperationException) { return; }
    throw new InvalidOperationException(message);
}

var duplicateFiles = result.Files.Where(x => x.File.EndsWith("NewType.cs", StringComparison.Ordinal)).ToList();
Assert(duplicateFiles.Count == 2, "both service copies of NewType.cs must be inventoried");
Assert(duplicateFiles.Select(x => x.ContentHash).Distinct(StringComparer.Ordinal).Count() == 1,
    "identical service files must share one content hash");
