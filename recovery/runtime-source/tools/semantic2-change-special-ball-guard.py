from pathlib import Path
import sys
root = Path(r'C:\Gunny\_work\BaseGunnyII-runtime-semantic2-r5-20260916')
p = (root/'Game.Logic/Phy/Object/Player.cs').read_text(encoding='utf-8-sig')
checks = [
    ('backing field', 'private int m_changeSpecialball;' in p),
    ('property', 'public int ChangeSpecialBall' in p and 'return m_changeSpecialball;' in p and 'm_changeSpecialball = value;' in p),
    ('constructor init', 'ChangeSpecialBall = 0;' in p),
    ('reset init', 'm_changeSpecialball = 0;' in p),
    ('weapon override guard', 'if (ChangeSpecialBall > 0)' in p),
    ('weapon override template', 'BallConfigMgr.FindBall(70396)' in p),
]
missing=[]
for name, ok in checks:
    print(('PASS' if ok else 'FAIL') + ': ' + name)
    if not ok: missing.append(name)
if missing:
    print('RED: ChangeSpecialBall runtime contracts missing: ' + ', '.join(missing)); sys.exit(1)
print('GREEN: ChangeSpecialBall runtime contracts present')
