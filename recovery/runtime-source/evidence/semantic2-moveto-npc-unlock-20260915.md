# Semantic recovery phase 2: MoveTo/NPC unlock

Date: 2026-09-15
Branch: `recovery/runtime-semantic-phase2-20260915`
Base: merged recovery `master@b489f1517dc46b40010028eeb869d76d8623d09e`

## RED/GREEN contract

`tests/runtime-semantic-moveto-compat-smoke.ps1` was added before production edits.
The RED run failed on the missing recovered `Living.MoveTo(..., sAction, speed)` overload.
After the additive compatibility implementation, the smoke test passes.

The semantic cluster preserves the legacy movement overload and adds:

- secondary-action/speed `Living.MoveTo` overloads;
- delayed callback support in `LivingMoveToAction`;
- a `BaseGame.SendLivingMoveTo(..., sAction)` packet overload;
- an empty secondary action for legacy callers, preserving their packet shape.

`Game.Logic.csproj` native net35/refpack qualification: exit 0, 0 compiler errors.
## NPC dependency-closure result

The remaining canonical gap before this phase contained 217 runtime-only Road `GameServerScript/AI/NPC` files.
A temporary qualification wave added all 217 files and compile items without committing them.

Before this semantic cluster, all 217 had compiler blockers in the historical NPC qualification.
After the MoveTo cluster, only 136 files still produced compiler errors; 81 became compile-clean.
The 136 blocked files were removed from the temporary wave and the remaining 81-file set rebuilt successfully.

Final trimmed `GameServerScript.csproj` qualification, with `PostBuildEvent=` disabled: exit 0.
No production deploy or service restart was performed.

## Fresh semantic inventory

- recovered types: 1,551
- recovered members: 10,639
- recovered files: 2,334
- runtime-only types: **477** (previously 558)
- variant-conflict types: **59** (unchanged)
- runtime-only members: **5,304** (previously 6,119)

The 81 promoted NPC types therefore account for the exact 81-type reduction in the runtime-only set.
