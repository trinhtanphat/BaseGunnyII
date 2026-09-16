from pathlib import Path
import re, sys
ROOT = Path(__file__).resolve().parents[3]
text = (ROOT / "Game.Logic" / "BaseGame.cs").read_text(encoding="utf-8-sig")
start = text.find("public void ClearAllChild()")
body = ""
if start >= 0:
    brace = text.find("{", start)
    depth = 0
    for i in range(brace, len(text)):
        if text[i] == "{": depth += 1
        elif text[i] == "}":
            depth -= 1
            if depth == 0:
                body = text[start:i+1]
                break
checks = {
    "ClearAllChild method": bool(body),
    "collect live SimpleNpc": bool(re.search(r"living\.IsLiving\s*&&\s*living\s+is\s+SimpleNpc", body)),
    "remove from living list": "m_livings.Remove(item);" in body,
    "dispose child": "item.Dispose();" in body,
    "send living removal": "RemoveLiving(item.Id);" in body,
}
failed=[]
for name, ok in checks.items():
    print(f"{'PASS' if ok else 'FAIL'}: {name}")
    if not ok: failed.append(name)
if failed:
    print("RED: missing ClearAllChild runtime contracts: " + ", ".join(failed)); sys.exit(1)
print("GREEN: BaseGame.ClearAllChild runtime semantic contracts present")