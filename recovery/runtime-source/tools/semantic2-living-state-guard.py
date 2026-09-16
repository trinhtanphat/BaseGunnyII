from pathlib import Path
import re
import sys

ROOT = Path(__file__).resolve().parents[3]
LIVING = ROOT / "Game.Logic" / "Phy" / "Object" / "Living.cs"
text = LIVING.read_text(encoding="utf-8-sig")

checks = {
    "FallCount backing field": r"private\s+int\s+m_FallCount\s*;",
    "FallCount property": r"public\s+int\s+FallCount\s*\{[\s\S]*?get\s*\{\s*return\s+m_FallCount\s*;\s*\}[\s\S]*?set\s*\{\s*m_FallCount\s*=\s*value\s*;\s*\}",
    "SetSeal(bool) overload": r"public\s+void\s+SetSeal\s*\(\s*bool\s+state\s*\)",
    "SetSeal state mutation": r"m_isSeal\s*=\s*state\s*;",
    "SetSeal runtime property packet": r"SendGamePlayerProperty\s*\(\s*this\s*,\s*\"silenceMany\"\s*,\s*state\.ToString\(\)\s*\)",
}

failed = []
for name, pattern in checks.items():
    ok = re.search(pattern, text) is not None
    print(f"{'PASS' if ok else 'FAIL'}: {name}")
    if not ok:
        failed.append(name)

if failed:
    print("RED: missing runtime semantic contracts: " + ", ".join(failed))
    sys.exit(1)
print("GREEN: Living FallCount + SetSeal runtime semantic contracts present")