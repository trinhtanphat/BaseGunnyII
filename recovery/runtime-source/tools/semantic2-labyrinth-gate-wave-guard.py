from pathlib import Path
r=Path(r'C:\Gunny\_work\BaseGunnyII-runtime-semantic2-r5-20260916')
p=(r/'Game.Logic/PVEGame.cs').read_text(encoding='utf-8-sig')
proj=(r/'GameServerScript/GameServerScript.csproj').read_text(encoding='utf-8-sig')
clean='40001 40002 40003 40004 40005 40006 40007 40008 40009 40010 40011 40013 40014 40015 40017 40018 40019 40020 40021 40023 40024 40025 40027 40028 40029 40031 40033 40035 40036 40037 40038 40039'.split()
names=[f'Labyrinth{x}.cs' for x in clean]
checks=[]
checks.append(('CanEnterGate field','public bool CanEnterGate;' in p))
checks.append(('CanShowBigBox field','public bool CanShowBigBox;' in p))
checks.append(('CreateGate method','public void CreateGate(bool isEnter)' in p and 'CanEnterGate = isEnter;' in p))
missing_src=[n for n in names if not (r/'GameServerScript/AI/Messions'/n).exists()]
checks.append(('32 mission sources',not missing_src))
bad_map=[n for n in names if proj.count('AI\\Messions\\'+n)!=1]
checks.append(('32 compile mappings',not bad_map))
for label,ok in checks: print(('PASS: ' if ok else 'FAIL: ')+label)
if missing_src: print('MISSING_SOURCE='+','.join(missing_src))
if bad_map: print('BAD_MAPPING='+','.join(bad_map))
if not all(ok for _,ok in checks): raise SystemExit('RED: Labyrinth gate-wave contracts missing')
print('GREEN: Labyrinth gate-wave contracts present')