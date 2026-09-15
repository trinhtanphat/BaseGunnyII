namespace RuntimeSourceInventory;

public static partial class InventoryEngine
{
    public static void ValidateAdditiveTypes(InventoryResult result, IEnumerable<string> proposedTypes)
    {
        var byName = result.Types.ToDictionary(x => x.FullName, StringComparer.Ordinal);
        foreach (var name in proposedTypes)
        {
            if (!byName.TryGetValue(name, out var type))
                throw new InvalidOperationException($"Proposed additive type is not present in runtime inventory: {name}");
            if (type.CanonicalExists)
                throw new InvalidOperationException($"Proposed additive type already exists in canonical source: {name}");
            if (type.Classification != "runtime-only" || !type.AutoPromotionAllowed)
                throw new InvalidOperationException($"Proposed additive type is not unambiguous: {name} ({type.Classification})");
        }
    }
}
