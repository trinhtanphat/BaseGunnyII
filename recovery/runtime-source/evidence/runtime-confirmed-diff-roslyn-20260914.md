# Confirmed runtime-only diff (repo-wide Roslyn AST)

Exact fully-qualified type/method signatures were checked against every first-party C# file in the best-source repository.

## Road / GameServerScripts
- best source: `DDTank-3.0`
- runtime SHA256: `ae10f9def9a42d50b35c2079788f6a75df80ab0e35b35cc9801dee5b0029abec`
- confirmed runtime-only types: **525** / initial 526
- found elsewhere in repo (types): **1**
- confirmed runtime-only method signatures: **4893** / initial 4894
- found elsewhere in repo (methods): **1**
- type sample: `GameServerScript.AI.Game._20Skill`, `GameServerScript.AI.Game._65Skill`, `GameServerScript.AI.Game.Activity77`, `GameServerScript.AI.Game.AntCaveNormalGame`, `GameServerScript.AI.Game.AntCaveSimpleGame`, `GameServerScript.AI.Game.BossGuild`, `GameServerScript.AI.Game.CampBattle1`, `GameServerScript.AI.Game.CampBattle10`, `GameServerScript.AI.Game.CampBattle11`, `GameServerScript.AI.Game.CampBattle12`, `GameServerScript.AI.Game.CampBattle13`, `GameServerScript.AI.Game.CampBattle14`
- method sample: `GameServerScript.AI.Game._20Skill.CalculateScoreGrade(int)`, `GameServerScript.AI.Game._20Skill.OnCreated()`, `GameServerScript.AI.Game._20Skill.OnGameOverAllSession()`, `GameServerScript.AI.Game._20Skill.OnPrepated()`, `GameServerScript.AI.Game._65Skill.CalculateScoreGrade(int)`, `GameServerScript.AI.Game._65Skill.OnCreated()`, `GameServerScript.AI.Game._65Skill.OnGameOverAllSession()`, `GameServerScript.AI.Game._65Skill.OnPrepated()`, `GameServerScript.AI.Game.Activity77.CalculateScoreGrade(int)`, `GameServerScript.AI.Game.Activity77.OnCreated()`, `GameServerScript.AI.Game.Activity77.OnGameOverAllSession()`, `GameServerScript.AI.Game.Activity77.OnPrepated()`

## Road / Game.Server
- best source: `DDTank41`
- runtime SHA256: `b5773d6a3e7fb69f140c501c2c37f9b7fb30fb223734609875e1eeed0a81f78d`
- confirmed runtime-only types: **73** / initial 73
- found elsewhere in repo (types): **0**
- confirmed runtime-only method signatures: **870** / initial 879
- found elsewhere in repo (methods): **9**
- type sample: `Game.Server.Achievements.BaseAchievement`, `Game.Server.GameUtils.PlayerBeadInventory`, `Game.Server.GameUtils.PlayerDice`, `Game.Server.GameUtils.PlayerTreasure`, `Game.Server.Packets.ActivityPackageType`, `Game.Server.Packets.BattleGoundPackageType`, `Game.Server.Packets.CampPackageType`, `Game.Server.Packets.CatchBeastPackageType`, `Game.Server.Packets.ChristmasPackageType`, `Game.Server.Packets.Client.BeadHandle`, `Game.Server.Packets.Client.BuyTransnationalGoodsHandler`, `Game.Server.Packets.Client.CampBattleHandler`
- method sample: `Game.Base.Packets.AbstractPacketLib.SendAchievementDatas(GamePlayer,BaseAchievement[])`, `Game.Base.Packets.AbstractPacketLib.SendActivityList(int)`, `Game.Base.Packets.AbstractPacketLib.SendBattleGoundOpen(int)`, `Game.Base.Packets.AbstractPacketLib.SendBattleGoundOver(int)`, `Game.Base.Packets.AbstractPacketLib.SendCampBattleOpenClose(int,bool)`, `Game.Base.Packets.AbstractPacketLib.SendCatchBeastOpen(int,bool)`, `Game.Base.Packets.AbstractPacketLib.SendCollectInfor(int,byte)`, `Game.Base.Packets.AbstractPacketLib.sendCompose(GamePlayer)`, `Game.Base.Packets.AbstractPacketLib.SendConsortia(int,bool,string,int)`, `Game.Base.Packets.AbstractPacketLib.sendConsortiaApplyStatusOut(bool,bool,int)`, `Game.Base.Packets.AbstractPacketLib.SendConsortiaBattleOpenClose(int,bool)`, `Game.Base.Packets.AbstractPacketLib.sendConsortiaChangeChairman(string,bool,string,int)`

