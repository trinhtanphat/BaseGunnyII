# Gunny Android Client Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a .NET Android Gunny login/register launcher that reuses `GunnyLauncher.Core` and launches the signed `Loading.swf` in the pinned native Ruffle Android runtime.

**Architecture:** The Gunny app targets `net8.0-android`, references the shared core directly, and owns account/server UI. Native Flash execution is delegated through an explicit Android VIEW intent to the companion Ruffle runtime package `rs.ruffle`; the runtime source is always built from pinned revision `b4cf842e58a01b91f4ddb6dfa4c8ec7cb531bbb8` (minSdk 26, targetSdk 35) rather than a floating nightly.

**Tech Stack:** C#/.NET 8 for Android, Android SDK API 35, Java 17/Rust/NDK 27 for pinned Ruffle build.

**Spec:** `docs/superpowers/specs/2026-09-11-mobile-launcher-design.md`

## Global Constraints

- Reuse `GunnyLauncher.Core` for login and registration; do not duplicate account protocol in Kotlin/Java.
- Ruffle package is exactly `rs.ruffle`; its exported `PlayerActivity` accepts `http`/`https` SWF VIEW intents at the pinned revision.
- Do not persist password, CAPTCHA, cookies, or auth keys.
- Initial game socket remains TCP `9200`; Ruffle itself owns the native TCP connection.
- An APK compile is not claimed as gameplay proof without an emulator/device smoke.

---

### Task 1: Android Shell and Shared-Core Reference

**Files:**
- Create: `mobile/android/GunnyMobile.Android/GunnyMobile.Android.csproj`
- Create: `mobile/android/GunnyMobile.Android/MainActivity.cs`
- Create: `mobile/android/GunnyMobile.Android/AndroidManifest.xml`
- Create: `mobile/android/GunnyMobile.Android.Tests/GunnyMobile.Android.Tests.csproj`
- Create: `mobile/android/GunnyMobile.Android.Tests/Program.cs`

**Interfaces:**
- Android project references `../../../tools/GunnyLauncher/GunnyLauncher.Core/GunnyLauncher.Core.csproj`.
- `MainActivity` exposes Login/Register tabs and configurable server URL.
- [ ] **Step 1: Add failing platform-neutral Android contract test**

```csharp
Require(RuffleAndroidContract.PackageName == "rs.ruffle", "Ruffle package mismatch");
var uri = RuffleAndroidContract.BuildGameUri(launch, gameBase);
Require(uri.AbsolutePath.EndsWith("/flash/Loading.swf"), "Android SWF path mismatch");
Require(uri.Query.Contains("key=abc-123"), "signed key missing");
```

- [ ] **Step 2: Run test and confirm RED**

Run: `dotnet run --project mobile/android/GunnyMobile.Android.Tests/GunnyMobile.Android.Tests.csproj -c Release`
Expected: compile failure because `RuffleAndroidContract` does not exist.

- [ ] **Step 3: Add `RuffleAndroidContract` in shared core**

Create `tools/GunnyLauncher/GunnyLauncher.Core/RuffleAndroidContract.cs` with constant package `rs.ruffle` and `BuildGameUri` delegating to `GameLaunchInfo.BuildSwfUri`.

- [ ] **Step 4: Build Android shell**

`MainActivity` constructs the login/register form programmatically, calls `GunnyLoginClient`/`GunnyRegistrationClient`, clears password fields after use, refreshes CAPTCHA after registration attempts, and never writes auth metadata to preferences.

- [ ] **Step 5: Run contract test**

Expected: `GUNNY_ANDROID_CONTRACT_SMOKE=PASS`.

- [ ] **Step 6: Commit**

```bash
git add mobile/android tools/GunnyLauncher/GunnyLauncher.Core/RuffleAndroidContract.cs
git commit -m "feat(android): add shared-core Gunny launcher shell"
```

### Task 2: Pinned Native Ruffle Runtime

**Files:**
- Create: `mobile/android/ruffle-android.revision`
- Create: `mobile/android/fetch-ruffle-android.ps1`
- Create: `mobile/android/fetch-ruffle-android.sh`
- Create: `mobile/android/README.md`

- [ ] **Step 1: Write exact revision file**

Content is exactly `b4cf842e58a01b91f4ddb6dfa4c8ec7cb531bbb8` followed by newline.

- [ ] **Step 2: Implement deterministic fetch scripts**

Scripts clone `https://github.com/ruffle-rs/ruffle-android.git`, fetch the exact revision, detach checkout it, and fail if `git rev-parse HEAD` differs. They never checkout upstream `main` as the build input.

- [ ] **Step 3: Document upstream runtime build**

Use Java 17, NDK `r27`, `cargo-ndk 4.1.2`, Android API 26+ and upstream `./gradlew assembleRelease`, matching the pinned upstream workflow.

- [ ] **Step 4: Commit**

```bash
git add mobile/android
git commit -m "build(android): pin native Ruffle runtime revision"
```

### Task 3: Explicit Ruffle Launch Adapter

**Files:**
- Create: `mobile/android/GunnyMobile.Android/RuffleIntentLauncher.cs`
- Modify: `mobile/android/GunnyMobile.Android/MainActivity.cs`
- Modify: `mobile/android/GunnyMobile.Android.Tests/Program.cs`

**Interfaces:**
- `bool IsRuntimeInstalled(Context context)` checks package `rs.ruffle`.
- `void Launch(Context context, GameLaunchInfo launch, Uri gameBase)` creates `ACTION_VIEW` for the signed SWF URI and constrains resolution to package `rs.ruffle`.

- [ ] **Step 1: Add failing launch-contract assertions**

Assert package, action semantics, SWF URL and signed query values through a platform-neutral descriptor factory before Android `Intent` construction.

- [ ] **Step 2: Implement launch adapter**

Create the intent with `Intent.ActionView`, parsed SWF URI, MIME `application/x-shockwave-flash`, `SetPackage("rs.ruffle")`, and `ActivityFlags.NewTask` only when launching outside an Activity context. If runtime is missing, show a sanitized install/runtime message instead of falling back to a browser.

- [ ] **Step 3: Build and run tests**

Run:
`dotnet workload list`
`dotnet run --project mobile/android/GunnyMobile.Android.Tests/GunnyMobile.Android.Tests.csproj -c Release`
`dotnet build mobile/android/GunnyMobile.Android/GunnyMobile.Android.csproj -c Release`
Expected: contract smoke PASS and Android build PASS when Android workload is installed.

- [ ] **Step 4: Emulator/device smoke**

Install the pinned Ruffle APK and Gunny APK, authenticate with a test account, assert Android resolves the SWF intent to `rs.ruffle.PlayerActivity`, `Loading.swf` renders, and the runtime attempts the configured game socket without immediate crash. Record observed result in `mobile/android/SMOKE.md` with device/API/runtime revision but no credentials.

- [ ] **Step 5: Commit**

```bash
git add mobile/android
git commit -m "feat(android): launch Gunny through pinned native Ruffle"
```

### Task 4: Android Regression Gate

- [ ] Shared core smoke PASS.
- [ ] Android contract smoke PASS.
- [ ] Android Release build PASS where workload is available.
- [ ] `git diff --check` produces no output.
- [ ] `SMOKE.md` distinguishes automated build evidence from real emulator/device gameplay evidence.
