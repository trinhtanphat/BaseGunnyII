# Phase 2 - explicit fall target unlock

Date: 2026-09-15
Branch: `recovery/runtime-semantic-phase2-20260915`

Recovered Fight and Road runtimes agree on the additive `Living.FallFromTo(...)` method.
It enqueues `LivingFallingAction` to caller-provided coordinates while legacy `FallFrom` keeps its terrain resolution behavior.

TDD smoke: `tests/runtime-semantic-fallfromto-smoke.ps1`.
Compiler-guided qualification started with 48 deferred NPC files; 44 still had errors and 4 became dependency-closed.
Fresh inventory after the 4-file wave:
- runtime-only types: 385
- variant-conflict types: 59
- canonical-present types: 530
- runtime-only members: 4,229

Final guards: 4 files / 4 compile items, `git diff --check` clean, GameServerScript build exit 0 with zero errors.
