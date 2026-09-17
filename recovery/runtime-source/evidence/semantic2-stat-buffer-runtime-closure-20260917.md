# Semantic recovery — stat buffer runtime closure (2026-09-17)

## Base and scope
- Base branch: `recovery/runtime-semantic-phase2-20260915`.
- Base SHA: `27e9ec448e7219d0a90af3a021ce506345300415`.
- Scope restores the seven basic stat buffers end-to-end: Agility, Attack, Damage, Defence, Guard, HP, and Luck.
- No `master` merge, production deployment, DLL copy, service restart, or force push is part of this wave.

## TDD and dependency closure
- RED guard failed all 5 closure contracts before the production-source patch.
- GREEN guard passes all 5 after the patch: 7 `PlayerInfo.*AddPlus` fields, 7 buffer sources, 7 project mappings, factory cases 74–80, and gameplay consumers.
- Detached qualification proved classes alone were insufficient; runtime reachability also requires `BufferList.CreateBuffer` cases 74–80 and `Game.Logic.Player` consumers.
- Canonical HP/base-stat formulas are preserved; only the recovered additive buff terms are restored.
- `PrepareNewTurn` retains canonical/runtime-compatible integer energy semantics through `(int)Agility / 30 + 240`.

## Semantic parity
- All 7 buffer implementations match recovered Road runtime after namespace-only normalization for the canonical C# language level.
- All 7 `PlayerInfo` fields are present identically in recovered Road, Fight, and center variants and in the candidate canonical source.
- Factory mapping parity: 74=Defend, 75=Attack, 76=Guard, 77=Agi, 78=Dame, 79=Hp, 80=Luck.
- Consumer audit confirms all 7 additive fields are applied in gameplay state.
- Independent parity verifier result: `PARITY_ALL=True`.

## Fresh build verification
- Toolchain: .NET SDK `8.0.425` MSBuild/Roslyn with net35 reference root `C:\Gunny\BuildRefs\net35pkg\build\`.
- `SqlDataProvider`: exit `0`, compiler errors `0`.
- `Game.Logic`: exit `0`, compiler errors `0`.
- `Game.Server`: exit `0`, compiler errors `0`.
- Builds were run sequentially with `/m:1`; `PostBuildEvent=` was disabled. No build output was deployed.

## Safety and inventory
- Added source lines scanned: `53`.
- Sensitive hits: `0`.
- Decompiler/artifact hits: `0`.
- CRLF-aware `git diff --check`: `0`.
- Fresh inventory: `1551` runtime types, `10639` runtime members, `2334` recovered C# files.
- Type classifications: runtime-only `196`, variant-conflict `59`, canonical-present `720`.
- Member classifications: runtime-only `2389`, variant-conflict `176`, canonical-present `5838`.
- Delta from the previous sealed checkpoint: runtime-only types `203→196`, canonical-present `713→720`; runtime-only members `2410→2389`, canonical-present `5817→5838`; variant types unchanged at `59`.

## Release discipline
- Commit/push is permitted only if local and remote branch heads still equal the base SHA at the final anti-drift gate.
- Push must be fast-forward only. No force/bypass.
- `master` and production remain untouched by this recovery wave.
