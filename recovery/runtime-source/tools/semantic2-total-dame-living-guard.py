from pathlib import Path
r=Path(r'C:\Gunny\_work\BaseGunnyII-runtime-semantic2-r5-20260916')
l=(r/'Game.Logic/Phy/Object/Living.cs').read_text(encoding='utf-8-sig')
b=(r/'Game.Logic/Phy/Object/SimpleBomb.cs').read_text(encoding='utf-8-sig')
checks=[
 ('TotalDameLiving field','public int TotalDameLiving;' in l),
 ('player-to-boss accounting guard','if (m_owner is Player && p is SimpleBoss)' in b),
 ('boss damage accumulation','m_owner.TotalDameLiving += critical + damage;' in b),
]
for label,ok in checks: print(('PASS: ' if ok else 'FAIL: ')+label)
if not all(ok for _,ok in checks): raise SystemExit('RED: TotalDameLiving runtime contracts missing')
print('GREEN: TotalDameLiving runtime contracts present')