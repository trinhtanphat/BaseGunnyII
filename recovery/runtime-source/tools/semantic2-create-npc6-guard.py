from pathlib import Path
p=Path(r'C:\Gunny\_work\BaseGunnyII-runtime-semantic2-r5-20260916\Game.Logic\PVEGame.cs')
s=p.read_text(encoding='utf-8-sig')
checks={
'CreateNpc6 signature':'public SimpleNpc CreateNpc(int npcId, int x, int y, int type, int direction, LivingConfig config)' in s,
'config assignment':'simpleNpc.Config = config;' in s,
'reduce blood':'simpleNpc.Blood = npcInfoById.Blood / simpleNpc.Config.ReduceBloodStart;' in s,
'reset fallback':'simpleNpc.Reset();' in s,
'registration':'AddLiving(simpleNpc);' in s and 'simpleNpc.StartMoving();' in s,
}
for k,v in checks.items(): print(('PASS' if v else 'FAIL')+': '+k)
if not all(checks.values()):
 print('RED: CreateNpc(6) runtime contracts missing'); raise SystemExit(1)
print('GREEN: CreateNpc(6) runtime contracts present')
