# Runtime Source Recovery Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Preserve the deployed Gunny runtime as buildable service-scoped C# source and safely promote verified missing code into canonical `BaseGunnyII` source.

**Architecture:** Keep all 19 recovered runtime projects under a service-scoped recovery lane so binary variants remain distinct. Use Roslyn AST inventories against current `master` to identify additive types and method deltas; only promote code into canonical projects after duplicate/type checks and build gates.

**Tech Stack:** C#, .NET Framework 3.5/4.0, .NET SDK 10 MSBuild, Roslyn, PowerShell, Git worktrees.

**Spec:** `docs/superpowers/specs/2026-09-15-runtime-source-recovery-design.md`

## Global Constraints

- Base SHA is `88ee24ddff46629abde0fab198f0e4b53c866b26` from `origin/master`.
- Work only in `C:\Gunny\_work\BaseGunnyII-runtime-recovery-20260915` on branch `recovery/runtime-source-20260915`.
- Do not restart or overwrite Center/Fighting/Road services on 182.
- Preserve raw forensic source under `C:\Gunny\_decompiled_20260914` unchanged.
- Never overwrite an existing canonical type wholesale with decompiled source.
- Method-only deltas require semantic review before promotion.

---
### Task 1: Restore isolated .NET Framework reference assemblies

**Files:**
- Create outside repo: `C:\Gunny\_work\refpacks\`
- Evidence: `C:\Gunny\_work\baseline-runtime-integration-20260915\`

**Interfaces:**
- Consumes: unchanged legacy `.csproj` targets `v3.5` and `v4.0`.
- Produces: a command-line build environment that does not retarget source projects.

- [ ] Create a temporary SDK project and restore `Microsoft.NETFramework.ReferenceAssemblies.net35` and `.net40` packages.
- [ ] Resolve the package reference-assembly directories from the NuGet cache.
- [ ] Run `CenterServer.sln`, `FightingServer.sln`, and `GameServer.sln` with a temporary targeting-root override or equivalent isolated MSBuild property.
- [ ] Record baseline exit codes and compiler errors without changing source.

### Task 2: Import the variant-preserving recovered source lane

**Files:**
- Create: `recovery/runtime-source/by-service/{center,Fight,Road}/...`
- Create: `recovery/runtime-source/manifest.json`
- Create: `recovery/runtime-source/README.md`
- Create: `recovery/runtime-source/evidence/`

**Interfaces:**
- Consumes: `C:\Gunny\_work\RuntimeByService-buildable-20260914` and recovery manifests/reports.
- Produces: service-scoped buildable recovered projects with provenance metadata.

- [ ] Copy source/project/resource files only; exclude `bin`, `obj`, `.git`, and runtime binaries.
- [ ] Copy Roslyn confirmed-diff, build, and SHA-integrity evidence into `evidence/`.
- [ ] Generate `manifest.json` with service, assembly, SHA256, source directory, and duplicate-SHA aliases.
- [ ] Verify file counts and that no `.exe`, `.dll`, `.pdb`, database, or secret/config payload was imported.
### Task 3: Build a current-master canonical Roslyn inventory

**Files:**
- Create: `recovery/runtime-source/tools/RuntimeSourceInventory/`
- Create: `recovery/runtime-source/evidence/canonical-diff.json`
- Create: `recovery/runtime-source/evidence/canonical-diff.md`

**Interfaces:**
- Consumes: current worktree source plus the service-scoped recovered lane.
- Produces: fully-qualified type/method comparisons against `BaseGunnyII@88ee24d`.

- [ ] Parse canonical and recovered C# with Roslyn syntax trees.
- [ ] Index fully-qualified types, constructors, methods, operators, and explicit-interface methods.
- [ ] Classify each runtime member as canonical-present, runtime-only, or same-signature/body-different.
- [ ] Group runtime-only files by assembly and detect when multiple service variants contain the same file content hash.
- [ ] Fail the tool if a proposed additive type already exists anywhere in canonical source.

### Task 4: Promote additive types with variant consensus

**Files:**
- Modify only canonical projects identified by `canonical-diff.json`.
- Modify matching legacy `.csproj` compile includes when required.

**Interfaces:**
- Consumes: canonical runtime-only type list and per-file content hashes.
- Produces: additive canonical source changes with no duplicate fully-qualified types.

- [ ] Start with files whose type is absent from canonical source and whose content is identical across every runtime variant that contains it.
- [ ] Add one assembly group at a time: data/shared types, then `Game.Logic`, then `Game.Server`, then scripts.
- [ ] Run Roslyn duplicate-type guard after each group.
- [ ] Build the affected project/solution using the isolated reference assemblies after each group.
- [ ] Revert any group that requires replacing an existing canonical type or selecting between conflicting service variants.
### Task 5: Review method-only semantic deltas

**Files:**
- Create: `recovery/runtime-source/evidence/method-review.json`
- Create: `recovery/runtime-source/evidence/method-review.md`
- Modify canonical source only for approved method-level deltas.

**Interfaces:**
- Consumes: Roslyn method signatures and normalized method bodies from recovered/canonical source.
- Produces: a classified queue of identical, compiler/decompiler-only, additive-method, and behavior-changing deltas.

- [ ] Normalize syntax bodies while preserving constants, member names, calls, and control flow.
- [ ] Auto-close exact/format-only matches.
- [ ] Promote only missing methods whose containing type exists, dependencies resolve, and no service-variant conflict exists.
- [ ] Keep behavior-changing or conflicting variants in the recovery lane with explicit review notes rather than overwriting canonical code.
- [ ] Build after every promoted method group.

### Task 6: Final verification and Git delivery

**Files:**
- Update: `recovery/runtime-source/evidence/final-verification.md`

**Interfaces:**
- Consumes: all integration changes.
- Produces: a reviewable recovery branch with fresh evidence.

- [ ] Run all three legacy server solution builds with isolated reference assemblies.
- [ ] Build all 19 recovered projects with `--no-restore` and require 0 errors.
- [ ] Re-run canonical Roslyn duplicate/type/member checks.
- [ ] Verify deployed runtime SHA256 remains 19/19 unchanged and the three service process start times are unchanged.
- [ ] Run `git diff --check` and inspect `git status` for accidental binaries, secrets, build outputs, or database files.
- [ ] Commit documentation/tooling/recovery lane separately from canonical promotions.
- [ ] Push `recovery/runtime-source-20260915`; do not merge to `master` unless protected checks/review are green.
