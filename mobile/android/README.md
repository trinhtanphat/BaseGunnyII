# Gunny Mobile Android

This folder contains the native Gunny account launcher and the pinned Ruffle Android runtime contract.

## Gunny launcher

`GunnyMobile.Android` targets `net10.0-android` and references `GunnyLauncher.Core` (`net8.0`) for login and registration. The launcher does not duplicate the legacy account protocol.

The default server remains `http://103.9.156.181/Gunny/` for compatibility with the current deployment. Passwords, CAPTCHA values, cookies, signed auth keys and launch redirects are never persisted by the Android app.

After login the app creates an explicit `ACTION_VIEW` intent for the signed `flash/Loading.swf` URL, MIME `application/x-shockwave-flash`, constrained to package `rs.ruffle`. It does not fall back to a browser.

## Pinned Ruffle runtime

The exact upstream revision is stored in `ruffle-android.revision`:

`b4cf842e58a01b91f4ddb6dfa4c8ec7cb531bbb8`

Use `fetch-ruffle-android.ps1` on Windows or `fetch-ruffle-android.sh` on Unix. Both scripts fail if the checked-out HEAD differs from the pinned revision.

At this revision upstream uses compileSdk 36, targetSdk 35, minSdk 26 and package `rs.ruffle`. `PlayerActivity` accepts HTTP/HTTPS SWF VIEW intents and passes `intent.dataString` to the native player.

## Building pinned Ruffle from source

Upstream's pinned workflow uses Java 17, Android NDK `r27`, `cargo-ndk 4.1.2`, Rust targets for each ABI, and Android API level 26 for native compilation. Native libraries are built with the `jpegxr` feature, then Gradle assembles the release APK.

Typical upstream flow:

```text
rustup target add aarch64-linux-android
cargo install cargo-ndk@4.1.2 --locked
cargo ndk --target arm64-v8a --platform 26 -o jniLibs build --release --features jpegxr
./gradlew assembleRelease
```

Build all required ABIs or use upstream CI/release artifacts as appropriate. Do not silently switch to upstream `main` or a floating nightly.

## Verification

`GunnyMobile.Android.Tests` is platform-neutral and verifies the signed SWF launch descriptor. A successful launcher APK compile does not prove that the legacy Flash game performs correctly on a real device. Device/emulator observations belong in `SMOKE.md` and must not contain credentials.
