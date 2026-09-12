import SwiftUI
import WebKit

struct GameWebView: UIViewRepresentable {
    let launch: LaunchInfo
    let gameBase: URL
    let proxyURL: URL

    func makeCoordinator() -> Coordinator {
        Coordinator(allowedHost: gameBase.host?.lowercased())
    }

    func makeUIView(context: Context) -> WKWebView {
        let configuration = WKWebViewConfiguration()
        configuration.websiteDataStore = .nonPersistent()

        let resourceRoot = Bundle.main.url(
            forResource: "RuffleAssets",
            withExtension: "bundle"
        ) ?? Bundle.main.bundleURL
        let handler = RuffleAssetSchemeHandler(root: resourceRoot)
        context.coordinator.assetHandler = handler
        configuration.setURLSchemeHandler(handler, forURLScheme: "gunny-ruffle")

        if let bootstrap = try? GamePageBuilder.makeBootstrapScript(proxyURL: proxyURL) {
            configuration.userContentController.addUserScript(WKUserScript(
                source: bootstrap,
                injectionTime: .atDocumentStart,
                forMainFrameOnly: true
            ))
        }

        let webView = WKWebView(frame: .zero, configuration: configuration)
        webView.navigationDelegate = context.coordinator
        webView.isOpaque = false
        webView.backgroundColor = .black
        webView.scrollView.backgroundColor = .black
        webView.scrollView.contentInsetAdjustmentBehavior = .never

        if let redirect = try? launch.signedRedirectURL(gameBase: gameBase) {
            webView.load(URLRequest(url: redirect))
        }
        return webView
    }

    func updateUIView(_ uiView: WKWebView, context: Context) {}

    final class Coordinator: NSObject, WKNavigationDelegate {
        let allowedHost: String?
        var assetHandler: RuffleAssetSchemeHandler?

        init(allowedHost: String?) {
            self.allowedHost = allowedHost
        }

        func webView(
            _ webView: WKWebView,
            decidePolicyFor navigationAction: WKNavigationAction,
            decisionHandler: @escaping (WKNavigationActionPolicy) -> Void
        ) {
            guard let url = navigationAction.request.url else {
                decisionHandler(.cancel)
                return
            }
            let scheme = url.scheme?.lowercased()
            if scheme == "gunny-ruffle" || scheme == "about" {
                decisionHandler(.allow)
                return
            }

            if (scheme == "http" || scheme == "https"),
               url.host?.lowercased() == allowedHost {
                decisionHandler(.allow)
                return
            }

            decisionHandler(.cancel)
        }
    }
}
