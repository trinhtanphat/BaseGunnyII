$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$loginMgr = Join-Path $root 'Center.Server\LoginMgr.cs'
if (-not (Test-Path $loginMgr)) { throw "missing LoginMgr: $loginMgr" }
$production = Get-Content $loginMgr -Raw
$stubs = @'
namespace SqlDataProvider.Data { }
namespace Center.Server {
  public enum ePlayerState { NotLogin, Logining, Play }
  public sealed class ServerInfo { public int ID; }
  public sealed class ServerClient {
    public ServerInfo Info = new ServerInfo();
    public int KickCount;
    public void SendKitoffUser(int id) { KickCount++; }
    public void SendAllowUserLogin(int id, bool allow) { }
  }
  public sealed class Player {
    public int Id;
    public string Name;
    public string Password;
    public bool IsFirst;
    public long LastTime;
    public ePlayerState State;
    public ServerClient CurrentServer;
  }
  public static class LoginMgrHarness {
    public static string Run() {
      const int id = 42;
      var road = new ServerClient();
      var p = new Player { Id = id, Name = "tester", Password = "token" };
      LoginMgr.CreatePlayer(p);

      if (!LoginMgr.TryLoginPlayer(id, road)) return "INITIAL_LOGIN_DENIED";
      if (road.KickCount != 0) return "CLEAN_LOGIN_KICKED";
      if (LoginMgr.GetPlayer(id).State != ePlayerState.Logining) return "NOT_LOGGING_IN";

      bool retryWhileStale = LoginMgr.TryLoginPlayer(id, road);
      if (retryWhileStale) return "STALE_LOGIN_ACCEPTED_TOO_EARLY";
      if (road.KickCount != 1) return "LOGGING_STALE_NOT_KICKED";

      LoginMgr.PlayerLoginOut(id, road);
      if (!LoginMgr.TryLoginPlayer(id, road)) return "RETRY_AFTER_OFFLINE_DENIED";
      if (!object.ReferenceEquals(LoginMgr.GetServerClient(id), road)) return "SERVER_NOT_RECLAIMED";
      return "PASS";
    }
  }
}
'@
$assemblyName = 'CenterReloginSmoke_' + [Guid]::NewGuid().ToString('N')
$out = Join-Path $env:TEMP ($assemblyName + '.dll')
try {
  Add-Type -TypeDefinition ($production + "`r`n" + $stubs) -Language CSharp -OutputAssembly $out
  Add-Type -Path $out
  $result = [Center.Server.LoginMgrHarness]::Run()
  if ($result -ne 'PASS') { throw "CENTER_STALE_SESSION_RELOGIN_SMOKE=FAIL:$result" }
  Write-Host 'CENTER_STALE_SESSION_RELOGIN_SMOKE=PASS'
}
finally {
  Remove-Item $out -Force -ErrorAction SilentlyContinue
}
