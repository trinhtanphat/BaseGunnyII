# Gunny Desktop Launcher

Windows desktop launcher for the legacy Gunny web client using the Ruffle desktop runtime instead of a browser Flash plugin.

## Runtime contract

- Gunny base URL defaults to `http://103.9.156.182/Gunny/`.
- Authentication reuses the existing `createLogin.ashx` session flow.
- `LoginGame.aspx` supplies the signed `user` / `key` redirect.
- Ruffle loads `flash/Loading.swf` and the deployed `config.xml`.
- Ruffle TCP connections are enabled so the legacy client can reach the Road server; the expected game endpoint remains `<server-host>:9200`.
- Passwords are not written to disk by the launcher.

## Build and smoke

```powershell
dotnet run --project .\GunnyLauncher.Tests\GunnyLauncher.Tests.csproj -c Release
dotnet build .\GunnyLauncher.slnx -c Release
```

## Package layout

Publish `GunnyLauncher.App`, then place the verified Ruffle Windows x64 executable at `runtime\ruffle.exe`. Keep Ruffle's license and README beside the executable. The repository intentionally does not vendor the third-party Ruffle binary.