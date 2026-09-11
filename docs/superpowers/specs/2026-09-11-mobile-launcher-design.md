# BaseGunnyII Desktop + Mobile Launcher Design

Date: 2026-09-11
Status: Approved design checkpoint
Branch: `feat/mobile-launcher-v1`
Base: `origin/master` at `52a5704`

## Goal

Upgrade the current Windows Gunny launcher with a polished login/register experience and add Android and iOS clients that can authenticate, register accounts, select a server, and launch the existing Gunny Flash client without changing the legacy game protocol.

The existing production-compatible login contract remains canonical:

1. `POST createLogin.ashx` with username/password.
2. Preserve the returned ASP.NET session cookies.
3. `GET LoginGame.aspx` without following redirects.
4. Parse the signed `user`, `key`, and `editby` values from the redirect.
5. Launch `flash/Loading.swf` with the existing `config.xml` and socket endpoint.

Registration remains compatible with the legacy CAPTCHA/session contract and does not bypass CAPTCHA.

## Repository and isolation

Implementation is performed in `E:\Gunny\BaseGunnyII-mobile-v1`, a clean worktree created from `origin/master`.

The existing `E:\Gunny\BaseGunnyII-launcher` worktree is intentionally left untouched because it contains uncommitted changes.

The feature will merge back to the repository's actual default development branch, `master`.

## Architecture

### Shared launcher core

`tools/GunnyLauncher/GunnyLauncher.Core` becomes the platform-neutral source of truth for authentication, registration, redirect parsing, server configuration, validation, and launch metadata.

It will expose separate services for login and registration rather than coupling HTTP/session behavior to the Windows process launcher. The core stays on `net8.0` and contains no WinForms, Android, iOS, or Ruffle-specific UI code.

A registration client will:

1. GET `auth/ValidateCode.aspx` and retain the session cookie.
2. Expose the CAPTCHA image bytes to the UI.
3. POST `auth/register.ashx` with username, password, repeated password, email, sex, and entered CAPTCHA code using the same session.
4. Treat response `ok` as success and return all other response text as a user-facing server validation error.

Passwords must exist only in memory for the active request and must never be written to logs, settings, crash reports, or disk.

### Runtime adapters

Platform-specific projects consume the shared core and translate `GameLaunchInfo` into the runtime each platform supports:

- Windows: desktop Ruffle executable.
- Android: native Ruffle Android integration pinned to a known revision.
- iOS: Ruffle Web/WASM hosted in `WKWebView` with a restricted WebSocket socket proxy.

No platform is allowed to invent a different account or login protocol.

## Windows launcher

The current functional WinForms launcher remains the Windows base. Its presentation is upgraded without changing the proven login/launch sequence.

The main window will provide:

- Gunny-branded header and clear server status.
- Login and Register views in the same launcher.
- Username, password, server selection, show/hide password, and optional remember-username behavior.
- Registration fields for username, password, confirmation, email, CAPTCHA image, CAPTCHA refresh, and submit.
- A prominent `Chơi ngay` action with disabled/loading state during authentication.
- Status/error text that distinguishes validation, HTTP/server, missing-runtime, and launch failures.

Only the username and selected server may be persisted. Passwords, CAPTCHA values, auth keys, and session cookies must not be persisted.

## Android client

Add `mobile/android` as an Android app with a login/register/server-selection shell and a game screen backed by the native Ruffle Android runtime.

The Android app must use the same login/registration endpoints and signed launch values as Windows. Raw game TCP connections are restricted to the configured Gunny game host and approved game ports, with `9200` as the initial default.

The Ruffle Android dependency/revision is pinned and documented. The build must not silently float to a nightly revision.

A successful APK build alone is insufficient: a smoke path must validate loading the deployed `Loading.swf`, reaching the configured game socket, and entering the initial game-loading flow without an immediate runtime failure.

## iOS client

Add `mobile/ios` as a SwiftUI shell containing the same login/register/server-selection flows and a `WKWebView` game surface.

Because iOS WebKit/WASM cannot make the legacy raw TCP connection directly, the game page uses Ruffle Web/WASM with `socketProxy` configured to a dedicated secure WebSocket gateway. The gateway is the only supported TCP bridge for iOS.

