import SwiftUI
import UIKit

struct LauncherView: View {
    private struct GameContext: Identifiable {
        let id = UUID()
        let launch: LaunchInfo
        let gameBase: URL
        let proxyURL: URL
    }
    enum Mode: String, CaseIterable, Identifiable {
        case login = "Đăng nhập"
        case register = "Đăng ký"
        var id: String { rawValue }
    }

    @AppStorage("gunny.server") private var server = "http://103.9.156.182/Gunny/"
    @AppStorage("gunny.username") private var username = ""

    @State private var mode: Mode = .login
    @State private var password = ""
    @State private var showLoginPassword = false
    @State private var registerUsername = ""
    @State private var registerPassword = ""
    @State private var registerConfirmation = ""
    @State private var registerEmail = ""
    @State private var showRegisterPassword = false
    @State private var captchaCode = ""
    @State private var captchaData: Data?
    @State private var status = "Sẵn sàng."
    @State private var busy = false
    @State private var registrationSession: GunnySession?
    @State private var gameContext: GameContext?

    var body: some View {
        NavigationStack {
            ScrollView {
                VStack(spacing: 18) {
                    Text("GUNNY")
                        .font(.system(size: 38, weight: .heavy, design: .rounded))
                    TextField("Máy chủ", text: $server)
                        .textInputAutocapitalization(.never)
                        .autocorrectionDisabled()
                        .textFieldStyle(.roundedBorder)

                    Picker("Chế độ", selection: $mode) {
                        ForEach(Mode.allCases) { item in
                            Text(item.rawValue).tag(item)
                        }
                    }
                    .pickerStyle(.segmented)

                    if mode == .login {
                        loginForm
                    } else {
                        registerForm
                    }

                    Text(status)
                        .font(.footnote)
                        .foregroundStyle(.secondary)
                        .frame(maxWidth: .infinity, alignment: .leading)
                }
                .padding(24)
            }
            .navigationTitle("Gunny Mobile")
            .onChange(of: mode) { newMode in
                guard newMode == .register, captchaData == nil else { return }
                Task { await refreshCaptcha() }
            }
        }
        .fullScreenCover(item: $gameContext) { context in
            ZStack(alignment: .topTrailing) {
                GameWebView(
                    launch: context.launch,
                    gameBase: context.gameBase,
                    proxyURL: context.proxyURL
                )
                .ignoresSafeArea()

                Button("Đóng") {
                    gameContext = nil
                }
                .buttonStyle(.borderedProminent)
                .padding()
            }
        }
    }

    private var loginForm: some View {
        VStack(spacing: 12) {
            TextField("Tài khoản", text: $username)
                .textInputAutocapitalization(.never)
                .autocorrectionDisabled()
                .textFieldStyle(.roundedBorder)

            Group {
                if showLoginPassword {
                    TextField("Mật khẩu", text: $password)
                } else {
                    SecureField("Mật khẩu", text: $password)
                }
            }
            .textFieldStyle(.roundedBorder)

            Toggle("Hiện mật khẩu", isOn: $showLoginPassword)
            Button("CHƠI NGAY") {
                Task { await login() }
            }
            .buttonStyle(.borderedProminent)
            .disabled(busy)
        }
    }

    private var registerForm: some View {
        VStack(spacing: 12) {
            TextField("Tài khoản", text: $registerUsername)
                .textInputAutocapitalization(.never)
                .autocorrectionDisabled()
                .textFieldStyle(.roundedBorder)

            Group {
                if showRegisterPassword {
                    TextField("Mật khẩu", text: $registerPassword)
                    TextField("Nhập lại mật khẩu", text: $registerConfirmation)
                } else {
                    SecureField("Mật khẩu", text: $registerPassword)
                    SecureField("Nhập lại mật khẩu", text: $registerConfirmation)
                }
            }
            .textFieldStyle(.roundedBorder)

            Toggle("Hiện mật khẩu", isOn: $showRegisterPassword)
            TextField("Email", text: $registerEmail)
                .textInputAutocapitalization(.never)
                .keyboardType(.emailAddress)
                .textFieldStyle(.roundedBorder)

            captchaView
            TextField("Mã CAPTCHA", text: $captchaCode)
                .textInputAutocapitalization(.characters)
                .autocorrectionDisabled()
                .textFieldStyle(.roundedBorder)

            HStack {
                Button("Làm mới CAPTCHA") {
                    Task { await refreshCaptcha() }
                }
                .buttonStyle(.bordered)

                Button("TẠO TÀI KHOẢN") {
                    Task { await register() }
                }
                .buttonStyle(.borderedProminent)
            }
            .disabled(busy)
        }
    }

