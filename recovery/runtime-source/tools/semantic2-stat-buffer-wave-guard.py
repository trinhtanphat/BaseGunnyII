from pathlib import Path
import sys
r=Path(r'C:\Gunny\_work\BaseGunnyII-runtime-semantic2-r5-20260916')
fields={'AgiBuffer':'AgiAddPlus','AttackBuffer':'AttackAddPlus','DameBuffer':'DameAddPlus','DefendBuffer':'DefendAddPlus','GuardBuffer':'GuardAddPlus','HpBuffer':'HpAddPlus','LuckBuffer':'LuckAddPlus'}
pi=(r/'SqlDataProvider/Data/PlayerInfo.cs').read_text(encoding='utf-8-sig')
bl=(r/'Game.Server/Buffer/BufferList.cs').read_text(encoding='utf-8-sig')
gp=(r/'Game.Logic/Phy/Object/Player.cs').read_text(encoding='utf-8-sig')
proj=(r/'Game.Server/Game.Server.csproj').read_text(encoding='utf-8-sig')
checks=[]
checks.append(('7 PlayerInfo stat fields',all(pi.count(f'public int {f};')==1 for f in fields.values())))
sem=True
for cls,f in fields.items():
    p=r/f'Game.Server/Buffer/{cls}.cs'
    if not p.exists(): sem=False; continue
    s=p.read_text(encoding='utf-8-sig')
    sem &= f'player.PlayerCharacter.{f} += base.Info.Value;' in s and f'm_player.PlayerCharacter.{f} -= m_info.Value;' in s
    sem &= 'Info.ValidDate > 30' in s and 'Info.ValidDate = 30' in s
checks.append(('7 buffer lifecycle sources',sem))
checks.append(('7 compile mappings',all(proj.count(f'Buffer\\{c}.cs')==1 for c in fields)))
case_map={74:'DefendBuffer',75:'AttackBuffer',76:'GuardBuffer',77:'AgiBuffer',78:'DameBuffer',79:'HpBuffer',80:'LuckBuffer'}
checks.append(('factory cases 74-80',all(f'case {n}:' in bl and f'new {c}(info)' in bl for n,c in case_map.items())))
consumer=gp.count('m_player.PlayerCharacter.HpAddPlus;')>=2 and 'm_player.PlayerCharacter.AgiAddPlus' in gp
consumer &= all(x in gp for x in ['BaseDamage += m_player.PlayerCharacter.DameAddPlus;','BaseGuard += m_player.PlayerCharacter.GuardAddPlus;','Attack += m_player.PlayerCharacter.AttackAddPlus;','Defence += m_player.PlayerCharacter.DefendAddPlus;','Agility += m_player.PlayerCharacter.AgiAddPlus;','Lucky += m_player.PlayerCharacter.LuckAddPlus;','m_energy = (int)Agility / 30 + 240;'])
checks.append(('gameplay stat consumers',consumer))
for name,ok in checks: print(('PASS: ' if ok else 'FAIL: ')+name)
if not all(ok for _,ok in checks): sys.exit('RED: stat-buffer runtime closure missing')
print('GREEN: stat-buffer runtime closure present')