## Fight / Game.Logic
- best source: `DDTank41`
- runtime SHA256: `c55aa6d9dca93b1d7183d7f9e46f618f38e66e7b69ea2de72bd4c75ada66d661`
- confirmed runtime-only types: **24** / initial 24
- found elsewhere in repo (types): **0**
- confirmed runtime-only method signatures: **314** / initial 314
- found elsewhere in repo (methods): **0**
- type sample: `Game.Logic.PetEffects.PetAddAttackEquipEffect`, `Game.Logic.PetEffects.PetAddDefendEffect`, `Game.Logic.PetEffects.PetAddDefendEquipEffect`, `Game.Logic.PetEffects.PetAddLuckEquipEffect`, `Game.Logic.PetEffects.PetAlwayNoHoleEquipEffect`, `Game.Logic.PetEffects.PetAttackAroundEquipEffect`, `Game.Logic.PetEffects.PetFatalEffect`, `Game.Logic.PetEffects.PetNoHoleEffect`, `Game.Logic.PetEffects.PetNoHoleEquipEffect`, `Game.Logic.PetEffects.PetPlusAllTwoMpEquipEffect`, `Game.Logic.PetEffects.PetPlusDameEquipEffect`, `Game.Logic.PetEffects.PetPlusGuardEquipEffect`
- method sample: `Game.Logic.Actions.CallFunctionAction..ctor(LivingCallBack,int)`, `Game.Logic.Actions.FightAchievementAction..ctor(Living,int,int,int)`, `Game.Logic.Actions.LivingDieAction..ctor(Living,int)`, `Game.Logic.Actions.LivingRangeAttackingAction..ctor(Living,int,int,string,int,List<Player>)`, `Game.Logic.Actions.LivingRotateTurnAction..ctor(Player,int,int,string,int)`, `Game.Logic.Actions.LivingSealAction..ctor(Living,Player,int,int)`, `Game.Logic.AI.ABrain..ctor()`, `Game.Logic.BallMgr.LoadFromDatabase()`, `Game.Logic.BallMgr.LoadFromFiles(Dictionary<int,BallInfo>)`, `Game.Logic.BaseGame.AddBall(Point,bool)`, `Game.Logic.BaseGame.FindNextTurnedFightFootball()`, `Game.Logic.BaseGame.FindPhysicalObjByName(string,bool)`

## Road / Game.Logic
- best source: `DDTank41`
- runtime SHA256: `85cd47cd952e6376280efd81a2efffe7d46570a73859e849e147ddc9bcf0de6d`
- confirmed runtime-only types: **24** / initial 24
- found elsewhere in repo (types): **0**
- confirmed runtime-only method signatures: **314** / initial 314
- found elsewhere in repo (methods): **0**
- type sample: `Game.Logic.PetEffects.PetAddAttackEquipEffect`, `Game.Logic.PetEffects.PetAddDefendEffect`, `Game.Logic.PetEffects.PetAddDefendEquipEffect`, `Game.Logic.PetEffects.PetAddLuckEquipEffect`, `Game.Logic.PetEffects.PetAlwayNoHoleEquipEffect`, `Game.Logic.PetEffects.PetAttackAroundEquipEffect`, `Game.Logic.PetEffects.PetFatalEffect`, `Game.Logic.PetEffects.PetNoHoleEffect`, `Game.Logic.PetEffects.PetNoHoleEquipEffect`, `Game.Logic.PetEffects.PetPlusAllTwoMpEquipEffect`, `Game.Logic.PetEffects.PetPlusDameEquipEffect`, `Game.Logic.PetEffects.PetPlusGuardEquipEffect`
- method sample: `Game.Logic.Actions.CallFunctionAction..ctor(LivingCallBack,int)`, `Game.Logic.Actions.FightAchievementAction..ctor(Living,int,int,int)`, `Game.Logic.Actions.LivingDieAction..ctor(Living,int)`, `Game.Logic.Actions.LivingRangeAttackingAction..ctor(Living,int,int,string,int,List<Player>)`, `Game.Logic.Actions.LivingRotateTurnAction..ctor(Player,int,int,string,int)`, `Game.Logic.Actions.LivingSealAction..ctor(Living,Player,int,int)`, `Game.Logic.AI.ABrain..ctor()`, `Game.Logic.BallMgr.LoadFromDatabase()`, `Game.Logic.BallMgr.LoadFromFiles(Dictionary<int,BallInfo>)`, `Game.Logic.BaseGame.AddBall(Point,bool)`, `Game.Logic.BaseGame.FindNextTurnedFightFootball()`, `Game.Logic.BaseGame.FindPhysicalObjByName(string,bool)`

