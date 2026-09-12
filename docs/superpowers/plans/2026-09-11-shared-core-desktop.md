# Shared Core + Windows Launcher Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add canonical registration/session contracts to `GunnyLauncher.Core` and upgrade the Windows launcher with Login/Register without regressing the proven Ruffle launch flow.

**Architecture:** Keep HTTP/account semantics in the platform-neutral `net8.0` core. WinForms binds to those services and persists only server URL and username. Existing `GunnyLauncherService` remains the Windows process adapter but receives authenticated launch metadata from the shared login client.

**Tech Stack:** C# 12, .NET 8, WinForms, `HttpClient`, existing executable smoke-test project.

**Spec:** `docs/superpowers/specs/2026-09-11-mobile-launcher-design.md`

## Global Constraints

- Preserve `POST createLogin.ashx` then `GET LoginGame.aspx` with redirects disabled.
- Registration uses `auth/ValidateCode.aspx` then `auth/register.ashx` with the same HTTP session.
- Do not persist password, CAPTCHA, cookies, auth keys, or game payloads.
- Keep `GunnyLauncher.Core` free of WinForms/mobile runtime dependencies.
- Default game port remains `9200`; server base URL remains configurable.

---

### Task 1: Registration HTTP Contract

**Files:**
- Create: `tools/GunnyLauncher/GunnyLauncher.Core/RegistrationCaptcha.cs`
- Create: `tools/GunnyLauncher/GunnyLauncher.Core/RegistrationResult.cs`
- Create: `tools/GunnyLauncher/GunnyLauncher.Core/GunnyRegistrationClient.cs`
- Modify: `tools/GunnyLauncher/GunnyLauncher.Tests/Program.cs`

**Interfaces:**
- Produces: `Task<RegistrationCaptcha> GetCaptchaAsync(CancellationToken)`.
- Produces: `Task<RegistrationResult> RegisterAsync(string username, string password, string confirmation, string email, string sex, string captchaCode, CancellationToken)`.
- [ ] **Step 1: Write failing registration smoke assertions**

```csharp
var registration = new GunnyRegistrationClient(gameBase, registrationHandler);
var captcha = await registration.GetCaptchaAsync(CancellationToken.None);
Require(captcha.ImageBytes.SequenceEqual(new byte[] { 1, 2, 3 }), "captcha bytes mismatch");
var result = await registration.RegisterAsync("newuser", "secret", "secret", "u@example.com", "1", "A1B2", CancellationToken.None);
Require(result.Success, "registration should accept ok");
Require(registrationHandler.Requests[0].Uri.AbsolutePath.EndsWith("/auth/ValidateCode.aspx"), "captcha endpoint mismatch");
Require(registrationHandler.Requests[1].Body!.Contains("username=newuser"), "registration username missing");
Require(!registrationHandler.Requests[1].Body!.Contains("other-secret-value"), "unexpected secret persistence");
```

- [ ] **Step 2: Run test and confirm RED**

Run: `dotnet run --project tools/GunnyLauncher/GunnyLauncher.Tests/GunnyLauncher.Tests.csproj -c Release`
Expected: compile failure because `GunnyRegistrationClient` and result types do not exist.

- [ ] **Step 3: Implement minimal registration client**

```csharp
public sealed record RegistrationCaptcha(byte[] ImageBytes, string ContentType);
public sealed record RegistrationResult(bool Success, string Message);
```

`GunnyRegistrationClient` owns one `HttpClient` for its lifetime, uses a `CookieContainer` when no test handler is supplied, GETs `auth/ValidateCode.aspx`, and POSTs exact legacy form fields `username`, `password`, `repassword`, `email`, `sex`, `validateCode`. Response text equal to `ok` maps to success; any other trimmed text maps to `RegistrationResult(false, text)`.

- [ ] **Step 4: Re-run smoke**

Run the same `dotnet run` command.
Expected: `GUNNY_LOGIN_SMOKE=PASS` plus `GUNNY_REGISTER_SMOKE=PASS`.

- [ ] **Step 5: Commit**

```bash
git add tools/GunnyLauncher/GunnyLauncher.Core tools/GunnyLauncher/GunnyLauncher.Tests/Program.cs
git commit -m "feat(launcher): add registration session client"
```

### Task 2: Shared Authentication/Launch Boundary

**Files:**
- Modify: `tools/GunnyLauncher/GunnyLauncher.Core/GunnyLauncherService.cs`
- Modify: `tools/GunnyLauncher/GunnyLauncher.Tests/Program.cs`

