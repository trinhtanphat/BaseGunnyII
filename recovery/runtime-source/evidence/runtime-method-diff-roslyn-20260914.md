# Runtime method/type diff (Roslyn AST)

Best-source comparison of decompiled runtime variants using Roslyn syntax trees.

## center / Bussiness
- best source: `DDTank41`
- runtime SHA256: `24fa15ff6045e87736624dddb7cacdd9bfbfbe0414d6ba6ab57a3a132528cf0f`
- binary-only files: **2**
- runtime-only types: **2**
- runtime-only method signatures: **141**
- source-only method signatures: **259**
- files with signature deltas: **25**
- binary-only sample: `Bussiness\Interface\InterfaceType.cs`, `Bussiness\Managers\TreasureAwardMgr.cs`
- runtime-only type sample: `Bussiness.Interface.InterfaceType`, `Bussiness.Managers.TreasureAwardMgr`
- runtime-only method sample: `Bussiness.ActiveBussiness.AddActiveNumber(string,int)`, `Bussiness.ActiveBussiness.PullDown(int,string,int,refstring)`, `Bussiness.BaseBussiness..ctor()`, `Bussiness.CenterService.CenterServiceClient.ValidateLoginAndGetID(string,string,refint,refbool)`, `Bussiness.CenterService.ICenterService.ValidateLoginAndGetID(string,string,refint,refbool)`, `Bussiness.GameProperties.ConsortiaStrengExp(int)`, `Bussiness.GameProperties.HoleLevelUpExp(int)`, `Bussiness.GameProperties.RuneExp()`, `Bussiness.GameProperties.VIPStrengthenExp(int)`, `Bussiness.Interface.BaseInterface.CreateLogin(string,string,refstring,refint,string,refbool,bool,refbool,string,string)`, `Bussiness.Interface.BaseInterface.LoginGame(string,string,refbool)`, `Bussiness.Managers.AchievementMgr.GetNextLimit(int,int)`

## center / Center.Server
- best source: `DDTank41`
- runtime SHA256: `f446ce3aef72e2b57fb626a11a125c16caf436fd186a89f65bc4840b245a30c2`
- binary-only files: **0**
- runtime-only types: **0**
- runtime-only method signatures: **15**
- source-only method signatures: **0**
- files with signature deltas: **2**
- runtime-only method sample: `Center.Server.ServerClient.HandleEventRank(GSPacketIn)`, `Center.Server.ServerClient.HandleWorldEvent(GSPacketIn)`, `Center.Server.WorldMgr.AddOrUpdateLanternriddles(int,LanternriddlesInfo)`, `Center.Server.WorldMgr.GetAllLuckyStarRank()`, `Center.Server.WorldMgr.GetLanternriddles(int)`, `Center.Server.WorldMgr.ResetLightriddleRank()`, `Center.Server.WorldMgr.ResetLuckStar()`, `Center.Server.WorldMgr.SaveLuckyStarRewardRecord()`, `Center.Server.WorldMgr.SavekyStarToDatabase()`, `Center.Server.WorldMgr.SelectTopEight()`, `Center.Server.WorldMgr.SendLightriddleTopEightAward()`, `Center.Server.WorldMgr.SendLuckyStarTopTenAward()`

## center / Center.Service
- best source: `DDTank41`
- runtime SHA256: `5c24462c871d8a70dafcd9ac2a9b747e6e81457abfda7722ba1c75368b4491a9`
- binary-only files: **0**
- runtime-only types: **0**
- runtime-only method signatures: **9**
- source-only method signatures: **9**
- files with signature deltas: **2**
- runtime-only method sample: `Game.Service.Program.GetAction(string)`, `Game.Service.Program.Main(string[])`, `Game.Service.Program.ParseParameters(string[],outstring,outHashtable)`, `Game.Service.Program.RegisterAction(IAction)`, `Game.Service.Program.RegisterActions()`, `Game.Service.Program.ShowSyntax()`, `Game.Service.actions.ConsoleStart.OnAction(Hashtable)`, `Game.Service.actions.ConsoleStart.Reload(eReloadType)`, `Game.Service.actions.ConsoleStart.StartServer()`

