# Runtime Source Recovery Integration Design

## Goal

Preserve and integrate the C# source recovered from the deployed Gunny server binaries into `BaseGunnyII` without changing the running 182 deployment and without collapsing service-specific binary variants into one misleading source tree.

## Evidence baseline

- Canonical remote: `trinhtanphat/BaseGunnyII`, `master@88ee24ddff46629abde0fab198f0e4b53c866b26`.
- Recovery branch: `recovery/runtime-source-20260915` in an isolated worktree on 182.
- Decompiled runtime: 19 projects / 17 unique binary SHA variants.
- Rebuilt recovery source: 19/19 projects pass with `--no-restore`, 0 warnings, 0 errors.
- Deployed binaries: 19/19 SHA values still match the forensic baseline.
- Running services must not be restarted or overwritten by this work.

## Architecture

The repository will gain a variant-preserving recovery area under `recovery/runtime-source/`. It stores service-scoped recovered source and manifests so Center, Fight, and Road variants remain distinct even when assemblies share the same filename.

Canonical projects remain the product source of record. Recovered code is promoted into canonical projects only when Roslyn evidence proves the type is absent from the current canonical repo and the change can be compiled/tested without forcing one service variant over another.
## Integration rules

- Raw decompiler output is evidence and is never edited in place.
- Every imported file records service, assembly, runtime SHA256, and source path in a machine-readable manifest.
- Duplicate binaries are deduplicated by SHA in manifests, while service aliases remain visible.
- Generated SOAP/service proxy members and types found elsewhere in the canonical repo are not re-imported.
- Method-only deltas are never bulk-overwritten. They require signature-level and body-level semantic comparison before promotion.
- Existing canonical files are not replaced wholesale by decompiled files.
- No production binaries, configs, databases, IIS sites, or running processes are modified.

## Build strategy

The 182 host lacks .NET Framework 3.5/4.0 targeting packs, so the unchanged canonical solutions currently fail with MSB3644. Verification will use isolated reference assemblies under `C:\Gunny\_work\refpacks` or an equivalent non-system location; source projects will not be retargeted merely to satisfy the machine.

The recovered variant lane must remain independently buildable. Canonical promotion is gated by the relevant project/solution build plus Roslyn checks proving no duplicate fully-qualified type was introduced.

## Promotion order

1. Commit documentation, manifests, tools, and variant-preserving recovered source.
2. Generate a current-master Roslyn inventory of canonical types/methods.
3. Promote additive runtime-only types in small assembly groups, starting with low-conflict data/shared types.
4. Build and review after every group.
5. Review method-only deltas separately; promote only semantically justified methods.
6. Push the recovery branch only after fresh build, diff, and production-integrity checks.
