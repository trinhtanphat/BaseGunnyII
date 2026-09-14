# Redaction validation — 2026-09-15

- Nine credential-like `m_password` string literals in recovered Road room-action source were replaced with `<redacted-runtime-room-password>` before Git staging.
- Exact unredacted forensic source remains outside the repository under `C:\Gunny\_decompiled_20260914\RuntimeByService`.
- Post-redaction scan: 0 unredacted password-suffix string literals; 0 hardcoded VPS IP hits; 0 connection-string hits; 0 API-key/token/secret/private-key literal hits.
- `Road/Game.Server/Game.Server.csproj` restore+build after redaction: exit 0, 0 errors, 14 decompiler/source warnings.
- These redacted files must not be auto-promoted into canonical source without manual semantic review.