## center / Bussiness
- best source: `DDTank41`
- runtime SHA256: `24fa15ff6045e87736624dddb7cacdd9bfbfbe0414d6ba6ab57a3a132528cf0f`
- confirmed runtime-only types: **2** / initial 2
- found elsewhere in repo (types): **0**
- confirmed runtime-only method signatures: **132** / initial 141
- found elsewhere in repo (methods): **9**
- type sample: `Bussiness.Interface.InterfaceType`, `Bussiness.Managers.TreasureAwardMgr`
- method sample: `Bussiness.ActiveBussiness.AddActiveNumber(string,int)`, `Bussiness.ActiveBussiness.PullDown(int,string,int,refstring)`, `Bussiness.BaseBussiness..ctor()`, `Bussiness.CenterService.CenterServiceClient.ValidateLoginAndGetID(string,string,refint,refbool)`, `Bussiness.CenterService.ICenterService.ValidateLoginAndGetID(string,string,refint,refbool)`, `Bussiness.GameProperties.ConsortiaStrengExp(int)`, `Bussiness.GameProperties.HoleLevelUpExp(int)`, `Bussiness.GameProperties.RuneExp()`, `Bussiness.GameProperties.VIPStrengthenExp(int)`, `Bussiness.Interface.BaseInterface.CreateLogin(string,string,refstring,refint,string,refbool,bool,refbool,string,string)`, `Bussiness.Interface.BaseInterface.LoginGame(string,string,refbool)`, `Bussiness.Managers.AchievementMgr.GetNextLimit(int,int)`

## Fight / Bussiness
- best source: `DDTank41`
- runtime SHA256: `d6f9342d241a6ea94753bbc4e05acb3361bac852bcaf9f23c150010aaedf29bc`
- confirmed runtime-only types: **2** / initial 2
- found elsewhere in repo (types): **0**
- confirmed runtime-only method signatures: **132** / initial 132
- found elsewhere in repo (methods): **0**
- type sample: `Bussiness.Interface.InterfaceType`, `Bussiness.Managers.TreasureAwardMgr`
- method sample: `Bussiness.ActiveBussiness.AddActiveNumber(string,int)`, `Bussiness.ActiveBussiness.PullDown(int,string,int,refstring)`, `Bussiness.BaseBussiness..ctor()`, `Bussiness.CenterService.CenterServiceClient.ValidateLoginAndGetID(string,string,refint,refbool)`, `Bussiness.CenterService.ICenterService.ValidateLoginAndGetID(string,string,refint,refbool)`, `Bussiness.GameProperties.ConsortiaStrengExp(int)`, `Bussiness.GameProperties.HoleLevelUpExp(int)`, `Bussiness.GameProperties.RuneExp()`, `Bussiness.GameProperties.VIPStrengthenExp(int)`, `Bussiness.Interface.BaseInterface.CreateLogin(string,string,refstring,refint,string,refbool,bool,refbool,string,string)`, `Bussiness.Interface.BaseInterface.LoginGame(string,string,refbool)`, `Bussiness.Managers.AchievementMgr.GetNextLimit(int,int)`

