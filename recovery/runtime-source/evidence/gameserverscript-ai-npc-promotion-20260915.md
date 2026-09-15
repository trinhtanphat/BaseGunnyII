# GameServerScript AI/NPC Promotion — 2026-09-15

- Branch: `recovery/runtime-source-20260915`
- Starting candidates: 287 runtime-only NPC type files.
- Source variants: 285 Road files, 2 Fight-only files (`NullAi`, `SeizeNpcAi`).
- Preflight: 0 destination collisions, 0 credential-pattern hits, 0 modern-syntax hints.
- Compatibility normalization: file-scoped namespaces converted to C# 7.3 block namespaces.
- Build target: `GameServerScript/GameServerScript.csproj`, Release, .NET Framework 3.5 reference pack.
- Qualification build overrides `PostBuildEvent=` to prevent copy/deploy side effects.
- Build round 1: 661 errors in 217 candidate files; 0 canonical files failed.
- Dominant blockers: runtime-only overload/member/constructor/model drift (`CS1501`, `CS1061`, `CS1729`, `CS0246`).
- Deferred: 217 compiler-proven dependency-blocked files; no core API was modified to force compatibility.
- Retained safe subset: 70 NPC type files.
- Build round 2: exit 0, 0 errors, 62 warnings.
- Project mapping: 70/70 retained files have explicit `<Compile Include>` entries.
- Artifact guard: 0 EXE/DLL/PDB/config/database/PFX files in the working diff.
- `git diff --check`: PASS.
- Production Center/Fighting/Road services were not modified or restarted by this promotion.