## center / Game.Base
- best source: `DDTank4.1`
- runtime SHA256: `98af5762474f3c328213db101e0e2cfce95be297662687a79f50fcfe0b84fa7c`
- binary-only files: **0**
- runtime-only types: **0**
- runtime-only method signatures: **0**
- source-only method signatures: **7**
- files with signature deltas: **2**

## center / SqlDataProvider
- best source: `DDTank41`
- runtime SHA256: `0a1f788f4664ddfba3394b27104ec31ba3ab64840be8d4cbcec4ae0458cba71f`
- binary-only files: **2**
- runtime-only types: **2**
- runtime-only method signatures: **10**
- source-only method signatures: **37**
- files with signature deltas: **12**
- binary-only sample: `SqlDataProvider\Data\GoldEquipTemplateLoadInfo.cs`, `SqlDataProvider\Data\UserGemStone.cs`
- runtime-only type sample: `SqlDataProvider.Data.GoldEquipTemplateLoadInfo`, `SqlDataProvider.Data.UserGemStone`
- runtime-only method sample: `SqlDataProvider.BaseClass.Sql_DbObject.SetDataTable(DataTable,string,List<SqlBulkCopyColumnMapping>)`, `SqlDataProvider.Data.ItemInfo.IsAdvanceDate()`, `SqlDataProvider.Data.PlayerInfo.IsValidadteTimeBox()`, `SqlDataProvider.Data.QuestDataInfo.setProgressConcoat()`, `SqlDataProvider.Data.UsersPetinfo.GetEquip()`, `SqlDataProvider.Data.UsersPetinfo.GetPetType(int)`, `SqlDataProvider.Data.UsersPetinfo.GetSkill()`, `SqlDataProvider.Data.UsersPetinfo.GetSkillEquip()`, `SqlDataProvider.Data.UsersPetinfo.ReduceProp(int)`, `SqlDataProvider.Data.UsersPetinfo.happyPercent()`

## Fight / Bussiness
- best source: `DDTank41`
- runtime SHA256: `d6f9342d241a6ea94753bbc4e05acb3361bac852bcaf9f23c150010aaedf29bc`
- binary-only files: **2**
- runtime-only types: **2**
- runtime-only method signatures: **132**
- source-only method signatures: **250**
- files with signature deltas: **24**
- binary-only sample: `Bussiness\Interface\InterfaceType.cs`, `Bussiness\Managers\TreasureAwardMgr.cs`
- runtime-only type sample: `Bussiness.Interface.InterfaceType`, `Bussiness.Managers.TreasureAwardMgr`
- runtime-only method sample: `Bussiness.ActiveBussiness.AddActiveNumber(string,int)`, `Bussiness.ActiveBussiness.PullDown(int,string,int,refstring)`, `Bussiness.BaseBussiness..ctor()`, `Bussiness.CenterService.CenterServiceClient.ValidateLoginAndGetID(string,string,refint,refbool)`, `Bussiness.CenterService.ICenterService.ValidateLoginAndGetID(string,string,refint,refbool)`, `Bussiness.GameProperties.ConsortiaStrengExp(int)`, `Bussiness.GameProperties.HoleLevelUpExp(int)`, `Bussiness.GameProperties.RuneExp()`, `Bussiness.GameProperties.VIPStrengthenExp(int)`, `Bussiness.Interface.BaseInterface.CreateLogin(string,string,refstring,refint,string,refbool,bool,refbool,string,string)`, `Bussiness.Interface.BaseInterface.LoginGame(string,string,refbool)`, `Bussiness.Managers.AchievementMgr.GetNextLimit(int,int)`

## Fight / Fighting.Server
- best source: `DDTank41`
- runtime SHA256: `bcda33f114d84465ec6be03bf73f5d62d6d3c4e09c9eae50fef91532140a78a9`
- binary-only files: **0**
- runtime-only types: **0**
- runtime-only method signatures: **25**
- source-only method signatures: **48**
- files with signature deltas: **7**
- runtime-only method sample: `Fighting.Server.GameObjects.ProxyPlayer..ctor(ServerClient,PlayerInfo,UserMatchInfo,ProxyPlayerInfo,UsersPetinfo,List<BufferInfo>,List<ItemInfo>,List<BufferInfo>)`, `Fighting.Server.GameObjects.ProxyPlayer.AddActiveMoney(int)`, `Fighting.Server.GameObjects.ProxyPlayer.AddLeagueMoney(int)`, `Fighting.Server.GameObjects.ProxyPlayer.AddMedal(int)`, `Fighting.Server.GameObjects.ProxyPlayer.AddPrestige(bool)`, `Fighting.Server.GameObjects.ProxyPlayer.AddTemplate(ItemInfo,eBageType,int,eItemNotice,eItemNotice)`, `Fighting.Server.GameObjects.ProxyPlayer.FootballTakeOut(bool)`, `Fighting.Server.GameObjects.ProxyPlayer.GetFightFootballStyle(int)`, `Fighting.Server.GameObjects.ProxyPlayer.OnGameOver(AbstractGame,bool,int)`, `Fighting.Server.Games.GameMgr.CanUseBuff(eGameType)`, `Fighting.Server.Games.GameMgr.ClearStoppedGames(long)`, `Fighting.Server.Games.GameMgr.GameThread()`