    @ViewBuilder
    private var captchaView: some View {
        if let captchaData,
           let image = UIImage(data: captchaData) {
            Image(uiImage: image)
                .resizable()
                .scaledToFit()
                .frame(maxHeight: 80)
                .accessibilityLabel("CAPTCHA")
        } else {
            RoundedRectangle(cornerRadius: 8)
                .fill(.quaternary)
                .frame(height: 80)
                .overlay(Text("Chưa tải CAPTCHA"))
        }
    }

    @MainActor
    private func login() async {
        busy = true
        status = "Đang đăng nhập..."
        defer {
            password = ""
            busy = false
        }
        do {
            let session = try makeSession()
            let launch = try await session.authenticate(
                username: username.trimmingCharacters(in: .whitespacesAndNewlines),
                password: password
            )
            guard let proxyURL = AppConfiguration.socketProxyURL else {
                gameContext = nil
                status = "Đăng nhập thành công nhưng chưa cấu hình WSS socket gateway."
                return
            }
            _ = try GamePageBuilder.makeBootstrapScript(proxyURL: proxyURL)
            gameContext = GameContext(
                launch: launch,
                gameBase: session.gameBase,
                proxyURL: proxyURL
            )
            status = "Đăng nhập thành công. Đang mở game..."
        } catch {
            gameContext = nil
            status = userFacing(error)
        }
    }

    @MainActor
    private func refreshCaptcha() async {
        busy = true
        status = "Đang tải CAPTCHA..."
        defer { busy = false }
        do {
            let base = try normalizedBase()
            let session: GunnySession
            if let existing = registrationSession, existing.gameBase == base {
                session = existing
            } else {
                session = try GunnySession(gameBase: base)
                registrationSession = session
            }
            try await loadCaptcha(using: session)
        } catch {
            captchaData = nil
            status = userFacing(error)
        }
    }

    @MainActor
    private func register() async {
        busy = true
        status = "Đang tạo tài khoản..."
        defer {
            registerPassword = ""
            registerConfirmation = ""
            busy = false
        }
        do {
            let base = try normalizedBase()
            guard let session = registrationSession,
                  session.gameBase == base,
                  captchaData != nil else {
                status = "Hãy tải CAPTCHA trước khi đăng ký."
                busy = false
                await refreshCaptcha()
                return
            }

            let result = try await session.register(
                username: registerUsername.trimmingCharacters(in: .whitespacesAndNewlines),
                password: registerPassword,
                confirmation: registerConfirmation,
                email: registerEmail.trimmingCharacters(in: .whitespacesAndNewlines),
                captchaCode: captchaCode.trimmingCharacters(in: .whitespacesAndNewlines)
            )

            if result.success {
                username = registerUsername.trimmingCharacters(in: .whitespacesAndNewlines)
                captchaData = nil
                captchaCode = ""
                mode = .login
                status = "Đăng ký thành công. Bạn có thể đăng nhập."
            } else {
                status = result.message.isEmpty ? "Đăng ký thất bại." : result.message
                try await loadCaptcha(using: session)
            }
        } catch {
            status = userFacing(error)
        }
    }

    @MainActor
    private func loadCaptcha(using session: GunnySession) async throws {
        captchaData = try await session.fetchCaptcha()
        captchaCode = ""
        status = "CAPTCHA đã sẵn sàng."
    }

    private func normalizedBase() throws -> URL {
        guard let candidate = URL(string: server.trimmingCharacters(in: .whitespacesAndNewlines)) else {
            throw GunnySession.SessionError.invalidServer
        }
        return try GunnySession(gameBase: candidate).gameBase
    }

    private func makeSession() throws -> GunnySession {
        try GunnySession(gameBase: normalizedBase())
    }

    private func userFacing(_ error: Error) -> String {
        if let localized = error as? LocalizedError,
           let description = localized.errorDescription,
           !description.isEmpty {
            return description
        }
        return "Có lỗi kết nối tới máy chủ Gunny."
    }
}
