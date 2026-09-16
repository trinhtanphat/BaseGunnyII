from pathlib import Path
import sys
root = Path(r'C:\Gunny\_work\BaseGunnyII-runtime-semantic2-r5-20260916')
p = (root/'Game.Logic/PVEGame.cs').read_text(encoding='utf-8-sig')
checks = [
    ('IsKillWorldBoss field', p.count('public bool IsKillWorldBoss;') == 1),
    ('no guessed initializer', 'public bool IsKillWorldBoss =' not in p),
]
missing=[]
for name, ok in checks:
    print(('PASS' if ok else 'FAIL') + ': ' + name)
    if not ok: missing.append(name)
if missing:
    print('RED: IsKillWorldBoss runtime contracts missing: ' + ', '.join(missing)); sys.exit(1)
print('GREEN: IsKillWorldBoss runtime contract present')