## Fight / Fighting.Service
- best source: `DDTank41`
- runtime SHA256: `dc4eb09bc5f0ec10ae018da823a4896b826b4ae58d498da89a83e0ad29c6ed9b`
- binary-only files: **0**
- runtime-only types: **0**
- runtime-only method signatures: **0**
- source-only method signatures: **0**
- files with signature deltas: **0**

## Fight / Game.Base
- best source: `DDTank4.1`
- runtime SHA256: `5851154b8fef454261006a00b92e1b89f4d1d254ef519ee57219b2784d265fb3`
- binary-only files: **0**
- runtime-only types: **0**
- runtime-only method signatures: **0**
- source-only method signatures: **7**
- files with signature deltas: **2**

## Fight / Game.Logic
- best source: `DDTank41`
- runtime SHA256: `c55aa6d9dca93b1d7183d7f9e46f618f38e66e7b69ea2de72bd4c75ada66d661`
- binary-only files: **25**
- runtime-only types: **24**
- runtime-only method signatures: **314**
- source-only method signatures: **482**
- files with signature deltas: **45**
- binary-only sample: `Game\Logic\PetEffects\PetAddAttackEquipEffect.cs`, `Game\Logic\PetEffects\PetAddDefendEffect.cs`, `Game\Logic\PetEffects\PetAddDefendEquipEffect.cs`, `Game\Logic\PetEffects\PetAddLuckEquipEffect.cs`, `Game\Logic\PetEffects\PetAlwayNoHoleEquipEffect.cs`, `Game\Logic\PetEffects\PetAttackAroundEquipEffect.cs`, `Game\Logic\PetEffects\PetFatalEffect.cs`, `Game\Logic\PetEffects\PetNoHoleEffect.cs`, `Game\Logic\PetEffects\PetNoHoleEquipEffect.cs`, `Game\Logic\PetEffects\PetPlusAllTwoMpEquipEffect.cs`, `Game\Logic\PetEffects\PetPlusDameEquipEffect.cs`, `Game\Logic\PetEffects\PetPlusGuardEquipEffect.cs`
- runtime-only type sample: `Game.Logic.PetEffects.PetAddAttackEquipEffect`, `Game.Logic.PetEffects.PetAddDefendEffect`, `Game.Logic.PetEffects.PetAddDefendEquipEffect`, `Game.Logic.PetEffects.PetAddLuckEquipEffect`, `Game.Logic.PetEffects.PetAlwayNoHoleEquipEffect`, `Game.Logic.PetEffects.PetAttackAroundEquipEffect`, `Game.Logic.PetEffects.PetFatalEffect`, `Game.Logic.PetEffects.PetNoHoleEffect`, `Game.Logic.PetEffects.PetNoHoleEquipEffect`, `Game.Logic.PetEffects.PetPlusAllTwoMpEquipEffect`, `Game.Logic.PetEffects.PetPlusDameEquipEffect`, `Game.Logic.PetEffects.PetPlusGuardEquipEffect`
- runtime-only method sample: `Game.Logic.AI.ABrain..ctor()`, `Game.Logic.Actions.CallFunctionAction..ctor(LivingCallBack,int)`, `Game.Logic.Actions.FightAchievementAction..ctor(Living,int,int,int)`, `Game.Logic.Actions.LivingDieAction..ctor(Living,int)`, `Game.Logic.Actions.LivingRangeAttackingAction..ctor(Living,int,int,string,int,List<Player>)`, `Game.Logic.Actions.LivingRotateTurnAction..ctor(Player,int,int,string,int)`, `Game.Logic.Actions.LivingSealAction..ctor(Living,Player,int,int)`, `Game.Logic.BallMgr.LoadFromDatabase()`, `Game.Logic.BallMgr.LoadFromFiles(Dictionary<int,BallInfo>)`, `Game.Logic.BaseGame.AddBall(Point,bool)`, `Game.Logic.BaseGame.FindNextTurnedFightFootball()`, `Game.Logic.BaseGame.FindPhysicalObjByName(string,bool)`

