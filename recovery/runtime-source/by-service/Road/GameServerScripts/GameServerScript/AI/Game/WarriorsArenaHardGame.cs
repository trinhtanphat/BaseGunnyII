using Game.Logic.AI;

namespace GameServerScript.AI.Game;

internal class WarriorsArenaHardGame : APVEGameControl
{
	public override void OnCreated()
	{
		base.Game.SetupMissions("13201, 13202, 13203, 13204");
		base.Game.TotalMissionCount = 4;
	}

	public override void OnPrepated()
	{
		base.OnPrepated();
		base.Game.SessionId = 0;
	}

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 900)
		{
			return 3;
		}
		if (score > 825)
		{
			return 2;
		}
		if (score > 725)
		{
			return 1;
		}
		return 0;
	}

	public override void OnGameOverAllSession()
	{
		base.OnGameOverAllSession();
	}
}
