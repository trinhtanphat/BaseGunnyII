# Android smoke evidence

Verified locally on Windows 11, 2026-09-12.

- .NET SDK: 10.0.401 with Android workload 36.1.2.
- Android SDK: `E:\Android\Sdk`, API 36 / build-tools 36.0.0.
- Java: Temurin 17.0.19.
- Minimum Android API: 26.
- Default device RID `android-arm64`: Release build PASS, 0 warnings / 0 errors.
- Emulator RID `android-x64`: Release build PASS, 0 warnings / 0 errors.
- arm64 signed APK: 4,386,236 bytes, SHA256 `7972f1210ac6b1479ee2faf846112287c86a0b652bf8427c447281699e094966`.
- x64 signed APK: 4,595,024 bytes, SHA256 `d1eb1d09a1d9f4c96364cae57f7d09ac64873b56e4b760081423c11bd8fc1722`.
- APK signature verification: v2 PASS and v3 PASS for both checked builds.
- Manifest check: package `com.trinhtanphat.gunnymobile`, minSdk 26, targetSdk 36, INTERNET permission, launchable `MainActivity`.
- Shared login/register smoke PASS.
- Android launch-contract smoke PASS for `rs.ruffle` and signed `Loading.swf` URL.

`adb devices -l` reported no connected device/emulator during this verification. Therefore this file does **not** claim gameplay, touch input, Ruffle rendering, or game-socket success on a real Android runtime yet. Those require the pinned Ruffle APK plus an emulator/device smoke.