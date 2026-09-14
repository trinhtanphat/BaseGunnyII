# Recovered Runtime Source

This directory preserves C# source decompiled from the Gunny server binaries deployed on VPS 182. It is intentionally service-scoped because assemblies with the same filename can have different SHA256/content in Center, Fight, and Road.

## Safety and provenance

- Raw forensic output remains outside the repository at `C:\Gunny\_decompiled_20260914\RuntimeByService`.
- `manifest.json` records the deployed SHA256 for every service/assembly variant.
- Runtime `app.config` files are deliberately omitted because deployed configs can contain environment-specific endpoints, connection strings, and historical credentials.
- No EXE, DLL, PDB, database, build output, or deployed secret is stored here.
- This recovery lane is not automatically the canonical product source. Promotion into top-level projects requires Roslyn duplicate/type checks and a successful canonical build.

## Building the recovered projects

The generated projects reference deployed dependency binaries through the MSBuild property `RuntimeBinaryRoot`. On VPS 182 the verified value is `C:\Gunny\GunnyFileExe\SERVER`.

Legacy framework targeting assemblies are kept outside Git under `C:\Gunny\_work\refpacks\root`.

Example:

```powershell
dotnet build recovery\runtime-source\by-service\Road\Game.Server\Game.Server.csproj -c Release --nologo `
  -p:RuntimeBinaryRoot="C:\Gunny\GunnyFileExe\SERVER" `
  -p:TargetFrameworkRootPath="C:\Gunny\_work\refpacks\root\"
```

## Redaction policy

The exact raw forensic decompile remains outside Git under `C:\Gunny\_decompiled_20260914\RuntimeByService`.
Nine credential-like `m_password` string literals recovered from Road room-action classes are replaced in this Git lane with `<redacted-runtime-room-password>`.
Those files are listed in `evidence/redactions-20260915.json` and are excluded from automatic canonical promotion until manually reviewed.
