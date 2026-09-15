using Game.Logic.AI;

namespace GameServerScript.AI.NPC;

public class NullAi : ABrain
{
	private int m_attackTurn = 0;

	private int currentCount = 0;

	private static string[] AllAttackChat = new string[0];

	private static string[] ShootChat = new string[0];

	private static string[] KillPlayerChat = new string[0];

	private static string[] CallChat = new string[0];

	private static string[] ShootedChat = new string[0];

	private static string[] JumpChat = new string[0];

	private static string[] KillAttackChat = new string[0];

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void KillAttack(int fx, int tx)
	{
	}

	private void AllAttack()
	{
	}

	private void PersonalAttack()
	{
	}

	private void Summon()
	{
	}

	private void NextAttack()
	{
	}

	private void ChangeDirection(int count)
	{
	}

	public void CreateChild()
	{
	}
}
