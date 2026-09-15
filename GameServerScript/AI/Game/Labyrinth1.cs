using Game.Logic.AI;

namespace GameServerScript.AI.Game
{

public class Labyrinth1 : APVEGameControl
{
	public override void OnCreated()
	{
		string text = "40001,40002,40003,40004,40005,40006,40007,40008,40009,40010";
		text += ",40011,40012,40013,40014,40015,40016,40017,40018,40019,40020";
		text += ",40021,40022,40023,40024,40025,40026,40027,40028,40029,40030";
		text += ",40031,40032,40033,40034,40035,40036,40037,40038,40039,40040";
		base.Game.SetupMissions(text);
		base.Game.TotalMissionCount = text.Split(',').Length;
	}

	public override void OnPrepated()
	{
	}

	public override int CalculateScoreGrade(int score)
	{
		if (score > 800)
		{
			return 3;
		}
		if (score > 725)
		{
			return 2;
		}
		if (score > 650)
		{
			return 1;
		}
		return 0;
	}

	public override void OnGameOverAllSession()
	{
	}
}
}
