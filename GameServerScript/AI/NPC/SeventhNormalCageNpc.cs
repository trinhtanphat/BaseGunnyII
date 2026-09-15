using Game.Logic.AI;

namespace GameServerScript.AI.NPC
{

public class SeventhNormalCageNpc : ABrain
{
	public override void OnStartAttacking()
	{
		if (base.Body.Blood == 0)
		{
			Out();
		}
	}

	private void Out()
	{
		base.Body.PlayMovie("out", 3000, 0);
	}
}
}
