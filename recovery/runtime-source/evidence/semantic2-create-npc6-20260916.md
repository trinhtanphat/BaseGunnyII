# Semantic phase 2: CreateNpc(6) recovery

Date: 2026-09-16
Parent recovery commit: `c1cdac78f7ee7d791ac6bd708590b2412b400626`
Branch: `recovery/runtime-semantic-phase2-20260915`

## Qualification

Fresh current-state qualification added 90 still-missing recovered Messions in an isolated detached worktree.
Baseline qualification produced 154 compiler error lines across the 90 mission files.
`PVEGame.CreateNpc(int,int,int,int,int,LivingConfig)` accounted for exactly 5 errors in 4 files: AC30001, Labyrinth40037, Labyrinth40039, and QX70001.
None of those four missions is dependency-closed yet, so this wave restores only the primitive overload and does not promote mission files.

## TDD and parity

The CreateNpc(6) guard was introduced first and was RED with all five runtime contracts missing.
After exact runtime restoration it is GREEN for signature, config assignment, ReduceBloodStart behavior, reset fallback, and registration/start-moving semantics.
Canonical-to-Road exact-token parity: `True`.
Fight-to-Road exact-token parity: `True`.
Sensitive scan: `0` hits. Decompiler/source-artifact scan: `0` hits.
## Fresh verification

Toolchain: `C:\Gunny\BuildTools\dotnet8\dotnet.exe` SDK 8.0.425 / MSBuild 17.11.48 with net35 reference assemblies and `LangVersion=7.3`.
`Game.Logic` Release rebuild: exit `0`, compiler error lines `0`.
`GameServerScript` Release rebuild: exit `0`, compiler error lines `0`; `PostBuildEvent=` explicitly disabled.
Detached all-Messions qualification after the overload: total errors `149` (was `154`), CreateNpc(6) errors `0` (was `5`).

Fresh runtime-source inventory: 1551 runtime types, 10639 runtime members, 2334 recovered C# files.
Type classifications are unchanged: runtime-only `235`, canonical-present `681`, variant-conflict `59`, same-signature/body-different `576`.
Member runtime-only is `2705` (was `2706`) and canonical-present is `5522` (was `5521`); variant-conflict remains `176` and same-signature/body-different remains `2236`.

## Safety

`git -c core.whitespace=cr-at-eol diff --check` passes while preserving repository EOL policy.
No `master` merge, force push, production deployment, service restart, or PostBuildEvent execution was performed.
The broader world-boss cluster (`CreateBoss(7)`, `WorldbossBood`, `AllWorldDameBoss`, `UpdatePveResult`, `TotalDameLiving`, related server/proxy state) remains deferred for cross-layer semantic review.
