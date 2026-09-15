# Semantic phase 2 - variable strength reduction unlock

- Semantic commit: `564b643` restores variable `ReduceStrengthEffect(count, reduce)` while preserving the legacy 50-energy behavior.
- Qualification candidates retained: 7 NPC scripts.
- `GameServerScript` Release/net35 build: exit 0, 0 errors.
- Sensitive scan: 0 hits.
- `git diff --check`: PASS.
- Fresh runtime inventory: 1,551 types / 10,639 members / 2,334 recovered files.
- Runtime-only types: 377 -> 370.
- Variant-conflict types: 59 (unchanged).
- Canonical-present types: 545.
- Runtime-only members: 4,058.