## Fight / GameServerScripts
- best source: `BaseGunnyII`
- runtime SHA256: `e4d0871d43f7cf6e7689c65b3ce2b738b7e140b0ae2db4f3e7cd0c8544bd84f0`
- binary-only files: **3**
- runtime-only types: **3**
- runtime-only method signatures: **25**
- source-only method signatures: **0**
- files with signature deltas: **0**
- binary-only sample: `GameServerScript\AI\NPC\NullAi.cs`, `GameServerScript\AI\NPC\SeizeNpcAi.cs`, `GameServerScript\Commands\VersionCommandHandler.cs`
- runtime-only type sample: `GameServerScript.AI.NPC.NullAi`, `GameServerScript.AI.NPC.SeizeNpcAi`, `GameServerScript.Commands.VersionCommandHandler`
- runtime-only method sample: `GameServerScript.AI.NPC.NullAi.AllAttack()`, `GameServerScript.AI.NPC.NullAi.ChangeDirection(int)`, `GameServerScript.AI.NPC.NullAi.CreateChild()`, `GameServerScript.AI.NPC.NullAi.KillAttack(int,int)`, `GameServerScript.AI.NPC.NullAi.NextAttack()`, `GameServerScript.AI.NPC.NullAi.OnBeginNewTurn()`, `GameServerScript.AI.NPC.NullAi.OnBeginSelfTurn()`, `GameServerScript.AI.NPC.NullAi.OnCreated()`, `GameServerScript.AI.NPC.NullAi.OnStartAttacking()`, `GameServerScript.AI.NPC.NullAi.OnStopAttacking()`, `GameServerScript.AI.NPC.NullAi.PersonalAttack()`, `GameServerScript.AI.NPC.NullAi.Summon()`

## Fight / SqlDataProvider
- best source: `DDTank41`
- runtime SHA256: `0a1f788f4664ddfba3394b27104ec31ba3ab64840be8d4cbcec4ae0458cba71f`
- binary-only files: **2**
- runtime-only types: **2**
- runtime-only method signatures: **10**
- source-only method signatures: **37**
- files with signature deltas: **12**
- binary-only sample: `SqlDataProvider\Data\GoldEquipTemplateLoadInfo.cs`, `SqlDataProvider\Data\UserGemStone.cs`
- runtime-only type sample: `SqlDataProvider.Data.GoldEquipTemplateLoadInfo`, `SqlDataProvider.Data.UserGemStone`
- runtime-only method sample: `SqlDataProvider.BaseClass.Sql_DbObject.SetDataTable(DataTable,string,List<SqlBulkCopyColumnMapping>)`, `SqlDataProvider.Data.ItemInfo.IsAdvanceDate()`, `SqlDataProvider.Data.PlayerInfo.IsValidadteTimeBox()`, `SqlDataProvider.Data.QuestDataInfo.setProgressConcoat()`, `SqlDataProvider.Data.UsersPetinfo.GetEquip()`, `SqlDataProvider.Data.UsersPetinfo.GetPetType(int)`, `SqlDataProvider.Data.UsersPetinfo.GetSkill()`, `SqlDataProvider.Data.UsersPetinfo.GetSkillEquip()`, `SqlDataProvider.Data.UsersPetinfo.ReduceProp(int)`, `SqlDataProvider.Data.UsersPetinfo.happyPercent()`

