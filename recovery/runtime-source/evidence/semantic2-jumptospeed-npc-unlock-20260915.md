# Phase 2 - unrestricted jump speed unlock

Date: 2026-09-15
Branch: `recovery/runtime-semantic-phase2-20260915`

Recovered Fight and Road runtimes agree on the additive `Living.JumpToSpeed(...)` method.
It resolves terrain Y and always enqueues `LivingJumpAction` with the caller-provided speed, unlike legacy `JumpTo` which retains its original height guard.

TDD smoke: `tests/runtime-semantic-jumptospeed-smoke.ps1`.
The smoke was observed RED before the method existed and GREEN after implementation.

Compiler-guided qualification started with 61 deferred NPC files. After restoring `JumpToSpeed`, 48 still had compiler errors and 13 became dependency-closed.
Fresh inventory after the 13-file wave:
- runtime-only types: 389
- variant-conflict types: 59
- canonical-present types: 526
- runtime-only members: 4,286

Final guards: 13 files / 13 compile items, zero sensitive hits, zero binary/config artifacts, `git diff --check` clean, GameServerScript build exit 0 with zero errors.
