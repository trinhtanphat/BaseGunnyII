import Foundation

final class GunnySession: NSObject, URLSessionTaskDelegate, @unchecked Sendable {
    enum SessionError: LocalizedError {
        case invalidServer
        case invalidCredentials
        case invalidResponse
        case missingRedirect

        var errorDescription: String? {
            switch self {
            case .invalidServer: return "Địa chỉ máy chủ không hợp lệ."
            case .invalidCredentials: return "Tài khoản hoặc mật khẩu không đúng."
            case .invalidResponse: return "Máy chủ Gunny trả về dữ liệu không hợp lệ."
            case .missingRedirect: return "Không nhận được game redirect đã ký."
            }
        }
    }

    let gameBase: URL
    private var session: URLSession!
    private let redirectLock = NSLock()
    private var redirectLocation: URL?

    init(gameBase: URL) throws {
        let text = gameBase.absoluteString.hasSuffix("/")
            ? gameBase.absoluteString
            : gameBase.absoluteString + "/"
        guard let normalized = URL(string: text),
              let scheme = normalized.scheme?.lowercased(),
              (scheme == "http" || scheme == "https"),
              normalized.host != nil else {
            throw SessionError.invalidServer
        }
        self.gameBase = normalized
        super.init()

        let configuration = Self.makeSessionConfiguration()
        session = URLSession(configuration: configuration, delegate: self, delegateQueue: nil)
    }

    static func makeSessionConfiguration() -> URLSessionConfiguration {
        let configuration = URLSessionConfiguration.ephemeral
        configuration.httpShouldSetCookies = true
        configuration.requestCachePolicy = .reloadIgnoringLocalCacheData
        return configuration
    }

    deinit {
        session.invalidateAndCancel()
    }

    func authenticate(username: String, password: String) async throws -> LaunchInfo {
        guard !username.isEmpty, !password.isEmpty else { throw SessionError.invalidCredentials }
        var request = URLRequest(url: endpoint("createLogin.ashx"))
        request.httpMethod = "POST"
        request.httpBody = formBody([
            ("username", username),
            ("password", password)
        ])
        request.setValue("application/x-www-form-urlencoded", forHTTPHeaderField: "Content-Type")

        let (data, response) = try await session.data(for: request)
        try validate(response)
        let text = String(decoding: data, as: UTF8.self).trimmingCharacters(in: .whitespacesAndNewlines)
        guard text.caseInsensitiveCompare("ok") == .orderedSame else {
            throw SessionError.invalidCredentials
        }

        setRedirect(nil)
        var gameRequest = URLRequest(url: endpoint("LoginGame.aspx"))
        gameRequest.httpMethod = "GET"
        _ = try await session.data(for: gameRequest)
        guard let redirect = getRedirect() else { throw SessionError.missingRedirect }
        return try LaunchInfo.parseRedirect(redirect)
    }

    func fetchCaptcha() async throws -> Data {
        let (data, response) = try await session.data(from: endpoint("auth/ValidateCode.aspx"))
        try validate(response)
        return data
    }

    func register(
        username: String,
        password: String,
        confirmation: String,
        email: String,
        sex: String = "1",
        captchaCode: String
    ) async throws -> RegistrationResult {
        var request = URLRequest(url: endpoint("auth/register.ashx"))
        request.httpMethod = "POST"
        request.httpBody = formBody([
            ("username", username),
            ("password", password),
            ("repassword", confirmation),
            ("email", email),
            ("sex", sex),
            ("code", captchaCode)
        ])
        request.setValue("application/x-www-form-urlencoded", forHTTPHeaderField: "Content-Type")
        let (data, response) = try await session.data(for: request)
        try validate(response)
        let text = String(decoding: data, as: UTF8.self).trimmingCharacters(in: .whitespacesAndNewlines)
        return RegistrationResult(
            success: text.caseInsensitiveCompare("ok") == .orderedSame,
            message: text
        )
    }

    private func endpoint(_ path: String) -> URL {
        URL(string: path, relativeTo: gameBase)!.absoluteURL
    }

    private func validate(_ response: URLResponse) throws {
        guard let http = response as? HTTPURLResponse,
              (200..<300).contains(http.statusCode) else {
            throw SessionError.invalidResponse
        }
    }

    private func setRedirect(_ value: URL?) {
        redirectLock.lock()
        redirectLocation = value
        redirectLock.unlock()
    }

    private func getRedirect() -> URL? {
        redirectLock.lock()
        defer { redirectLock.unlock() }
        return redirectLocation
    }

    private func formBody(_ fields: [(String, String)]) -> Data {
        let text = fields.map { key, value in
            "\(formEncode(key))=\(formEncode(value))"
        }.joined(separator: "&")
        return Data(text.utf8)
    }

    private func formEncode(_ value: String) -> String {
        var allowed = CharacterSet.alphanumerics
        allowed.insert(charactersIn: "-._*")
        return value
            .addingPercentEncoding(withAllowedCharacters: allowed)?
            .replacingOccurrences(of: "%20", with: "+") ?? ""
    }

    func urlSession(
        _ session: URLSession,
        task: URLSessionTask,
        willPerformHTTPRedirection response: HTTPURLResponse,
        newRequest request: URLRequest,
        completionHandler: @escaping (URLRequest?) -> Void
    ) {
        let sourceName = response.url?.lastPathComponent.lowercased()
        if sourceName == "logingame.aspx" {
            setRedirect(request.url)
            completionHandler(nil)
            return
        }
        completionHandler(request)
    }
}