## Road / Bussiness
- best source: `DDTank41`
- runtime SHA256: `d6f9342d241a6ea94753bbc4e05acb3361bac852bcaf9f23c150010aaedf29bc`
- binary-only files: **2**
- runtime-only types: **2**
- runtime-only method signatures: **132**
- source-only method signatures: **250**
- files with signature deltas: **24**
- binary-only sample: `Bussiness\Interface\InterfaceType.cs`, `Bussiness\Managers\TreasureAwardMgr.cs`
- runtime-only type sample: `Bussiness.Interface.InterfaceType`, `Bussiness.Managers.TreasureAwardMgr`
- runtime-only method sample: `Bussiness.ActiveBussiness.AddActiveNumber(string,int)`, `Bussiness.ActiveBussiness.PullDown(int,string,int,refstring)`, `Bussiness.BaseBussiness..ctor()`, `Bussiness.CenterService.CenterServiceClient.ValidateLoginAndGetID(string,string,refint,refbool)`, `Bussiness.CenterService.ICenterService.ValidateLoginAndGetID(string,string,refint,refbool)`, `Bussiness.GameProperties.ConsortiaStrengExp(int)`, `Bussiness.GameProperties.HoleLevelUpExp(int)`, `Bussiness.GameProperties.RuneExp()`, `Bussiness.GameProperties.VIPStrengthenExp(int)`, `Bussiness.Interface.BaseInterface.CreateLogin(string,string,refstring,refint,string,refbool,bool,refbool,string,string)`, `Bussiness.Interface.BaseInterface.LoginGame(string,string,refbool)`, `Bussiness.Managers.AchievementMgr.GetNextLimit(int,int)`

## Road / Game.Base
- best source: `DDTank4.1`
- runtime SHA256: `7a8201c002726131b137aa9a35c29847b030614fa452e944cad2e72795cfef3c`
- binary-only files: **0**
- runtime-only types: **0**
- runtime-only method signatures: **0**
- source-only method signatures: **7**
- files with signature deltas: **2**

## Road / Game.Logic
- best source: `DDTank41`
- runtime SHA256: `85cd47cd952e6376280efd81a2efffe7d46570a73859e849e147ddc9bcf0de6d`
- binary-only files: **25**
- runtime-only types: **24**
- runtime-only method signatures: **314**
- source-only method signatures: **482**
- files with signature deltas: **45**
- binary-only sample: `Game\Logic\PetEffects\PetAddAttackEquipEffect.cs`, `Game\Logic\PetEffects\PetAddDefendEffect.cs`, `Game\Logic\PetEffects\PetAddDefendEquipEffect.cs`, `Game\Logic\PetEffects\PetAddLuckEquipEffect.cs`, `Game\Logic\PetEffects\PetAlwayNoHoleEquipEffect.cs`, `Game\Logic\PetEffects\PetAttackAroundEquipEffect.cs`, `Game\Logic\PetEffects\PetFatalEffect.cs`, `Game\Logic\PetEffects\PetNoHoleEffect.cs`, `Game\Logic\PetEffects\PetNoHoleEquipEffect.cs`, `Game\Logic\PetEffects\PetPlusAllTwoMpEquipEffect.cs`, `Game\Logic\PetEffects\PetPlusDameEquipEffect.cs`, `Game\Logic\PetEffects\PetPlusGuardEquipEffect.cs`
- runtime-only type sample: `Game.Logic.PetEffects.PetAddAttackEquipEffect`, `Game.Logic.PetEffects.PetAddDefendEffect`, `Game.Logic.PetEffects.PetAddDefendEquipEffect`, `Game.Logic.PetEffects.PetAddLuckEquipEffect`, `Game.Logic.PetEffects.PetAlwayNoHoleEquipEffect`, `Game.Logic.PetEffects.PetAttackAroundEquipEffect`, `Game.Logic.PetEffects.PetFatalEffect`, `Game.Logic.PetEffects.PetNoHoleEffect`, `Game.Logic.PetEffects.PetNoHoleEquipEffect`, `Game.Logic.PetEffects.PetPlusAllTwoMpEquipEffect`, `Game.Logic.PetEffects.PetPlusDameEquipEffect`, `Game.Logic.PetEffects.PetPlusGuardEquipEffect`
- runtime-only method sample: `Game.Logic.AI.ABrain..ctor()`, `Game.Logic.Actions.CallFunctionAction..ctor(LivingCallBack,int)`, `Game.Logic.Actions.FightAchievementAction..ctor(Living,int,int,int)`, `Game.Logic.Actions.LivingDieAction..ctor(Living,int)`, `Game.Logic.Actions.LivingRangeAttackingAction..ctor(Living,int,int,string,int,List<Player>)`, `Game.Logic.Actions.LivingRotateTurnAction..ctor(Player,int,int,string,int)`, `Game.Logic.Actions.LivingSealAction..ctor(Living,Player,int,int)`, `Game.Logic.BallMgr.LoadFromDatabase()`, `Game.Logic.BallMgr.LoadFromFiles(Dictionary<int,BallInfo>)`, `Game.Logic.BaseGame.AddBall(Point,bool)`, `Game.Logic.BaseGame.FindNextTurnedFightFootball()`, `Game.Logic.BaseGame.FindPhysicalObjByName(string,bool)`

