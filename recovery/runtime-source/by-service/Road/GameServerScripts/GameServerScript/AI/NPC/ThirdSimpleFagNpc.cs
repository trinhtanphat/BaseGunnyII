using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class ThirdSimpleFagNpc : ABrain
{
	private Player m_target = null;

	private int m_targetDis = 0;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		foreach (Player item in allLivingPlayers)
		{
			item.AddEffect(new ReduceStrengthEffect(2, 50), 0);
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		foreach (Player item in allLivingPlayers)
		{
			base.Body.Say("Haha, tôi là đầy sức mạnh!", 1, 0);
			item.EffectList.Remove(null);
		}
	}
}
