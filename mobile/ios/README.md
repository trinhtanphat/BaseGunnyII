# Gunny Mobile iOS

The iOS client is a SwiftUI launcher with session-bound legacy Gunny login and registration, plus a WKWebView-hosted Ruffle player.

## Security and persistence

Only the server URL and username are persisted with `AppStorage`. Passwords, CAPTCHA values, cookies, signed launch keys, and game redirects remain in memory. `GunnySession` uses an ephemeral `URLSessionConfiguration` so ASP.NET session cookies survive CAPTCHA/login flows without being persisted to disk.

The current game server is plain HTTP at `103.9.156.182`; `project.yml` scopes the ATS insecure-load exception to that IP only. The raw game TCP socket is never opened by WebKit. Ruffle is configured to proxy only `103.9.156.182:9200` through a required `wss://.../socket?route=game` endpoint.

## Ruffle Web

`web/package.json` pins `@ruffle-rs/ruffle` to exact version `0.6.0`. Run:

```sh
cd mobile/ios/web
npm ci
npm run prepare:ruffle
```

The preparation script copies only the runtime JS/WASM files to `mobile/ios/Generated/ruffle` and writes a SHA-256 manifest. Generated assets and `node_modules` are intentionally ignored by Git.
## Project generation and tests

Install XcodeGen on macOS, prepare Ruffle assets, then run:

```sh
cd mobile/ios
xcodegen generate
xcodebuild -project GunnyMobile.xcodeproj -scheme GunnyMobile \
  -sdk iphonesimulator -destination 'platform=iOS Simulator,id=<UDID>' \
  test CODE_SIGNING_ALLOWED=NO
```

Set build setting `GUNNY_SOCKET_PROXY_URL` to the deployed secure gateway URL when testing gameplay. If it is absent or invalid, account login can succeed but the app deliberately refuses to open a broken game surface.

`GunnyMobileTests` validates the exported C# golden protocol fixture, signed redirect/SWF URL generation, and the single approved Ruffle socket-proxy mapping. Simulator compile/tests do not prove real-device gameplay; record real-device observations separately without credentials.
