import Foundation
import WebKit

final class RuffleAssetSchemeHandler: NSObject, WKURLSchemeHandler {
    enum AssetError: Error {
        case invalidURL
        case assetNotFound
    }

    private let root: URL

    init(root: URL) {
        self.root = root.standardizedFileURL
        super.init()
    }

    func webView(_ webView: WKWebView, start urlSchemeTask: WKURLSchemeTask) {
        do {
            guard let url = urlSchemeTask.request.url,
                  url.scheme == "gunny-ruffle",
                  url.host == "assets" else {
                throw AssetError.invalidURL
            }

            let relative = url.path.trimmingCharacters(in: CharacterSet(charactersIn: "/"))
            guard !relative.isEmpty,
                  !relative.contains(".."),
                  !relative.contains("\\") else {
                throw AssetError.invalidURL
            }
            let target = root.appendingPathComponent(relative).standardizedFileURL
            guard target.path.hasPrefix(root.path + "/"),
                  FileManager.default.fileExists(atPath: target.path) else {
                throw AssetError.assetNotFound
            }

            let data = try Data(contentsOf: target, options: .mappedIfSafe)
            let response = URLResponse(
                url: url,
                mimeType: mimeType(for: target.pathExtension),
                expectedContentLength: data.count,
                textEncodingName: nil
            )
            urlSchemeTask.didReceive(response)
            urlSchemeTask.didReceive(data)
            urlSchemeTask.didFinish()
        } catch {
            urlSchemeTask.didFailWithError(error)
        }
    }

    func webView(_ webView: WKWebView, stop urlSchemeTask: WKURLSchemeTask) {}

    private func mimeType(for extensionName: String) -> String {
        switch extensionName.lowercased() {
        case "js": return "application/javascript"
        case "wasm": return "application/wasm"
        case "json", "map": return "application/json"
        default: return "application/octet-stream"
        }
    }
}
