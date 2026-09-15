# Phase 2 - source-aware periodic blood reduction unlock

Date: 2026-09-15
Branch: `recovery/runtime-semantic-phase2-20260915`

## Semantic delta

`ContinueReduceBloodEffect(int,int,Living)` restores the deployed source-aware periodic damage path while preserving the existing 2-argument constructor and legacy tick behavior.
The source-aware path uses `Living.AddBlood(-m_blood, 1)` and records kill credit when the source is a player.
Fight and Road recovered variants have the same constructor body hash.

## Qualification

44 deferred NPC files were compiled before the semantic patch; 36 still had compiler errors afterward.
After trimming only those 36 compiler-error files, the remaining 8 NPC scripts built with `GameServerScript` successfully.
Fresh inventory moved runtime-only types from 385 to 377, canonical-present types to 538, and runtime-only members to 4130.
Sensitive scan: 0 hits. Artifact scan: 0 hits. `git diff --check`: PASS.
