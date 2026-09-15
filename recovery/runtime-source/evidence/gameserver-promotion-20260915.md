# Game.Server runtime promotion — 2026-09-15

Canonical branch: `recovery/runtime-source-20260915`
Source variant: deployed Road `Game.Server` recovery lane.

- Initial runtime-only candidates: 170 types / 170 files.
- Credential-focused scan: 0 candidate files with password/pwd literal assignment or connection-string credential patterns.
- Destination collisions: 0.
- Syntax normalized only for C# 7.3 compatibility (file-scoped namespace to block namespace).
- Build round 1: 7 candidate files blocked by missing/variant runtime DTOs; 0 canonical source files errored.
- Build round 2: 94 additional candidate files blocked by runtime member/model drift; 0 canonical source files errored.
- Build round 3: `Achievements/BaseCondition.cs` blocked because `BaseAchievement` was deferred.
- Final dependency-closed subset: 68 files.
- Final build: `Game.Server/Game.Server.csproj`, Release, .NET Framework reference-pack root, exit 0.
- Final build error lines: 0; warnings: 3.
- Final project mapping: 68/68 new files have explicit `<Compile Include>` entries.
- `git diff --check`: PASS.
- Binary/config/database/signing artifacts in wave: 0.

Deferred files remain available in the service-scoped forensic recovery lane. No canonical core model or method-only delta was overwritten to make this wave compile.
