using Game.Logic.Phy.Object;

namespace Game.Logic.Actions;

public class FightAchievementAction : BaseAction
{
	private Living m_living;

	private int m_num;

	private int m_type;

	private int m_delay;

	public FightAchievementAction(Living living, int type, int num, int delay)
		: base(delay, 1500)
	{
		m_living = living;
		m_num = num;
		m_type = type;
		m_delay = delay;
	}

	protected override void ExecuteImp(BaseGame game, long tick)
	{
		game.SendFightAchievement(m_living, m_type, m_num, m_delay);
		Finish(tick);
	}
}