The iOS app passes only signed launch metadata to the game WebView. It must not embed database credentials, server secrets, or reusable privileged tokens.

The project must build on a GitHub Actions macOS runner for an iOS Simulator target without requiring signing. Device/App Store archives remain a release step requiring Apple signing credentials and are not falsely reported as complete without those credentials.

## Socket gateway

Add `services/GunnySocketProxy` as a .NET 8 WebSocket-to-TCP service for the iOS Ruffle client.

The service is intentionally not a general-purpose proxy. It has a static allowlist of permitted Gunny destinations, beginning with the configured production game host and port `9200`.

Controls include:

- Reject arbitrary host/port input.
- TLS/WSS at the deployment edge.
- Maximum connection lifetime and idle timeout.
- Per-client concurrent connection and connection-rate limits.
- Bounded frame/message sizes and backpressure.
- No logging of credentials, session cookies, auth keys, or game payload bodies.
- Health endpoint that reveals no secrets.

The proxy forwards bytes only; it does not reinterpret or modify the Gunny game protocol.

## Security and compatibility

The legacy backend remains authoritative for accounts and signed login data. New clients may harden transport and local storage behavior but must not change password hashing semantics or bypass server-side account checks.

Registration CAPTCHA remains server-generated and session-bound. The clients refresh it explicitly when requested or after a failed/consumed registration attempt.

All new network code uses bounded timeouts and cancellation. User-facing errors are sanitized while diagnostic logs record only non-secret endpoint/status information.

HTTP remains supported where required by the currently deployed legacy server, but the design keeps server base URLs configurable so HTTPS can be introduced without client rewrites.

## Testing

Shared core tests cover login request sequence, redirect parsing, cookie/session preservation, registration CAPTCHA session handling, registration form encoding, validation, cancellation, and secret non-persistence.

Windows tests cover launcher state transitions and Ruffle process command construction. Manual smoke validates actual login and game startup with the local packaged runtime.

Android verification includes Gradle build, unit tests, launcher/auth smoke, and a documented device/emulator game-loading smoke. iOS verification includes shared-core contract fixtures, Swift unit tests where practical, Simulator build, WebView launch-page validation, and socket-proxy integration tests.

Socket-proxy tests cover allowlist rejection, successful approved TCP bridging against a local fixture server, connection limits, timeout behavior, abnormal WebSocket closure, and bounded payload handling.

## CI and merge gate

GitHub Actions will run the platform-independent .NET tests/builds on every change. Windows builds run on Windows; Android builds on an Android-capable runner; iOS Simulator builds run on macOS.

The feature may merge to `master` only after applicable automated checks are green, the branch is up to date with `master`, `git diff --check` is clean, and the desktop/shared-core regression suite remains green.

## Delivery and acceptance criteria

The implementation is accepted when all of the following are true:

- Windows launcher has polished Login/Register flows and retains working game launch behavior.
- Registration works against the existing CAPTCHA/session endpoint without bypasses.
- Shared core contains no Windows/mobile UI dependencies and has regression coverage.
- Android project builds reproducibly with a pinned Ruffle revision and has a documented game-loading smoke result.
- iOS Simulator project builds, renders the launcher shell, and can construct the Ruffle Web launch page using the signed login result.
- Socket proxy accepts only approved Gunny destinations and passes its integration/security tests.
- CI covers desktop/shared core, Android, iOS Simulator, and socket proxy as far as repository-hosted runners permit.
- No passwords, CAPTCHA values, session cookies, or auth keys are committed or persisted by the clients.
- Feature branch is pushed, reviewed through a PR, and merged to `master` only after green checks.

## Explicit non-goals for this milestone

- Rewriting the Gunny Flash game into a native Unity/Unreal/mobile client.
- Replacing the legacy account database or password format.
- Bypassing CAPTCHA, authentication, licensing, signing, or server authorization.
- Publishing to Google Play or the Apple App Store without the required external developer credentials.
- Treating a successful compile as proof that all legacy Flash gameplay performs perfectly on mobile hardware.

Performance tuning for individual Flash scenes can follow after real-device profiling demonstrates a concrete bottleneck.