## Road / Game.Server
- best source: `DDTank41`
- runtime SHA256: `b5773d6a3e7fb69f140c501c2c37f9b7fb30fb223734609875e1eeed0a81f78d`
- binary-only files: **73**
- runtime-only types: **73**
- runtime-only method signatures: **879**
- source-only method signatures: **735**
- files with signature deltas: **87**
- binary-only sample: `Game\Server\Achievements\BaseAchievement.cs`, `Game\Server\GameUtils\PlayerBeadInventory.cs`, `Game\Server\GameUtils\PlayerDice.cs`, `Game\Server\GameUtils\PlayerTreasure.cs`, `Game\Server\Packets\ActivityPackageType.cs`, `Game\Server\Packets\BattleGoundPackageType.cs`, `Game\Server\Packets\CampPackageType.cs`, `Game\Server\Packets\CatchBeastPackageType.cs`, `Game\Server\Packets\ChristmasPackageType.cs`, `Game\Server\Packets\Client\BeadHandle.cs`, `Game\Server\Packets\Client\BuyTransnationalGoodsHandler.cs`, `Game\Server\Packets\Client\CampBattleHandler.cs`
- runtime-only type sample: `Game.Server.Achievements.BaseAchievement`, `Game.Server.GameUtils.PlayerBeadInventory`, `Game.Server.GameUtils.PlayerDice`, `Game.Server.GameUtils.PlayerTreasure`, `Game.Server.Packets.ActivityPackageType`, `Game.Server.Packets.BattleGoundPackageType`, `Game.Server.Packets.CampPackageType`, `Game.Server.Packets.CatchBeastPackageType`, `Game.Server.Packets.ChristmasPackageType`, `Game.Server.Packets.Client.BeadHandle`, `Game.Server.Packets.Client.BuyTransnationalGoodsHandler`, `Game.Server.Packets.Client.CampBattleHandler`
- runtime-only method sample: `Game.Base.Packets.AbstractPacketLib.SendAchievementDatas(GamePlayer,BaseAchievement[])`, `Game.Base.Packets.AbstractPacketLib.SendActivityList(int)`, `Game.Base.Packets.AbstractPacketLib.SendBattleGoundOpen(int)`, `Game.Base.Packets.AbstractPacketLib.SendBattleGoundOver(int)`, `Game.Base.Packets.AbstractPacketLib.SendCSMBox(int)`, `Game.Base.Packets.AbstractPacketLib.SendCampBattleOpenClose(int,bool)`, `Game.Base.Packets.AbstractPacketLib.SendCatchBeastOpen(int,bool)`, `Game.Base.Packets.AbstractPacketLib.SendCollectInfor(int,byte)`, `Game.Base.Packets.AbstractPacketLib.SendConsortia(int,bool,string,int)`, `Game.Base.Packets.AbstractPacketLib.SendConsortiaBattleOpenClose(int,bool)`, `Game.Base.Packets.AbstractPacketLib.SendConsortiaCreate(string,bool,int,string,string,int,string,int,int)`, `Game.Base.Packets.AbstractPacketLib.SendConsortiaInvite(string,bool,string,int)`