## Road / Bussiness
- best source: `DDTank41`
- runtime SHA256: `d6f9342d241a6ea94753bbc4e05acb3361bac852bcaf9f23c150010aaedf29bc`
- confirmed runtime-only types: **2** / initial 2
- found elsewhere in repo (types): **0**
- confirmed runtime-only method signatures: **132** / initial 132
- found elsewhere in repo (methods): **0**
- type sample: `Bussiness.Interface.InterfaceType`, `Bussiness.Managers.TreasureAwardMgr`
- method sample: `Bussiness.ActiveBussiness.AddActiveNumber(string,int)`, `Bussiness.ActiveBussiness.PullDown(int,string,int,refstring)`, `Bussiness.BaseBussiness..ctor()`, `Bussiness.CenterService.CenterServiceClient.ValidateLoginAndGetID(string,string,refint,refbool)`, `Bussiness.CenterService.ICenterService.ValidateLoginAndGetID(string,string,refint,refbool)`, `Bussiness.GameProperties.ConsortiaStrengExp(int)`, `Bussiness.GameProperties.HoleLevelUpExp(int)`, `Bussiness.GameProperties.RuneExp()`, `Bussiness.GameProperties.VIPStrengthenExp(int)`, `Bussiness.Interface.BaseInterface.CreateLogin(string,string,refstring,refint,string,refbool,bool,refbool,string,string)`, `Bussiness.Interface.BaseInterface.LoginGame(string,string,refbool)`, `Bussiness.Managers.AchievementMgr.GetNextLimit(int,int)`

## Fight / GameServerScripts
- best source: `BaseGunnyII`
- runtime SHA256: `e4d0871d43f7cf6e7689c65b3ce2b738b7e140b0ae2db4f3e7cd0c8544bd84f0`
- confirmed runtime-only types: **2** / initial 3
- found elsewhere in repo (types): **1**
- confirmed runtime-only method signatures: **24** / initial 25
- found elsewhere in repo (methods): **1**
- type sample: `GameServerScript.AI.NPC.NullAi`, `GameServerScript.AI.NPC.SeizeNpcAi`
- method sample: `GameServerScript.AI.NPC.NullAi.AllAttack()`, `GameServerScript.AI.NPC.NullAi.ChangeDirection(int)`, `GameServerScript.AI.NPC.NullAi.CreateChild()`, `GameServerScript.AI.NPC.NullAi.KillAttack(int,int)`, `GameServerScript.AI.NPC.NullAi.NextAttack()`, `GameServerScript.AI.NPC.NullAi.OnBeginNewTurn()`, `GameServerScript.AI.NPC.NullAi.OnBeginSelfTurn()`, `GameServerScript.AI.NPC.NullAi.OnCreated()`, `GameServerScript.AI.NPC.NullAi.OnStartAttacking()`, `GameServerScript.AI.NPC.NullAi.OnStopAttacking()`, `GameServerScript.AI.NPC.NullAi.PersonalAttack()`, `GameServerScript.AI.NPC.NullAi.Summon()`

## center / SqlDataProvider
- best source: `DDTank41`
- runtime SHA256: `0a1f788f4664ddfba3394b27104ec31ba3ab64840be8d4cbcec4ae0458cba71f`
- confirmed runtime-only types: **1** / initial 2
- found elsewhere in repo (types): **1**
- confirmed runtime-only method signatures: **10** / initial 10
- found elsewhere in repo (methods): **0**
- type sample: `SqlDataProvider.Data.GoldEquipTemplateLoadInfo`
- method sample: `SqlDataProvider.BaseClass.Sql_DbObject.SetDataTable(DataTable,string,List<SqlBulkCopyColumnMapping>)`, `SqlDataProvider.Data.ItemInfo.IsAdvanceDate()`, `SqlDataProvider.Data.PlayerInfo.IsValidadteTimeBox()`, `SqlDataProvider.Data.QuestDataInfo.setProgressConcoat()`, `SqlDataProvider.Data.UsersPetinfo.GetEquip()`, `SqlDataProvider.Data.UsersPetinfo.GetPetType(int)`, `SqlDataProvider.Data.UsersPetinfo.GetSkill()`, `SqlDataProvider.Data.UsersPetinfo.GetSkillEquip()`, `SqlDataProvider.Data.UsersPetinfo.happyPercent()`, `SqlDataProvider.Data.UsersPetinfo.ReduceProp(int)`

