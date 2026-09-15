# Canonical baseline build — 2026-09-15

Base: `origin/master@88ee24ddff46629abde0fab198f0e4b53c866b26` before runtime-source promotion.

The default .NET SDK build initially failed with MSB3644 because VPS 182 did not have .NET Framework 3.5/4.0 targeting packs. No source changes were made to address that environment issue.

Reference assemblies were restored outside Git using NuGet packages `Microsoft.NETFramework.ReferenceAssemblies.net20`, `.net35`, and `.net40`, all version `1.0.3`, and exposed through `C:\Gunny\_work\refpacks\root`.

With `-p:TargetFrameworkRootPath=C:\Gunny\_work\refpacks\root\`:

- `CenterServer.sln`: PASS, 0 errors, 8 warnings.
- `FightingServer.sln`: PASS, 0 errors, 24 warnings.
- `GameServer.sln`: core projects compile; default solution build stops at legacy ClickOnce/PFX signing because .NET Core MSBuild does not support that PFX path.
- `GameServer.sln` with command-line `-p:SignManifests=false -p:SignAssembly=false`: PASS, 0 errors, 4 warnings.

The signing override is a verification-only command-line property; repository signing configuration remains unchanged.
