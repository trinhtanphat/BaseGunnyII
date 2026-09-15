$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$living = Get-Content (Join-Path $root 'Game.Logic\Phy\Object\Living.cs') -Raw
$action = Get-Content (Join-Path $root 'Game.Logic\Actions\LivingMoveToAction.cs') -Raw
$game = Get-Content (Join-Path $root 'Game.Logic\BaseGame.cs') -Raw

function Assert-Match([string]$text, [string]$pattern, [string]$message) {
    if ($text -notmatch $pattern) { throw $message }
}

# Preserve the legacy surface while adding the recovered optional-action surface.
Assert-Match $living 'MoveTo\(int x, int y, string action, int delay, LivingCallBack callback\)' 'legacy Living.MoveTo callback overload missing'
Assert-Match $living 'MoveTo\(int x, int y, string action, int delay, string sAction, int speed\)' 'recovered Living.MoveTo 6-arg overload missing'
Assert-Match $living 'MoveTo\(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback\)' 'recovered Living.MoveTo callback overload missing'
Assert-Match $living 'MoveTo\(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback, int delayCallback\)' 'recovered Living.MoveTo delayed-callback overload missing'

Assert-Match $action 'LivingMoveToAction\(Living living, List<Point> path, string action, int delay, int speed, LivingCallBack callback\)' 'legacy LivingMoveToAction constructor missing'
Assert-Match $action 'LivingMoveToAction\(Living living, List<Point> path, string action, int delay, int speed, string sAction, LivingCallBack callback, int delayCallback\)' 'recovered LivingMoveToAction constructor missing'
Assert-Match $action 'SendLivingMoveTo\(m_living, m_living.X, m_living.Y, m_path\[m_path.Count - 1\].X, m_path\[m_path.Count - 1\].Y, m_action, m_speed, m_saction\)' 'secondary action is not sent to packet layer'
Assert-Match $action 'CallFuction\(m_callback, m_delayCallback\)' 'callback delay is not preserved'

Assert-Match $game 'SendLivingMoveTo\(Living living, int fromX, int fromY, int toX, int toY, string action,\s*int speed\)' 'legacy BaseGame.SendLivingMoveTo overload missing'
Assert-Match $game 'SendLivingMoveTo\(Living living, int fromX, int fromY, int toX, int toY, string action, int speed, string sAction\)' 'recovered BaseGame.SendLivingMoveTo overload missing'
Assert-Match $game 'WriteString\(!string\.IsNullOrEmpty\(sAction\) \? sAction : ""\);' 'packet secondary action is not serialized'

Write-Host 'RUNTIME_SEMANTIC_MOVETO_SMOKE=PASS'
