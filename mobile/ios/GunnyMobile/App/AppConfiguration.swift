import Foundation

enum AppConfiguration {
    static var socketProxyURL: URL? {
        guard let raw = Bundle.main.object(
            forInfoDictionaryKey: "GunnySocketProxyURL"
        ) as? String else {
            return nil
        }
        let value = raw.trimmingCharacters(in: .whitespacesAndNewlines)
        guard !value.isEmpty,
              !value.contains("$("),
              let url = URL(string: value) else {
            return nil
        }
        return url
    }
}
