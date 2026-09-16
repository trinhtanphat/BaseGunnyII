# Semantic phase 2: ChangeSpecialBall state recovery

Date: 2026-09-16
Parent recovery commit: `0c0e42abf88d5f1e24372271fcc9aa5e661b9760`
Branch: `recovery/runtime-semantic-phase2-20260915`

## Dependency closure

Recovered Road and Fight `Player` sources agree on the full lifecycle: backing field `m_changeSpecialball`, public `ChangeSpecialBall`, constructor/reset initialization to zero, and a `SetCurrentWeapon` override that selects ball config template `70396` whenever `ChangeSpecialBall > 0`.
Canonical source lacked all of those sites, so restoring only the property would have been compile-only and semantically incomplete. This wave restores the complete self-contained player-side behavior and does not add persistence or packet contracts.

The canonical source uses `var ballConfig` while recovered source uses `BallConfigInfo ballConfigInfo`; the restored override therefore applies the same `FindBall(70396)` substitution to the canonical variable without changing the surrounding ball configuration logic.

## TDD and qualification

The guard was introduced first and RED for six contracts: backing field, property, constructor init, reset init, override guard, and override template.
After promotion the guard is GREEN for all six contracts.
Road/Fight/canonical verification is TRUE for the field, property, init/reset lifecycle, and template-70396 override.
In the detached all-Messions qualification sandbox, blocker rows fell from `84` to `83`; the sole `ChangeSpecialBall` error disappeared and no replacement compiler error appeared.
## Fresh build verification

Toolchain: `C:\Gunny\BuildTools\dotnet8\dotnet.exe` SDK 8.0.425 / MSBuild 17.11.48 with the .NET Framework 3.5 reference pack and `LangVersion=7.3`.
`Game.Logic` Release rebuild: exit `0`, compiler error lines `0`.
`GameServerScript` Release rebuild: exit `0`, compiler error lines `0`; `PostBuildEvent=` was explicitly disabled.

## Fresh runtime-source inventory

- Runtime types: `1551`; runtime members: `10639`; recovered C# files: `2334`.
- `runtime-only` types: `203`; `canonical-present` types: `713`; `variant-conflict` types: `59`.
- `runtime-only` members: `2410`; `canonical-present` members: `5817`; `variant-conflict` members: `176`.
- Classification counts are unchanged because this wave restores state/behavior inside the already-canonical `Player` type.

## Safety

Sensitive scan: `0` hits. Decompiler/source-artifact scan: `0` hits.
`git -c core.whitespace=cr-at-eol diff --check` passes while preserving the BOM+CRLF policy of `Player.cs`.
No production deploy/service restart was performed. `master` remains untouched and no force-push is permitted.