## Road / GameServerScripts
- best source: `DDTank-3.0`
- runtime SHA256: `ae10f9def9a42d50b35c2079788f6a75df80ab0e35b35cc9801dee5b0029abec`
- binary-only files: **526**
- runtime-only types: **526**
- runtime-only method signatures: **4894**
- source-only method signatures: **14**
- files with signature deltas: **8**
- binary-only sample: `GameServerScript\AI\Game\Activity77.cs`, `GameServerScript\AI\Game\AntCaveNormalGame.cs`, `GameServerScript\AI\Game\AntCaveSimpleGame.cs`, `GameServerScript\AI\Game\BossGuild.cs`, `GameServerScript\AI\Game\CampBattle1.cs`, `GameServerScript\AI\Game\CampBattle10.cs`, `GameServerScript\AI\Game\CampBattle11.cs`, `GameServerScript\AI\Game\CampBattle12.cs`, `GameServerScript\AI\Game\CampBattle13.cs`, `GameServerScript\AI\Game\CampBattle14.cs`, `GameServerScript\AI\Game\CampBattle15.cs`, `GameServerScript\AI\Game\CampBattle16.cs`
- runtime-only type sample: `GameServerScript.AI.Game.Activity77`, `GameServerScript.AI.Game.AntCaveNormalGame`, `GameServerScript.AI.Game.AntCaveSimpleGame`, `GameServerScript.AI.Game.BossGuild`, `GameServerScript.AI.Game.CampBattle1`, `GameServerScript.AI.Game.CampBattle10`, `GameServerScript.AI.Game.CampBattle11`, `GameServerScript.AI.Game.CampBattle12`, `GameServerScript.AI.Game.CampBattle13`, `GameServerScript.AI.Game.CampBattle14`, `GameServerScript.AI.Game.CampBattle15`, `GameServerScript.AI.Game.CampBattle16`
- runtime-only method sample: `GameServerScript.AI.Game.Activity77.CalculateScoreGrade(int)`, `GameServerScript.AI.Game.Activity77.OnCreated()`, `GameServerScript.AI.Game.Activity77.OnGameOverAllSession()`, `GameServerScript.AI.Game.Activity77.OnPrepated()`, `GameServerScript.AI.Game.AntCaveNormalGame.CalculateScoreGrade(int)`, `GameServerScript.AI.Game.AntCaveNormalGame.OnCreated()`, `GameServerScript.AI.Game.AntCaveNormalGame.OnGameOverAllSession()`, `GameServerScript.AI.Game.AntCaveNormalGame.OnPrepated()`, `GameServerScript.AI.Game.AntCaveSimpleGame.CalculateScoreGrade(int)`, `GameServerScript.AI.Game.AntCaveSimpleGame.OnCreated()`, `GameServerScript.AI.Game.AntCaveSimpleGame.OnGameOverAllSession()`, `GameServerScript.AI.Game.AntCaveSimpleGame.OnPrepated()`

## Road / Road.Service
- best source: `DDTank41`
- runtime SHA256: `aa3c8793c3d3f2551aa559b93d1fdf08600a0d8951dc09bb96d72b9105c8d22b`
- binary-only files: **0**
- runtime-only types: **0**
- runtime-only method signatures: **0**
- source-only method signatures: **3**
- files with signature deltas: **1**

## Road / SqlDataProvider
- best source: `DDTank41`
- runtime SHA256: `4e385131d3024ed55f3e5b8dda60b866fe15f57bed42b5da4c6a06cba14e0691`
- binary-only files: **2**
- runtime-only types: **2**
- runtime-only method signatures: **10**
- source-only method signatures: **37**
- files with signature deltas: **12**
- binary-only sample: `SqlDataProvider\Data\GoldEquipTemplateLoadInfo.cs`, `SqlDataProvider\Data\UserGemStone.cs`
- runtime-only type sample: `SqlDataProvider.Data.GoldEquipTemplateLoadInfo`, `SqlDataProvider.Data.UserGemStone`
- runtime-only method sample: `SqlDataProvider.BaseClass.Sql_DbObject.SetDataTable(DataTable,string,List<SqlBulkCopyColumnMapping>)`, `SqlDataProvider.Data.ItemInfo.IsAdvanceDate()`, `SqlDataProvider.Data.PlayerInfo.IsValidadteTimeBox()`, `SqlDataProvider.Data.QuestDataInfo.setProgressConcoat()`, `SqlDataProvider.Data.UsersPetinfo.GetEquip()`, `SqlDataProvider.Data.UsersPetinfo.GetPetType(int)`, `SqlDataProvider.Data.UsersPetinfo.GetSkill()`, `SqlDataProvider.Data.UsersPetinfo.GetSkillEquip()`, `SqlDataProvider.Data.UsersPetinfo.ReduceProp(int)`, `SqlDataProvider.Data.UsersPetinfo.happyPercent()`
