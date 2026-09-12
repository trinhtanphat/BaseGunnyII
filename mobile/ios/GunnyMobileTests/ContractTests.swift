import XCTest
@testable import GunnyMobile

final class ContractTests: XCTestCase {
    func testGoldenProtocolFixtureMatchesSwiftLauncher() throws {
        let fixture = try loadFixture()
        let login = try XCTUnwrap(fixture["login"] as? [String: Any])
        XCTAssertEqual(login["endpoint"] as? String, "createLogin.ashx")
        XCTAssertEqual(login["redirectEndpoint"] as? String, "LoginGame.aspx")

        let registration = try XCTUnwrap(fixture["registration"] as? [String: Any])
        XCTAssertEqual(registration["captchaEndpoint"] as? String, "auth/ValidateCode.aspx")
        XCTAssertEqual(registration["registerEndpoint"] as? String, "auth/register.ashx")
        XCTAssertEqual(
            registration["fields"] as? [String],
            ["username", "password", "repassword", "email", "sex", "code"]
        )

        let sample = try XCTUnwrap(fixture["sample"] as? [String: Any])
        let redirect = try XCTUnwrap(URL(string: try XCTUnwrap(sample["redirect"] as? String)))
        let launch = try LaunchInfo.parseRedirect(redirect)
        XCTAssertEqual(launch.user, sample["user"] as? String)
        XCTAssertEqual(launch.key, sample["key"] as? String)
        XCTAssertEqual(launch.editBy, sample["editBy"] as? String)

        let gameBase = try XCTUnwrap(URL(string: try XCTUnwrap(fixture["gameBase"] as? String)))
        XCTAssertEqual(
            try launch.signedRedirectURL(gameBase: gameBase).absoluteString,
            sample["redirect"] as? String
        )
        XCTAssertEqual(
            try launch.signedSWFURL(gameBase: gameBase).absoluteString,
            sample["swf"] as? String
        )

        let socket = try XCTUnwrap(fixture["gameSocket"] as? [String: Any])
        XCTAssertEqual(socket["host"] as? String, "103.9.156.182")
        XCTAssertEqual(socket["port"] as? Int, 9200)
    }

    func testSessionConfigurationDoesNotUseSharedCookieStorage() {
        let configuration = GunnySession.makeSessionConfiguration()
        XCTAssertTrue(configuration.httpShouldSetCookies)
        if let storage = configuration.httpCookieStorage {
            XCTAssertFalse(storage === HTTPCookieStorage.shared)
        }
    }

    func testRuffleBootstrapContainsOnlyApprovedSocketProxy() throws {
        let proxyURL = try XCTUnwrap(URL(string: "wss://proxy.example.test/socket?route=game"))
        let script = try GamePageBuilder.makeBootstrapScript(proxyURL: proxyURL)
        XCTAssertTrue(script.contains("publicPath: \"gunny-ruffle://assets/\""))
        XCTAssertTrue(script.contains("socketProxy: [{ host: \"103.9.156.182\", port: 9200, proxyUrl: \"wss://proxy.example.test/socket?route=game\" }]") )
        XCTAssertEqual(script.components(separatedBy: "proxyUrl:").count - 1, 1)
    }

    private func loadFixture() throws -> [String: Any] {
        let url = try XCTUnwrap(Bundle(for: Self.self).url(
            forResource: "gunny-launch-contract",
            withExtension: "json"
        ))
        let data = try Data(contentsOf: url)
        return try XCTUnwrap(JSONSerialization.jsonObject(with: data) as? [String: Any])
    }
}
