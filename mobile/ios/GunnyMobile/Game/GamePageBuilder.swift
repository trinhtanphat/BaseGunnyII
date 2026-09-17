import Foundation

enum GamePageBuilder {
    enum PageError: Error {
        case invalidProxyURL
        case encodingFailed
    }

    static let gameHost = "103.9.156.181"
    static let gamePort = 9200
    static let publicPath = "gunny-ruffle://assets/"

    static func makeBootstrapScript(proxyURL: URL) throws -> String {
        guard proxyURL.scheme?.lowercased() == "wss",
              proxyURL.host != nil,
              proxyURL.path == "/socket",
              URLComponents(url: proxyURL, resolvingAgainstBaseURL: false)?
                .queryItems?.contains(where: { $0.name == "route" && $0.value == "game" }) == true else {
            throw PageError.invalidProxyURL
        }

        let proxyJS = try javascriptString(proxyURL.absoluteString)
        return """
        window.RufflePlayer = window.RufflePlayer || {};
        window.RufflePlayer.config = {
          publicPath: "\(publicPath)",
          autoplay: "on",
          unmuteOverlay: "hidden",
          socketProxy: [{ host: "\(gameHost)", port: \(gamePort), proxyUrl: \(proxyJS) }]
        };
        window.addEventListener("DOMContentLoaded", () => {
          const script = document.createElement("script");
          script.src = "\(publicPath)ruffle.js";
          document.head.appendChild(script);
        });
        """
    }

    private static func javascriptString(_ value: String) throws -> String {
        let data = try JSONSerialization.data(
            withJSONObject: [value],
            options: [.withoutEscapingSlashes]
        )
        guard var text = String(data: data, encoding: .utf8),
              text.count >= 2 else {
            throw PageError.encodingFailed
        }
        text.removeFirst()
        text.removeLast()
        return text
    }
}
