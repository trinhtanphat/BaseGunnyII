# SqlDataProvider safe additive promotion — 2026-09-15

- Baseline runtime-only candidates: 60 types / 60 files.
- Promoted to canonical `SqlDataProvider`: 59 files.
- Post-promotion Roslyn classification: 59 `canonical-present`, 1 `runtime-only`.
- Deferred type: `SqlDataProvider.Data.UserFarmInfo`.
- Blocker: `UserFarmInfo` requires `UserFieldInfo`, while `UserFieldInfo` is a runtime `variant-conflict`; no variant was selected automatically.
- Canonical `.csproj` received exactly 59 new explicit `<Compile Include>` entries.
- `git diff --check -- SqlDataProvider`: PASS.
- Roslyn compile-surface gate: 148 project sources, PASS with 0 compile errors against locally available .NET Framework v4 runtime assemblies plus `Lib/log4net.dll`.
- Exact .NET Framework 3.5 MSBuild gate is environment-blocked on VPS 182 (`MSB3644` / missing reference assemblies), so no false claim of a native net35 build is made.

Full semantic evidence: `canonical-diff-post-sql-safe-wave.json` and `.md`.
