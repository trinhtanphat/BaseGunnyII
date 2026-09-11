# Gunny iOS + CI Integration Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a SwiftUI iOS launcher with Ruffle Web/WASM over the restricted socket gateway, plus cross-platform CI that proves the repository-hosted build/test gates before merge.

**Architecture:** `GunnyLauncher.Core` exports golden protocol fixtures that native Swift code must match. SwiftUI performs legacy auth/register with one cookie-backed `URLSession`, renders Ruffle self-hosted `0.6.0` in `WKWebView`, and configures Ruffle `socketProxy` for game host `103.9.156.182`, port `9200`, through the dedicated gateway route. XcodeGen creates the Xcode project deterministically on macOS CI.

**Tech Stack:** Swift 6/SwiftUI, WKWebView, XcodeGen, Node/npm for `@ruffle-rs/ruffle` 0.6.0 self-hosted assets, GitHub Actions, .NET 8.

**Spec:** `docs/superpowers/specs/2026-09-11-mobile-launcher-design.md`

## Global Constraints

- Swift iOS code must conform to checked-in golden fixtures emitted from `GunnyLauncher.Core`; it must not invent alternate endpoint/form names.
- Login preserves cookies and intercepts the `LoginGame.aspx` redirect without following it.
- Registration preserves the CAPTCHA session and never bypasses CAPTCHA.
- Ruffle config uses lowercase `socketProxy` with exact `host`, `port`, and `proxyUrl` fields.
- App signing/App Store publication is out of scope; CI builds an unsigned Simulator target.

---

### Task 1: Golden Cross-Platform Contract Fixtures

**Files:**
- Create: `tools/GunnyLauncher/GunnyLauncher.ContractExport/GunnyLauncher.ContractExport.csproj`
- Create: `tools/GunnyLauncher/GunnyLauncher.ContractExport/Program.cs`
- Create: `mobile/contracts/gunny-launch-contract.json`

**Interfaces:**
- Exported JSON contains login endpoint/method, registration CAPTCHA/register endpoints, exact registration field names, sample redirect, parsed signed metadata, SWF URL, Ruffle Android package, game host, and game port.

- [ ] **Step 1: Create exporter referencing `GunnyLauncher.Core`**

The exporter builds a deterministic sample `GameLaunchInfo` from `http://103.9.156.182/Gunny/Default.aspx?user=fixture&key=fixture-key&editby=Trminhpc` and serializes canonical contract values with stable indentation/order.

- [ ] **Step 2: Generate fixture twice and verify byte stability**

Run exporter twice to separate temp files and compare SHA256; hashes must match.

- [ ] **Step 3: Commit exporter and fixture**

```bash
git add tools/GunnyLauncher/GunnyLauncher.ContractExport mobile/contracts
git commit -m "test(mobile): export canonical launcher contract fixtures"
```

### Task 2: SwiftUI Login/Register Shell

**Files:**
- Create: `mobile/ios/project.yml`
- Create: `mobile/ios/GunnyMobile/App/GunnyMobileApp.swift`
- Create: `mobile/ios/GunnyMobile/App/LauncherView.swift`
- Create: `mobile/ios/GunnyMobile/Networking/GunnySession.swift`
- Create: `mobile/ios/GunnyMobile/Model/LaunchInfo.swift`
- Create: `mobile/ios/GunnyMobileTests/ContractTests.swift`

**Interfaces:**
- `GunnySession.authenticate(username:password:) async throws -> LaunchInfo`.
- `GunnySession.fetchCaptcha() async throws -> Data`.
- `GunnySession.register(...) async throws -> RegistrationResult`.

- [ ] **Step 1: Write contract tests against `mobile/contracts/gunny-launch-contract.json`**

Assert endpoint paths, POST field names, parsed fixture redirect values, generated `Loading.swf` URL, and port `9200` match the fixture exactly.

- [ ] **Step 2: Implement cookie-backed URLSession**

Use one `URLSessionConfiguration.default` with `httpCookieStorage = .shared`. A delegate intercepts redirects for `LoginGame.aspx` by passing `nil` to the redirection completion handler and records the `Location` header for parsing.

- [ ] **Step 3: Implement SwiftUI views**

Create server selector, Login/Register segmented flow, secure fields, show-password controls, CAPTCHA image/refresh, disabled progress actions, and sanitized status errors. Store only username/server through `AppStorage`; do not store password, cookies, CAPTCHA or signed launch info.

- [ ] **Step 4: Generate and build project on macOS**