## Fight / SqlDataProvider
- best source: `DDTank41`
- runtime SHA256: `0a1f788f4664ddfba3394b27104ec31ba3ab64840be8d4cbcec4ae0458cba71f`
- confirmed runtime-only types: **1** / initial 2
- found elsewhere in repo (types): **1**
- confirmed runtime-only method signatures: **10** / initial 10
- found elsewhere in repo (methods): **0**
- type sample: `SqlDataProvider.Data.GoldEquipTemplateLoadInfo`
- method sample: `SqlDataProvider.BaseClass.Sql_DbObject.SetDataTable(DataTable,string,List<SqlBulkCopyColumnMapping>)`, `SqlDataProvider.Data.ItemInfo.IsAdvanceDate()`, `SqlDataProvider.Data.PlayerInfo.IsValidadteTimeBox()`, `SqlDataProvider.Data.QuestDataInfo.setProgressConcoat()`, `SqlDataProvider.Data.UsersPetinfo.GetEquip()`, `SqlDataProvider.Data.UsersPetinfo.GetPetType(int)`, `SqlDataProvider.Data.UsersPetinfo.GetSkill()`, `SqlDataProvider.Data.UsersPetinfo.GetSkillEquip()`, `SqlDataProvider.Data.UsersPetinfo.happyPercent()`, `SqlDataProvider.Data.UsersPetinfo.ReduceProp(int)`

## Road / SqlDataProvider
- best source: `DDTank41`
- runtime SHA256: `4e385131d3024ed55f3e5b8dda60b866fe15f57bed42b5da4c6a06cba14e0691`
- confirmed runtime-only types: **1** / initial 2
- found elsewhere in repo (types): **1**
- confirmed runtime-only method signatures: **10** / initial 10
- found elsewhere in repo (methods): **0**
- type sample: `SqlDataProvider.Data.GoldEquipTemplateLoadInfo`
- method sample: `SqlDataProvider.BaseClass.Sql_DbObject.SetDataTable(DataTable,string,List<SqlBulkCopyColumnMapping>)`, `SqlDataProvider.Data.ItemInfo.IsAdvanceDate()`, `SqlDataProvider.Data.PlayerInfo.IsValidadteTimeBox()`, `SqlDataProvider.Data.QuestDataInfo.setProgressConcoat()`, `SqlDataProvider.Data.UsersPetinfo.GetEquip()`, `SqlDataProvider.Data.UsersPetinfo.GetPetType(int)`, `SqlDataProvider.Data.UsersPetinfo.GetSkill()`, `SqlDataProvider.Data.UsersPetinfo.GetSkillEquip()`, `SqlDataProvider.Data.UsersPetinfo.happyPercent()`, `SqlDataProvider.Data.UsersPetinfo.ReduceProp(int)`

## Fight / Fighting.Server
- best source: `DDTank41`
- runtime SHA256: `bcda33f114d84465ec6be03bf73f5d62d6d3c4e09c9eae50fef91532140a78a9`
- confirmed runtime-only types: **0** / initial 0
- found elsewhere in repo (types): **0**
- confirmed runtime-only method signatures: **25** / initial 25
- found elsewhere in repo (methods): **0**
- method sample: `Fighting.Server.GameObjects.ProxyPlayer..ctor(ServerClient,PlayerInfo,UserMatchInfo,ProxyPlayerInfo,UsersPetinfo,List<BufferInfo>,List<ItemInfo>,List<BufferInfo>)`, `Fighting.Server.GameObjects.ProxyPlayer.AddActiveMoney(int)`, `Fighting.Server.GameObjects.ProxyPlayer.AddLeagueMoney(int)`, `Fighting.Server.GameObjects.ProxyPlayer.AddMedal(int)`, `Fighting.Server.GameObjects.ProxyPlayer.AddPrestige(bool)`, `Fighting.Server.GameObjects.ProxyPlayer.AddTemplate(ItemInfo,eBageType,int,eItemNotice,eItemNotice)`, `Fighting.Server.GameObjects.ProxyPlayer.FootballTakeOut(bool)`, `Fighting.Server.GameObjects.ProxyPlayer.GetFightFootballStyle(int)`, `Fighting.Server.GameObjects.ProxyPlayer.OnGameOver(AbstractGame,bool,int)`, `Fighting.Server.Games.GameMgr.CanUseBuff(eGameType)`, `Fighting.Server.Games.GameMgr.ClearStoppedGames(long)`, `Fighting.Server.Games.GameMgr.GameThread()`

