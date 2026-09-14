using System.Collections.Generic;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class AC30001 : AMissionControl
{
	private SimpleBoss boss = null;

	private int npcID = 30001;

	private int bossID = 30002;

	private LivingConfig config;

	private List<SimpleNpc> someNpc = new List<SimpleNpc>();

	private int[] birthX = new int[10] { 443, 515, 683, 723, 800, 606, 785, 842, 910, 1075 };

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 1750)
		{
			return 3;
		}
		if (score > 1675)
		{
			return 2;
		}
		if (score > 1600)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		int[] npcIds = new int[2] { npcID, bossID };
		int[] npcIds2 = new int[1] { npcID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(2, "image/game/effect/0/294b.swf", "asset.game.zero.294b");
		base.Game.SetMap(1244);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		createNPC();
		config = base.Game.BaseLivingConfig();
		config.IsFly = true;
		config.IsWorldBoss = true;
	}

	private void createNPC()
	{
		int[] array = birthX;
		foreach (int x in array)
		{
			int y = base.Game.Random.Next(478, 674);
			someNpc.Add(base.Game.CreateNpc(npcID, x, y, 0, 1, config));
		}
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (base.Game.GetLivedLivings().Count < 3)
		{
			createNPC();
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override bool CanGameOver()
	{
		if (base.Game.TotalKillCount > 15 && boss == null)
		{
			base.Game.ClearAllChild();
			boss = base.Game.CreateBoss(bossID, 944, 481, -1, 0, "", config);
			boss.SetRelateDemagemRect(-200, -179, 272, 200);
			boss.Say(LanguageMgr.GetTranslation("GameServerScript.AI.Messions.DCSM2002.msg1"), 0, 200, 0);
			List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
			Player player = base.Game.FindRandomPlayer();
			int num = 0;
			if (player != null)
			{
				num = player.Delay;
			}
			foreach (Player item in allFightPlayers)
			{
				if (item.Delay < num)
				{
					num = item.Delay;
				}
			}
			boss.State = 10;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		return base.Game.TotalKillCount;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (base.Game.TotalKillCount > 100)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			item.PlayerDetail.UpdatePveResult("worldboss", item.TotalDameLiving, base.Game.IsWin);
		}
	}
}