Run:
`xcodegen generate --spec mobile/ios/project.yml`
`xcodebuild -project mobile/ios/GunnyMobile.xcodeproj -scheme GunnyMobile -sdk iphonesimulator -configuration Debug CODE_SIGNING_ALLOWED=NO build`
Expected: Simulator build PASS.

- [ ] **Step 5: Commit**

```bash
git add mobile/ios
git commit -m "feat(ios): add SwiftUI Gunny login and registration shell"
```

### Task 3: Ruffle Web Game Surface

**Files:**
- Create: `mobile/ios/web/package.json`
- Create: `mobile/ios/web/package-lock.json`
- Create: `mobile/ios/web/game.html`
- Create: `mobile/ios/GunnyMobile/Game/GameWebView.swift`
- Create: `mobile/ios/GunnyMobile/Game/GamePageBuilder.swift`
- Modify: `mobile/ios/GunnyMobileTests/ContractTests.swift`

**Interfaces:**
- `GamePageBuilder.makeHTML(launch:gameBase:proxyBase:) -> String` produces a page whose Ruffle config contains one approved socket mapping.

- [ ] **Step 1: Pin self-hosted Ruffle**

`package.json` pins `@ruffle-rs/ruffle` to exact version `0.6.0`; commit the generated lockfile. Copy the package's self-hosted JS/WASM artifacts into the app bundle during XcodeGen pre-build scripting, never from a floating CDN/nightly URL.

- [ ] **Step 2: Add failing page-builder assertions**

Assert generated HTML contains the signed SWF URL and exactly:

```javascript
socketProxy: [{ host: "103.9.156.182", port: 9200, proxyUrl: "wss://proxy.example.test/socket?route=game" }]
```

The production gateway hostname is an app setting/build configuration value; tests use `wss://proxy.example.test/socket?route=game`.

- [ ] **Step 3: Implement `WKWebView` surface**

Bundle `game.html` + pinned Ruffle assets, enable JavaScript, disallow arbitrary navigation outside the configured game/server origins, inject only URL-encoded launch metadata, and load the local page with access to its asset directory.

- [ ] **Step 4: Run Swift tests and Simulator build**

Expected: contract/page tests PASS and `xcodebuild` Simulator target PASS.

- [ ] **Step 5: Commit**

```bash
git add mobile/ios
git commit -m "feat(ios): host Gunny in Ruffle Web with socket proxy"
```

### Task 4: Cross-Platform GitHub Actions

**Files:**
- Create: `.github/workflows/mobile-launcher.yml`
- Modify: `mobile/android/README.md`
- Create: `mobile/ios/README.md`

- [ ] **Step 1: Add Windows/.NET job**

Checkout, setup .NET 8, restore/build `tools/GunnyLauncher/GunnyLauncher.slnx`, run launcher smoke, build/run socket-proxy smoke, regenerate golden contract fixture and fail on diff.

- [ ] **Step 2: Add Android job**

Use Ubuntu, Java 17, .NET 8 Android workload and Android SDK; run Android contract smoke and build `GunnyMobile.Android.csproj`. Add a separate pinned-Ruffle job using NDK r27, Rust targets, `cargo-ndk 4.1.2`, exact Ruffle revision file, and upstream Gradle release build.

- [ ] **Step 3: Add iOS Simulator job**

Use `macos-latest`, install XcodeGen and Node, `npm ci` under `mobile/ios/web`, generate project, run Swift tests, and build with `CODE_SIGNING_ALLOWED=NO`.

- [ ] **Step 4: Add repository hygiene job**

Run `git diff --check`, reject committed password/CAPTCHA/cookie/auth-key fixtures, and verify `mobile/android/ruffle-android.revision` is exactly the pinned SHA.

- [ ] **Step 5: Commit**

```bash
git add .github mobile/android/README.md mobile/ios/README.md
git commit -m "ci: validate desktop and Gunny mobile launchers"
```

### Task 5: Final Integration and Merge Gate

- [ ] Run all local Windows-capable .NET tests/builds and `git diff --check`.
- [ ] Push `feat/mobile-launcher-v1` normally; do not force-push.
- [ ] Open a PR targeting repository default branch `master` with desktop/core, proxy, Android, iOS, runtime pin, and verification evidence summarized separately.
- [ ] Wait for GitHub-hosted Android/iOS jobs to establish platform build evidence; do not claim iOS device/App Store or Android real-device gameplay from CI alone.
- [ ] If CI fails, reproduce and patch on the feature branch with a focused RED→GREEN commit, then re-run checks.
- [ ] Merge only when branch is current with `master`, required applicable checks are green, and there are no unresolved review blockers.
- [ ] After merge, verify `master` contains the merge commit and the feature commits remain reachable.