## center / Center.Server
- best source: `DDTank41`
- runtime SHA256: `f446ce3aef72e2b57fb626a11a125c16caf436fd186a89f65bc4840b245a30c2`
- confirmed runtime-only types: **0** / initial 0
- found elsewhere in repo (types): **0**
- confirmed runtime-only method signatures: **15** / initial 15
- found elsewhere in repo (methods): **0**
- method sample: `Center.Server.ServerClient.HandleEventRank(GSPacketIn)`, `Center.Server.ServerClient.HandleWorldEvent(GSPacketIn)`, `Center.Server.WorldMgr.AddOrUpdateLanternriddles(int,LanternriddlesInfo)`, `Center.Server.WorldMgr.GetAllLuckyStarRank()`, `Center.Server.WorldMgr.GetLanternriddles(int)`, `Center.Server.WorldMgr.ResetLightriddleRank()`, `Center.Server.WorldMgr.ResetLuckStar()`, `Center.Server.WorldMgr.SavekyStarToDatabase()`, `Center.Server.WorldMgr.SaveLuckyStarRewardRecord()`, `Center.Server.WorldMgr.SelectTopEight()`, `Center.Server.WorldMgr.SendLightriddleTopEightAward()`, `Center.Server.WorldMgr.SendLuckyStarTopTenAward()`

## center / Center.Service
- best source: `DDTank41`
- runtime SHA256: `5c24462c871d8a70dafcd9ac2a9b747e6e81457abfda7722ba1c75368b4491a9`
- confirmed runtime-only types: **0** / initial 0
- found elsewhere in repo (types): **0**
- confirmed runtime-only method signatures: **2** / initial 9
- found elsewhere in repo (methods): **7**
- method sample: `Game.Service.actions.ConsoleStart.Reload(eReloadType)`, `Game.Service.actions.ConsoleStart.StartServer()`

## center / Game.Base
- best source: `DDTank4.1`
- runtime SHA256: `98af5762474f3c328213db101e0e2cfce95be297662687a79f50fcfe0b84fa7c`
- confirmed runtime-only types: **0** / initial 0
- found elsewhere in repo (types): **0**
- confirmed runtime-only method signatures: **0** / initial 0
- found elsewhere in repo (methods): **0**

## Fight / Fighting.Service
- best source: `DDTank41`
- runtime SHA256: `dc4eb09bc5f0ec10ae018da823a4896b826b4ae58d498da89a83e0ad29c6ed9b`
- confirmed runtime-only types: **0** / initial 0
- found elsewhere in repo (types): **0**
- confirmed runtime-only method signatures: **0** / initial 0
- found elsewhere in repo (methods): **0**

## Fight / Game.Base
- best source: `DDTank4.1`
- runtime SHA256: `5851154b8fef454261006a00b92e1b89f4d1d254ef519ee57219b2784d265fb3`
- confirmed runtime-only types: **0** / initial 0
- found elsewhere in repo (types): **0**
- confirmed runtime-only method signatures: **0** / initial 0
- found elsewhere in repo (methods): **0**

## Road / Game.Base
- best source: `DDTank4.1`
- runtime SHA256: `7a8201c002726131b137aa9a35c29847b030614fa452e944cad2e72795cfef3c`
- confirmed runtime-only types: **0** / initial 0
- found elsewhere in repo (types): **0**
- confirmed runtime-only method signatures: **0** / initial 0
- found elsewhere in repo (methods): **0**

## Road / Road.Service
- best source: `DDTank41`
- runtime SHA256: `aa3c8793c3d3f2551aa559b93d1fdf08600a0d8951dc09bb96d72b9105c8d22b`
- confirmed runtime-only types: **0** / initial 0
- found elsewhere in repo (types): **0**
- confirmed runtime-only method signatures: **0** / initial 0
- found elsewhere in repo (methods): **0**
