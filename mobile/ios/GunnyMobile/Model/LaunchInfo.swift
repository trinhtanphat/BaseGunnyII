import Foundation

struct LaunchInfo: Equatable, Sendable {
    let user: String
    let key: String
    let editBy: String

    enum ParseError: Error {
        case invalidRedirect
        case missingSignedField(String)
    }

    static func parseRedirect(_ url: URL) throws -> LaunchInfo {
        guard let components = URLComponents(url: url, resolvingAgainstBaseURL: false) else {
            throw ParseError.invalidRedirect
        }
        let values = Dictionary(uniqueKeysWithValues:
            (components.queryItems ?? []).compactMap { item in
                item.value.map { (item.name.lowercased(), $0) }
            })
        guard let user = values["user"], !user.isEmpty else {
            throw ParseError.missingSignedField("user")
        }
        guard let key = values["key"], !key.isEmpty else {
            throw ParseError.missingSignedField("key")
        }
        guard let editBy = values["editby"], !editBy.isEmpty else {
            throw ParseError.missingSignedField("editby")
        }
        return LaunchInfo(user: user, key: key, editBy: editBy)
    }

    func signedRedirectURL(gameBase: URL) throws -> URL {
        let base = gameBase.absoluteString.hasSuffix("/")
            ? gameBase
            : URL(string: gameBase.absoluteString + "/")!
        guard let target = URL(string: "Default.aspx", relativeTo: base)?.absoluteURL,
              var components = URLComponents(url: target, resolvingAgainstBaseURL: false) else {
            throw ParseError.invalidRedirect
        }
        components.queryItems = [
            URLQueryItem(name: "user", value: user),
            URLQueryItem(name: "key", value: key),
            URLQueryItem(name: "editby", value: editBy)
        ]
        guard let url = components.url else { throw ParseError.invalidRedirect }
        return url
    }
    func signedSWFURL(gameBase: URL) throws -> URL {
        let base = gameBase.absoluteString.hasSuffix("/")
            ? gameBase
            : URL(string: gameBase.absoluteString + "/")!
        guard let swf = URL(string: "flash/Loading.swf", relativeTo: base)?.absoluteURL,
              let config = URL(string: "config.xml", relativeTo: base)?.absoluteURL,
              var components = URLComponents(url: swf, resolvingAgainstBaseURL: false) else {
            throw ParseError.invalidRedirect
        }
        components.queryItems = [
            URLQueryItem(name: "user", value: user),
            URLQueryItem(name: "key", value: key),
            URLQueryItem(name: "config", value: config.absoluteString)
        ]
        guard let url = components.url else {
            throw ParseError.invalidRedirect
        }
        return url
    }
}

struct RegistrationResult: Equatable, Sendable {
    let success: Bool
    let message: String
}