**Interfaces:**
- Produces: `Task<GameLaunchInfo> AuthenticateAsync(string username, string password, CancellationToken)`.
- Existing `BuildStartInfoAsync` remains for Windows compatibility and delegates to `AuthenticateAsync`.

- [ ] **Step 1: Add a failing assertion for metadata-only authentication**

```csharp
var launchOnly = await service.AuthenticateAsync("test user", "secret pass", CancellationToken.None);
Require(launchOnly.Key == "server-guid", "metadata auth key mismatch");
```

- [ ] **Step 2: Run smoke and confirm RED**

Expected: compile failure because `AuthenticateAsync` is missing.

- [ ] **Step 3: Add the minimal method and delegate from process construction**

```csharp
public Task<GameLaunchInfo> AuthenticateAsync(string username, string password, CancellationToken token) =>
    new GunnyLoginClient(_gameBase, _handler).AuthenticateAsync(username, password, token);
```

`BuildStartInfoAsync` awaits this method, then calls the existing Ruffle argument/process builders.

- [ ] **Step 4: Run smoke and build**

Run:
`dotnet run --project tools/GunnyLauncher/GunnyLauncher.Tests/GunnyLauncher.Tests.csproj -c Release`
`dotnet build tools/GunnyLauncher/GunnyLauncher.slnx -c Release --no-restore`
Expected: both PASS with zero build errors.

- [ ] **Step 5: Commit**

```bash
git add tools/GunnyLauncher/GunnyLauncher.Core/GunnyLauncherService.cs tools/GunnyLauncher/GunnyLauncher.Tests/Program.cs
git commit -m "refactor(launcher): expose shared launch metadata auth"
```

### Task 3: Windows Login/Register State and Persistence

**Files:**
- Create: `tools/GunnyLauncher/GunnyLauncher.App/LauncherSettings.cs`
- Create: `tools/GunnyLauncher/GunnyLauncher.App/LauncherSettingsStore.cs`
- Modify: `tools/GunnyLauncher/GunnyLauncher.App/Form1.cs`
- Modify: `tools/GunnyLauncher/GunnyLauncher.Tests/Program.cs`

**Interfaces:**
- `LauncherSettings` contains only `ServerUrl` and `Username`.
- `LauncherSettingsStore.Load()` and `.Save(LauncherSettings)` read/write `%LOCALAPPDATA%/BaseGunnyII/launcher.json`.

- [ ] **Step 1: Add source-level persistence guard smoke**

```csharp
var settingsJson = JsonSerializer.Serialize(new { ServerUrl = gameBase.ToString(), Username = "newuser" });
Require(!settingsJson.Contains("password", StringComparison.OrdinalIgnoreCase), "settings must not persist password");
Require(!settingsJson.Contains("captcha", StringComparison.OrdinalIgnoreCase), "settings must not persist captcha");
```

- [ ] **Step 2: Run smoke to establish baseline**

Expected: current smoke remains green before UI changes.

- [ ] **Step 3: Implement polished two-view WinForms layout**

Use a top branded panel, server row, `TabControl` with `Đăng nhập` and `Đăng ký`, show/hide password checkboxes, login loading state, registration CAPTCHA `PictureBox`, refresh button, and sanitized status label. Registration calls `GunnyRegistrationClient.GetCaptchaAsync` and `RegisterAsync`; login keeps the existing `GunnyLauncherService.BuildStartInfoAsync` and process launch.

- [ ] **Step 4: Implement safe settings**

Persist only server URL and username when remember-username is checked. Clear login/registration password controls after success and never serialize response redirect/auth keys.

- [ ] **Step 5: Build and manually launch the form**

Run:
`dotnet build tools/GunnyLauncher/GunnyLauncher.slnx -c Release`
`dotnet run --project tools/GunnyLauncher/GunnyLauncher.App/GunnyLauncher.App.csproj -c Release`
Expected: window renders both tabs, server field, password toggles, CAPTCHA controls, and disabled/loading button behavior.

- [ ] **Step 6: Commit**

```bash
git add tools/GunnyLauncher/GunnyLauncher.App tools/GunnyLauncher/GunnyLauncher.Tests
git commit -m "feat(launcher): add polished login and registration UI"
```

### Task 4: Desktop Regression Gate

- [ ] Run `dotnet run --project tools/GunnyLauncher/GunnyLauncher.Tests/GunnyLauncher.Tests.csproj -c Release` and require all smoke markers PASS.
- [ ] Run `dotnet build tools/GunnyLauncher/GunnyLauncher.slnx -c Release --no-restore` and require zero errors.
- [ ] Run `git diff --check` and require no output.
- [ ] Commit only if verification creates required tracked updates; otherwise leave verification uncommitted.
