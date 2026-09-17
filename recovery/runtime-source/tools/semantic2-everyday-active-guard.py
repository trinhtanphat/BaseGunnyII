from pathlib import Path
root=Path(r'C:\Gunny\_work\BaseGunnyII-runtime-semantic2-r5-20260916')
checks=[]
def add(name, ok):
    checks.append((name,bool(ok)))
    print(('PASS: ' if ok else 'FAIL: ')+name)
handler=root/'Game.Server/Packets/Client/EverydayActivePointHandler.cs'
add('EverydayActivePointHandler source', handler.exists() and 'SendExpBlessedData' in handler.read_text(encoding='utf-8-sig',errors='ignore'))
proj=(root/'Game.Server/Game.Server.csproj').read_text(encoding='utf-8-sig',errors='ignore')
add('EverydayActivePointHandler compile mapping', 'Packets\\Client\\EverydayActivePointHandler.cs' in proj)
ip=(root/'Game.Server/Packets/Server/IPacketLib.cs').read_text(encoding='utf-8-sig',errors='ignore')
add('IPacketLib SendExpBlessedData contract', 'void SendExpBlessedData(int PlayerId);' in ip)
a=(root/'Game.Server/Packets/Server/AbstractPacketLib.cs').read_text(encoding='utf-8-sig',errors='ignore')
add('AbstractPacketLib ExpBlessed packet semantics', all(x in a for x in ['void SendExpBlessedData(int PlayerId)','new GSPacketIn(155, PlayerId)','WriteByte(8)','WriteInt(0)','SendTCP(gSPacketIn)']))
c=(root/'Game.Server/Packets/Server/ConsolePacketLib.cs').read_text(encoding='utf-8-sig',errors='ignore')
add('ConsolePacketLib ExpBlessed contract', 'void SendExpBlessedData(int PlayerId)' in c and 'throw new NotImplementedException();' in c)
if all(ok for _,ok in checks):
    print('GREEN: everyday active-point packet closure present')
    raise SystemExit(0)
print('RED: everyday active-point packet closure incomplete')
raise SystemExit(1)