using System.Reflection;
using log4net;

namespace Game.Logic.Actions;

public class CheckPVPGameStateAction : IAction
{
	private long m_tick;

	private bool m_isFinished;

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public CheckPVPGameStateAction(int delay)
	{
		m_isFinished = false;
		m_tick += TickHelper.GetTickCount() + delay;
	}

	public void Execute(BaseGame game, long tick)
	{
		if (m_tick > tick)
		{
			return;
		}
		if (game is PVPGame pVPGame)
		{
			switch (game.GameState)
			{
			case eGameState.Inited:
				pVPGame.Prepare();
				break;
			case eGameState.Prepared:
				pVPGame.StartLoading();
				break;
			case eGameState.Loading:
				if (pVPGame.IsAllComplete())
				{
					pVPGame.StartGame();
				}
				break;
			case eGameState.Playing:
				if (pVPGame.CurrentPlayer == null || !pVPGame.CurrentPlayer.IsAttacking)
				{
					if (pVPGame.CanGameOver())
					{
						pVPGame.GameOver();
					}
					else
					{
						pVPGame.NextTurn();
					}
				}
				break;
			case eGameState.GameOver:
				pVPGame.Stop();
				break;
			}
		}
		m_isFinished = true;
	}

	public bool IsFinished(long tick)
	{
		return m_isFinished;
	}
}
