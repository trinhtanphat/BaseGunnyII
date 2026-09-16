# Semantic phase 2: TotalDameLiving accounting recovery

Date: 2026-09-16
Parent recovery commit: `511727cef5d9f3ee9749160750fc2910d26a264d`
Branch: `recovery/runtime-semantic-phase2-20260915`

## Dependency closure

Recovered Road and Fight sources agree on exactly two semantic sites for this state: `Living.TotalDameLiving` and the `SimpleBomb` accumulation when a player damages a `SimpleBoss`.
Canonical source lacked both. No additional persistence, interface, packet, or server-side dependency is required for the accounting itself.

The canonical `SimpleBomb` predecessor uses names `p`, `damage`, and `critical` where recovered source uses `item4`, `num`, and `num2`. The restored expression therefore preserves recovered semantics as `m_owner.TotalDameLiving += critical + damage` under `m_owner is Player && p is SimpleBoss`.

## TDD and qualification

The guard was introduced first and RED for the field, player-to-boss accounting guard, and accumulation expression.
After promotion it is GREEN for all three contracts.
Road/Fight both contain the recovered field and identical recovered accounting expression; the canonical adaptation is explicitly verified against the corresponding canonical variables.

In the detached all-Messions qualification sandbox, blocker rows fell from `89` to `84`; all five `TotalDameLiving` errors disappeared and no replacement errors appeared.## Fresh build verification

Toolchain: `C:\Gunny\BuildTools\dotnet8\dotnet.exe` SDK 8.0.425 / MSBuild 17.11.48 with the .NET Framework 3.5 reference pack and `LangVersion=7.3`.
`Game.Logic` Release rebuild: exit `0`, compiler error lines `0`.
`GameServerScript` Release rebuild: exit `0`, compiler error lines `0`; `PostBuildEvent=` was explicitly disabled.

## Fresh runtime-source inventory

- Runtime types: `1551`; runtime members: `10639`; recovered C# files: `2334`.
- `runtime-only` types: `203`; `canonical-present` types: `713`; `variant-conflict` types: `59`.
- `runtime-only` members: `2410`; `canonical-present` members: `5817`; `variant-conflict` members: `176`.
- Classification counts are unchanged from the preceding Labyrinth wave because this promotion adds a field and restores accounting inside an already-canonical method rather than promoting a new runtime-only type/method.

## Safety

Sensitive scan: `0` hits. Decompiler/source-artifact scan: `0` hits.
`git -c core.whitespace=cr-at-eol diff --check` passes while preserving repository EOL policy.
No production deploy/service restart was performed. `master` remains untouched and no force-push is permitted.
