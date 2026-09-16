from pathlib import Path
import sys
root=Path(__file__).resolve().parents[3]
base=(root/'Game.Logic/BaseGame.cs').read_text(encoding='utf-8-sig'); pve=(root/'Game.Logic/PVEGame.cs').read_text(encoding='utf-8-sig')
living=(root/'Game.Logic/Phy/Object/Living.cs').read_text(encoding='utf-8-sig'); phys=(root/'Game.Logic/Phy/Object/PhysicalObj.cs').read_text(encoding='utf-8-sig')
gproj=(root/'Game.Logic/Game.Logic.csproj').read_text(encoding='utf-8-sig'); sproj=(root/'GameServerScript/GameServerScript.csproj').read_text(encoding='utf-8-sig')
ballp=root/'Game.Logic/Phy/Object/Ball.cs'; ball=ballp.read_text(encoding='utf-8-sig') if ballp.exists() else ''; mission=root/'GameServerScript/AI/Messions/GON6101.cs'
checks=[
 ('Shuffer','public void Shuffer<T>(T[] array)' in base and 'Random.Next(num)' in base),
 ('ball state','private List<Ball> m_tempBall;' in base and 'm_tempBall = new List<Ball>();' in base),
 ('AddBall point','public Ball AddBall(Point pos, bool sendToClient)' in base and 'return AddBall(ball, sendToClient);' in base),
 ('AddBall object','public Ball AddBall(Ball ball, bool sendToClient)' in base and 'm_tempBall.Add(ball);' in base),
 ('ClearBall','public void ClearBall()' in base and 'RemovePhysicalObj(item2, sendToClient: true);' in base),
 ('CreateBall','public Ball CreateBall(int x, int y, string action)' in pve and 'AddBall(ball, sendToClient: true);' in pve),
 ('Ball source',ballp.exists() and 'public class Ball : PhysicalObj' in ball and 'simpleBomb.Owner.PickBall(this);' in ball),
 ('Living PickBall','public virtual void PickBall(Ball ball)' in living and 'ball.PlayMovie(ball.ActionMapping[currentAction], 1000, 0);' in living),
 ('ActionMapping field/property','Dictionary<string, string> m_actionMapping' in phys and 'ActionMapping' in phys),
 ('ActionMapping init','m_actionMapping = new Dictionary<string, string>();' in phys and 'asset.game.six.ball' in phys),
 ('ActionMapping values','shield1' in phys and 'shield-6' in phys and 'shield-double' in phys),
 ('Ball compile map','Phy\\Object\\Ball.cs' in gproj),
 ('GON6101 source/map',mission.exists() and 'AI\\Messions\\GON6101.cs' in sproj),
]
failed=[]
for name,ok in checks:
 print(('PASS: ' if ok else 'FAIL: ')+name)
 if not ok: failed.append(name)
if failed:
 print('RED: missing GON6101 ball-wave contracts: '+', '.join(failed)); sys.exit(1)
print('GREEN: GON6101 ball-wave runtime contracts present')