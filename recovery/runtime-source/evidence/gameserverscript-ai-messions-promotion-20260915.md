# GameServerScript AI/Messions promotion — 2026-09-15

Canonical branch: `recovery/runtime-source-20260915`
Source variant: deployed Road `GameServerScripts` recovery lane.

- Initial runtime-only candidates: 196 types / 196 files.
- Destination collisions: 0; credential-focused scan: 0 hits.
- File-scoped namespaces converted to C# 7.3 block namespaces: 196.
- Build round 1: 700 errors in 191 candidate files; 0 canonical files errored.
- Dominant blockers: runtime overload/member drift (`CS1501`, `CS1061`).
- Blocked candidate files deferred: 191.
- Final dependency-closed subset: 5 files (`CHM1274`, `CHM1275`, `CNM1174`, `CSM1074`, `CTM1374`).
- Final qualification build: PASS, 0 compiler errors.
- Build mode: Release / .NET Framework 3.5 reference pack / `PostBuildEvent=`.
- Explicit `<Compile Include>` mapping: 5/5.
- `git diff --check`: PASS.

Deferred files remain in the forensic recovery lane; no canonical runtime overload or core model was changed to make them compile.
